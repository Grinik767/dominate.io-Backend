using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class LeaveCommand : ICommand
{
    public string Type => "Leave";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket)
    {
        if (!lobby.TryRemovePlayer(nickname))
            return;
        
        await manager.BroadcastAsync(lobbyCode, new { Type = "PlayerLeft", Nickname = nickname });
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
    }
}