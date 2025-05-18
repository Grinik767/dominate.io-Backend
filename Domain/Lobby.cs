using System.Collections.Concurrent;

namespace Domain;

public class Lobby
{
    public readonly State Situation = new();
    public DateTime LastAccess = DateTime.Now;
    private readonly ConcurrentDictionary<string, Player> _data = new();

    public Player? Leader { get; private set; }

    public void AddPlayer(string nickname)
    {
        LastAccess = DateTime.Now;

        if (_data.Count == 4)
            throw new InvalidOperationException("Lobby is full");
        if (_data.ContainsKey(nickname))
            throw new InvalidOperationException("Nickname already taken");

        _data[nickname] = new Player(nickname, GetFreeColor());

        if (_data.Count == 1)
            Leader = _data[nickname];
    }

    private Color GetFreeColor()
    {
        var allColors = Enum.GetValues(typeof(Color)).Cast<Color>().ToHashSet();
        allColors.ExceptWith(_data.Values.Select(player => player.Color).ToHashSet());

        if (allColors.Count == 0)
            throw new InvalidOperationException("No free color available");

        return allColors.First();
    }
}