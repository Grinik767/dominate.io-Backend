using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class LeaveCommand : ICommand
{
    public string Type => "Leave";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket? socket)
    {
        var nicknameLower = nickname.ToLower();

        if (lobby.TryRemovePlayer(nicknameLower))
            await manager.BroadcastAsync(lobbyCode, new { Type = "PlayerLeft", Nickname = nickname });
        
        
    }
}