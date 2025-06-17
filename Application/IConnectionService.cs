using System.Net.WebSockets;

namespace Application;

public interface IConnectionService
{
    Task BroadcastAsync(string lobbyCode, object message);
    Task SendToPlayerAsync(string lobbyCode, string player, object message);
    void AddSocket(string lobbyCode, string player, WebSocket socket);
    void RemoveSocket(string lobbyCode, string player);
}