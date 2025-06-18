using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class JoinCommand : ICommand
{
    public string Type => "Join";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket)
    {
        if (string.IsNullOrEmpty(nickname))
            throw new InvalidOperationException("Nickname is empty");

        if (socket is null)
            throw new InvalidOperationException("Socket is null");

        lobby.AddPlayer(nickname);

        manager.AddSocket(lobbyCode, nickname, socket);
        await manager.BroadcastAsync(lobbyCode, new
        {
            Type = "PlayerJoined", Nickname = nickname,
            Color = lobby.GetPlayer(nickname)!.Color.ToString()
        });
    }
}