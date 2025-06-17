using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(Storage storage, ConnectionManager manager, IConnectionService connectionService)
    : ControllerBase
{
    private readonly Dictionary<string, ICommand> _commands = new()
    {
        ["Join"] = new JoinCommand(),
        ["Leave"] = new LeaveCommand()
    };

    [HttpGet]
    public async Task Connect([FromQuery] ConnectRequest r)
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
            socket = await context.WebSockets.AcceptWebSocketAsync();
            var lobby = storage.GetLobby(r.Code);

            var buffer = new byte[4096];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var doc = JsonDocument.Parse(msg);
                var type = doc.RootElement.GetProperty("type").GetString()!;
                
                if (type == "Auth")
                {
                    if (string.IsNullOrEmpty(r.Nickname))
                        throw new InvalidOperationException("Никнейм обязателен");

                    manager.AddSocket(r.Code, r.Nickname, socket);
                    continue;
                }
                
                if (_commands.TryGetValue(type, out var command))
                    await command.ExecuteAsync(lobby, r.Code, r.Nickname, connectionService, CancellationToken.None);
            }
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

public class ConnectRequest
{
    [RegularExpression("^[A-Z0-9]+$")]
    public string Code { get; set; }

    [RegularExpression("^[a-zA-Z0-9_-]+$")]
    public string Nickname { get; set; }
}