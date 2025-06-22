using System.Net.WebSockets;
using System.Text.Json;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class LeaveCommand : ICommand
{
    public string Type => "Leave";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket, JsonElement data)
    {
        if (!lobby.TryRemovePlayer(nickname))
            return;
        
        if (lobby.IsGameStarted)
            lobby.Situation.RemovePlayer(nickname);

        object message;
        if (lobby.IsGameStarted)
            message = new { type = "PlayerLeft", nickname, nextPlayer = lobby.Situation.CurrentPlayer };
        else
            message = new { type = "PlayerLeft", nickname };

        await manager.BroadcastAsync(lobbyCode, message);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
    }
}