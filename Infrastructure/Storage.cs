using System.Collections.Concurrent;
using Domain;

namespace Infrastructure;

public class Storage
{
    private readonly ConcurrentDictionary<string, Lobby> _data = new();

    public HashSet<string> GetAllCodes() => _data.Keys.ToHashSet();

    public void AddNewLobby(string code, int playersCount)
    {
        if (!_data.TryAdd(code, new Lobby(playersCount)))
            throw new InvalidOperationException($"Lobby '{code}' already exists");
    }

    public Lobby GetLobby(string code)
    {
        if (!_data.TryGetValue(code, out var lobby))
            throw new InvalidOperationException($"Lobby '{code}' does not exist");

        return lobby;
    }
}