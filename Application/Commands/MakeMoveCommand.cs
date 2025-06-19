using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Infrastructure;
using Domain;

namespace Application.Commands;

public class MakeMoveCommand : ICommand
{
    public string Type => "MakeMove";
    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        if (!lobby.IsGameStarted)
            throw new InvalidOperationException("Game is not started");
        if (lobby.Situation.CurrentPlayer != nickname)
            throw new ArgumentException("This is not your move now!");
        
        try
        {
            var buffer = new byte[4096];
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType != WebSocketMessageType.Text)
                throw new InvalidDataException("Expected text message with moves");

            var msg = Encoding.UTF8.GetString(buffer[..result.Count]);
            var doc = JsonDocument.Parse(msg);
            var root = doc.RootElement;

            if (!root.TryGetProperty("Moves", out var movesElement) || !movesElement.EnumerateArray().Any())
                throw new ArgumentException("Moves array is missing or empty");

            var moveDtos = movesElement.Deserialize<MoveDto[]>() 
                           ?? throw new ArgumentException("Invalid moves format");

            var moves = moveDtos
                .Select(m => (m.Q, m.R, m.S, m.Power, m.Owner, m.Size))
                .ToArray();
            
            lobby.Situation.ValidateMove(nickname, moves);
            
            var losers = lobby.Situation.CheckForLose();
            if (losers.Count > 0) 
                await manager.BroadcastAsync(lobbyCode, new { Type = "PlayerLost", Loser = losers.First() });
            
            var winner = lobby.Situation.CheckForWinner();
            if (winner != null)
                await manager.BroadcastAsync(lobbyCode, new { Type = "GameEnd", Winner = winner });

            await manager.SendToPlayerAsync(lobbyCode, nickname, new { Type = "MakeMove", Message = "Correct move" });
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
    }
    
    private static Task SendErrorAsync(WebSocket socket, string errorType, string message)
    {
        var errorPayload = JsonSerializer.Serialize(new { type = "Error", error = errorType, message });
        var bytes = Encoding.UTF8.GetBytes(errorPayload);
        return socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

internal record MoveDto(int Q, int R, int S, int Power, string Owner, bool Size);