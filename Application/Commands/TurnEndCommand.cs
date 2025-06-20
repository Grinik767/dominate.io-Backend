using System.Net.WebSockets;
using System.Text.Json;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class TurnEndCommand : ICommand
{
    public string Type => "TurnEnd";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket, JsonElement data)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        if (!lobby.IsGameStarted)
            throw new InvalidOperationException("Game is not started");
        if (lobby.Situation.CurrentPhase != Phase.Upgrade)
            throw new ArgumentException("This is not Upgrade phase now, you can not finish your turn");
        if (lobby.Situation.CurrentPlayer != nickname)
            throw new ArgumentException("This is not your move now!");

        manager.AddSocket(lobbyCode, nickname, socket);

        lobby.Situation.PassTheMove();
        await manager.BroadcastAsync(lobbyCode,
            new
            {
                Type = "TurnEnd",
                Nickname = nickname,
                NextPlayer = lobby.Situation.CurrentPlayer,
                Message = $"Player {nickname} end his(her) turn, next is {lobby.Situation.CurrentPlayer}"
            });
    }
}