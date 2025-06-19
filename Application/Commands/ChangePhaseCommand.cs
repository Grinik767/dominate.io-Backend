using System.Net.WebSockets;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class ChangePhaseCommand : ICommand
{
    public string Type => "PhaseEnd"; 
    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager, WebSocket socket)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        if (!lobby.IsGameStarted)
            throw new InvalidOperationException("Game is not started");
        if (lobby.Situation.CurrentPhase != Phase.Attack)
            throw new ArgumentException("This is not Attack phase now");
        if (lobby.Situation.CurrentPlayer != nickname)
            throw new ArgumentException("This is not your move now!");
        
        manager.AddSocket(lobbyCode, nickname, socket);
        
        lobby.Situation.ChangePhase();
        await manager.BroadcastAsync(lobbyCode,
            new { Type = "PhaseEnd", Message = $"Player {nickname} end attack phase, upgrade now" });
    }
}