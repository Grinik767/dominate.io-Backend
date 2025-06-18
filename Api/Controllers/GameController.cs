using Api.Contracts;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Application.Commands;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(Storage storage, ConnectionManager manager, CommandDispatcher dispatcher) : ControllerBase
{
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
            var buffer = new byte[4096];

            var lobby = storage.GetLobby(r.Code);

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;
                
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var doc = JsonDocument.Parse(msg);
                var type = doc.RootElement.GetProperty("Type").GetString()!;

                var command = dispatcher.GetCommand(type);
                if (command is not null)
                    await command.ExecuteAsync(lobby, r.Code, r.Nickname, manager, socket);
            }
        }
        catch (Exception ex)
        {
            if (socket is { State: WebSocketState.Open })
            {
                await SendErrorAsync(socket, "ProcessingError", ex.Message);
                await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Closed by server",
                    CancellationToken.None);
            }
        }
        
        manager.RemoveSocket(r.Code, r.Nickname);
    }

    private static Task SendErrorAsync(WebSocket socket, string errorType, string message)
    {
        var errorPayload = JsonSerializer.Serialize(new { type = "Error", error = errorType, message });
        var bytes = Encoding.UTF8.GetBytes(errorPayload);
        return socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}