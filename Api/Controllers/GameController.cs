using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(Storage storage, ConnectionManager manager) : ControllerBase
{
    [HttpGet("connect/{code}/{nickname}")]
    public async Task Connect(string code, string nickname)
    {
        var context = HttpContext;
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        WebSocket? socket = null;
        try
        {
            var lobby = storage.GetLobby(code);

            socket = await context.WebSockets.AcceptWebSocketAsync();
            manager.AddSocket(code, nickname, socket);

            var buffer = new byte[4096];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var type = JsonDocument.Parse(msg).RootElement.GetProperty("type").GetString();
                switch (type)
                {
                    case "Join":
                        lobby.AddPlayer(nickname);
                        await manager.BroadcastAsync(code, new { Type = "PlayerJoined", Nickname = nickname });
                        break;
                    case "Leave":
                        if (lobby.TryRemovePlayer(nickname))
                            await manager.BroadcastAsync(code, new { Type = "PlayerLeft", Nickname = nickname });
                        goto Done;
                }
            }

            Done: ;
        }
        catch (Exception ex)
        {
            if (socket is { State: WebSocketState.Open })
                await SendErrorAsync(socket, "ProcessingError", ex.Message);
        }
        finally
        {
            if (socket is { State: WebSocketState.Open })
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
        }
    }

    private static Task SendErrorAsync(WebSocket socket, string errorType, string message)
    {
        var errorPayload = JsonSerializer.Serialize(new { type = "Error", error = errorType, message });
        var bytes = Encoding.UTF8.GetBytes(errorPayload);
        return socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}