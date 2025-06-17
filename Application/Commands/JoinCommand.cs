using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class JoinCommand : ICommand
{
    public string Type => "Leave";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket? socket)
    {
        var nicknameLower = nickname.ToLower();
        if (string.IsNullOrEmpty(nicknameLower))
            throw new InvalidOperationException("Nickname is empty");

        if (socket is null)
            throw new InvalidOperationException("Socket is null");

        lobby.AddPlayer(nicknameLower);

        manager.AddSocket(lobbyCode, nicknameLower, socket);
        await manager.BroadcastAsync(lobbyCode, new { Type = "PlayerJoined", Nickname = nickname });
    }
}