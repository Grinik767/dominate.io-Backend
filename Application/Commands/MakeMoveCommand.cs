using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Infrastructure;
using Domain;

namespace Application.Commands;

public class MakeMoveCommand : ICommand
{
    public string Type => "MakeMove";
    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager, WebSocket socket, JsonElement data)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        if (!lobby.IsGameStarted)
            throw new InvalidOperationException("Game is not started");
        if (lobby.Situation.CurrentPlayer != nickname)
            throw new ArgumentException("This is not your move now!");
        
        manager.AddSocket(lobbyCode, nickname, socket);
        
        try
        {
            if (!data.TryGetProperty("Moves", out var movesElement) || !movesElement.EnumerateArray().Any())
                throw new ArgumentException("Moves array is missing or empty");

            var moveDtos = movesElement.Deserialize<MoveDto[]>() 
                           ?? throw new ArgumentException("Invalid moves format");

            var moves = moveDtos
                .Select(m => (Q: m.q, R: m.r, S: m.s, Power: m.power, Owner: m.owner, Size: m.size))
                .ToArray();

            lobby.Situation.ValidateMove(nickname, moves);

            var losers = lobby.Situation.CheckForLose();
            if (losers.Count > 0) 
                await manager.BroadcastAsync(lobbyCode, new { Type = "PlayerLost", Loser = losers.First() });
        
            var winner = lobby.Situation.CheckForWinner();
            if (winner != null)
                await manager.BroadcastAsync(lobbyCode, new { Type = "GameEnd", Winner = winner });

            await manager.SendToPlayerAsync(lobbyCode, nickname, new { Type = "MakeMove", Message = "Correct move" });
        }
        catch (Exception ex)
        {
            if (socket.State == WebSocketState.Open) 
                await SendErrorAsync(socket, "ProcessingError", ex.Message);
        }
    }
    
    private static Task SendErrorAsync(WebSocket socket, string errorType, string message)
    {
        var errorPayload = JsonSerializer.Serialize(new { type = "Error", error = errorType, message });
        var bytes = Encoding.UTF8.GetBytes(errorPayload);
        return socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

internal record MoveDto(int q, int r, int s, int power, string owner, bool size);