using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Infrastructure;

public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _sockets = new();

    public void AddSocket(string lobbyCode, string player, WebSocket socket)
    {
        var dict = _sockets.GetOrAdd(lobbyCode, _ => new ConcurrentDictionary<string, WebSocket>());
        
        if (dict.TryGetValue(player, out var value) && value != socket)
            throw new InvalidOperationException($"Player {player} is already connected");
        
        dict[player] = socket;
    }

    public void RemoveSocket(string lobbyCode, string player)
    {
        if (_sockets.TryGetValue(lobbyCode, out var dict))
            dict.TryRemove(player, out _);
    }

    public async Task BroadcastAsync(string lobbyCode, object message)
    {
        if (!_sockets.TryGetValue(lobbyCode, out var dict)) return;
        var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var tasks = dict.Values
            .Where(ws => ws.State == WebSocketState.Open)
            .Select(ws => ws.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None));
        await Task.WhenAll(tasks);
    }

    public Task SendToPlayerAsync(string lobbyCode, string player, object message)
    {
        if (!_sockets.TryGetValue(lobbyCode, out var dict) || !dict.TryGetValue(player, out var ws) ||
            ws.State != WebSocketState.Open)
            return Task.CompletedTask;

        var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        return ws.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}