using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class GetPlayersCommand : ICommand
{
    public string Type => "GetPlayers";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        
        manager.AddSocket(lobbyCode, nickname, socket);
        
        var players = lobby.GetPlayers();
        await manager.SendToPlayerAsync(lobbyCode, nickname, new { Type = "SendPlayers", Players = players });
    }
}