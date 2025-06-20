using System.Net.WebSockets;
using System.Text.Json;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class JoinCommand : ICommand
{
    public string Type => "Join";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket, JsonElement data)
    {
        if (string.IsNullOrEmpty(nickname))
            throw new InvalidOperationException("Nickname is empty");
        if (socket is null)
            throw new InvalidOperationException("Socket is null");
        if (lobby.IsGameStarted)
            throw new InvalidOperationException("Game is started");

        lobby.AddPlayer(nickname);

        manager.AddSocket(lobbyCode, nickname, socket);
        await manager.BroadcastAsync(lobbyCode, new
        {
            type = "PlayerJoined",
            nickname,
            color = lobby.GetPlayer(nickname)!.Color.ToString()
        });
    }
}