using System.Net.WebSockets;
using Infrastructure;

namespace Application;

public class ConnectionService : IConnectionService
{
    private readonly ConnectionManager _manager;

    public ConnectionService(ConnectionManager manager) 
        => _manager = manager;

    public void AddSocket(string lobbyCode, string player, WebSocket socket) 
        => _manager.AddSocket(lobbyCode, player, socket);

    public void RemoveSocket(string lobbyCode, string player) 
        => _manager.RemoveSocket(lobbyCode, player);

    public Task BroadcastAsync(string lobbyCode, object message) 
        => _manager.BroadcastAsync(lobbyCode, message);

    public Task SendToPlayerAsync(string lobbyCode, string player, object message) 
        => _manager.SendToPlayerAsync(lobbyCode, player, message);
}