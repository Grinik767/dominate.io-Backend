using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application;

public class JoinCommand(ConnectionManager manager) : ICommand
{
    public string Type => "Leave";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, IConnectionService connectionService,
        CancellationToken ct, WebSocket? socket)
    {
        var nicknameLower = nickname.ToLower();
        if (string.IsNullOrEmpty(nicknameLower))
            throw new InvalidOperationException("Nickname is empty");
        
        if (socket is null)
            throw new InvalidOperationException("Socket is null");
        
        lobby.AddPlayer(nicknameLower);

        manager.AddSocket(lobbyCode, nicknameLower, socket);
        await connectionService.BroadcastAsync(lobbyCode, new { Type = "PlayerJoined", Nickname = nickname });
    }
}