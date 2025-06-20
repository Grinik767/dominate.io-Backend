using System.Net.WebSockets;
using System.Text.Json;
using Infrastructure;
using Domain;

namespace Application.Commands;

public class MakeMoveCommand : ICommand
{
    public string Type => "MakeMove";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket, JsonElement data)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        if (!lobby.IsGameStarted)
            throw new InvalidOperationException("Game is not started");
        if (lobby.Situation.CurrentPlayer != nickname)
            throw new ArgumentException("This is not your move now!");

        manager.AddSocket(lobbyCode, nickname, socket);

        if (!data.TryGetProperty("moves", out var movesElement) || !movesElement.EnumerateArray().Any())
            throw new ArgumentException("Moves array is missing or empty");

        var moveDtos = movesElement.Deserialize<MoveDto[]>()
                       ?? throw new ArgumentException("Invalid moves format");

        var moves = moveDtos
            .Select(m => (Q: m.q, R: m.r, S: m.s, Power: m.power, Owner: m.owner, Size: m.size))
            .ToArray();

        lobby.Situation.ValidateMove(nickname, moves);

        var losers = lobby.Situation.CheckForLose();
        if (losers.Count > 0)
            await manager.BroadcastAsync(lobbyCode, new { type = "PlayerLost", loser = losers.First() });

        var winner = lobby.Situation.CheckForWinner();
        if (winner != null)
            await manager.BroadcastAsync(lobbyCode, new { type = "GameEnd", winner });

        await manager.BroadcastAsync(lobbyCode,
            new
            {
                type = "MoveMade",
                nickname,
                moves = moveDtos,
                message = "Correct move"
            });
    }
}

internal record MoveDto(int q, int r, int s, int power, string owner, bool size);