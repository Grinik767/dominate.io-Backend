using System.Net.WebSockets;
using Domain;

namespace Application;

public interface ICommand
{
    string Type { get; }

    Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, IConnectionService connectionService,
        CancellationToken ct, WebSocket? webSocket = null);
}