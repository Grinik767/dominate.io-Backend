using System.Collections.Concurrent;

namespace Domain;

public class Lobby
{
    public readonly State Situation = new();
    public readonly Player Leader;
    public DateTime LastAccess = DateTime.Now;

    private readonly ConcurrentDictionary<Guid, Player> _data = new();

    public Lobby(string leaderNickname)
    {
        Leader = new Player(leaderNickname, GetFreeColor(), Guid.NewGuid());
        _data[Leader.Id] = Leader;
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