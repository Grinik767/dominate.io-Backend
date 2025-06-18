using System.Collections.Concurrent;

namespace Domain;

public class Lobby
{
    public readonly State Situation = new();
    private readonly ConcurrentDictionary<string, Player> _data = new();

    public DateTime LastAccess { get; private set; }
    public Player? Leader { get; private set; }

    public void AddPlayer(string nickname)
    {
        LastAccess = DateTime.Now;

        if (_data.Count == 4)
            throw new InvalidOperationException("Lobby is full");
        if (IsContainsPlayer(nickname))
            throw new InvalidOperationException("Nickname already taken");

        _data[nickname] = new Player(nickname, GetFreeColor());

        if (_data.Count == 1)
            Leader = _data[nickname];
    }

    public bool TryRemovePlayer(string nickname)
    {
        LastAccess = DateTime.Now;
        return _data.TryRemove(nickname, out _);
    }

    public bool IsContainsPlayer(string nickname) => _data.ContainsKey(nickname);

    public Dictionary<string, string> GetPlayersColor()
    {
        return _data.Keys.Select(x => (Nickname: x, _data[x].Color))
            .ToDictionary(x => x.Nickname, x => x.Color.ToString());
    }

    public Player? GetPlayer(string nickname) =>_data.GetValueOrDefault(nickname);

    private Color GetFreeColor()
    {
        var allColors = Enum.GetValues(typeof(Color)).Cast<Color>().ToHashSet();
        allColors.ExceptWith(_data.Values.Select(player => player.Color).ToHashSet());

        if (allColors.Count == 0)
            throw new InvalidOperationException("No free color available");

        return allColors.First();
    }
}