using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class SwitchReadinessCommand : ICommand
{
    public string Type => "SwitchReadiness";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        if (lobby.IsGameStarted)
            throw new InvalidOperationException("Game is started");

        var player = lobby.GetPlayer(nickname)!;
        player.SwitchReadiness();
        await manager.BroadcastAsync(lobbyCode, new { Type = "Readiness", player.Nickname, player.IsReady });

        lobby.CheckGameStart();
        if (lobby.IsGameStarted)
            await manager.BroadcastAsync(lobbyCode, new { Type = "GameStarted" });
    }
}