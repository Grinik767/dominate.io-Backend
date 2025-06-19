using System.Net.WebSockets;
using System.Text.Json;
using Domain;
using Infrastructure;

namespace Application.Commands;

public interface ICommand
{
    string Type { get; }
    Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager, WebSocket socket, JsonElement data);
}