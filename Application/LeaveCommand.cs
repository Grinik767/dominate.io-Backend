using System.Net.WebSockets;
using Domain;

namespace Application;

public class LeaveCommand : ICommand
{
    public string Type => "Leave";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, IConnectionService connectionService,
        CancellationToken ct, WebSocket? socket)
    {
        if (lobby.TryRemovePlayer(nickname))
            await connectionService.BroadcastAsync(lobbyCode, new { Type = "PlayerLeft", Nickname = nickname });
    }
}