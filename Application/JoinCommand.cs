using Domain;

namespace Application;

public class JoinCommand : ICommand
{
    public string Type => "Leave";
    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, IConnectionService connectionService, CancellationToken ct)
    {
        lobby.AddPlayer(nickname);
        await connectionService.BroadcastAsync(lobbyCode, new { Type = "PlayerJoined", Nickname = nickname });
    }
}