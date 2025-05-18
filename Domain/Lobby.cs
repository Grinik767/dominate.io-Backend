using System.Collections.Concurrent;
using System.Data;

namespace Domain;

public class Lobby
{
    public readonly State Situation = new();
    public readonly Player[] Players = new Player[4];
    public readonly Player Leader;
    public DateTime LastAccess = DateTime.Now;

    private readonly ConcurrentDictionary<Guid, Player> _data = new();

    public Lobby(string leaderNickname)
    {
        var leader = new Player(leaderNickname, GetFreeColor());
        
        _data[Guid.NewGuid()] = leader;
        Leader = leader;
    }

    private Color GetFreeColor()
    {
        var allColors = Enum.GetValues(typeof(Color)).Cast<Color>().ToHashSet();
        allColors.ExceptWith(_data.Values.Select(player => player.Color).ToHashSet());

        if (allColors.Count == 0)
            throw new DataException("No free color available");

        return allColors.First();
    }
}