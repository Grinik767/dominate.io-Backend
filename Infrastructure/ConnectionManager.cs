using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Infrastructure;


public class ConnectionManager
{  
    private readonly ConcurrentDictionary<string, ConcurrentBag<WebSocket>> _sockets = new();

    public void AddSocket(string lobbyCode, WebSocket socket)
    {
        var bag = _sockets.GetOrAdd(lobbyCode, _ => []);
        bag.Add(socket);
    }

    /*
    public void RemoveSocket(string lobbyCode, WebSocket socket)
    {
    }
    
    public async Task BroadcastAsync(string lobbyCode, object message)
    {
        if (!_sockets.TryGetValue(lobbyCode, out var bag)) return;
        var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var tasks = bag
            .Where(ws => ws.State == WebSocketState.Open)
            .Select(ws => ws.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None));
        await Task.WhenAll(tasks);
    }
    */
}
