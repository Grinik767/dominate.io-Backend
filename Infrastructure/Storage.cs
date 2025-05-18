using System.Collections.Concurrent;
using Domain;

namespace Infrastructure;

public class Storage
{
    private readonly ConcurrentDictionary<string, Lobby> _data = new();

    public HashSet<string> GetAllCodes() => _data.Keys.ToHashSet();

    public Guid AddNewLobby(string code, string leaderNickname)
    {
        if (!_data.TryAdd(code, new Lobby(leaderNickname)))
            throw new InvalidOperationException($"Lobby '{code}' already exists");

        return _data[code].Leader.Id;
    }
}