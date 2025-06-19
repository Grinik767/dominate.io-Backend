using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace Domain;

public class State
{
    public string CurrentPlayer { get; private set; }
    public Phase CurrentPhase { get; private set; }
    public List<string> PlayerQueue { get; private set; }
    private readonly ConcurrentDictionary<(int q, int r, int s), HexCell> _field = new();
    private readonly ConcurrentDictionary<string, int> _playersHexCount = new();

    public State((int q, int r, int s, int power, string owner, bool size)[] field)
    {
        foreach (var hex in field)
            _field[(hex.q, hex.r, hex.s)] = new HexCell(hex.q, hex.r, hex.s, hex.power, hex.owner, hex.size);
    }

    public void StartGame(List<string> players)
    {
        if (players.Count < 1)
            throw new ArgumentException("Lobby is empty");
        
        var rnd = new Random();
        PlayerQueue = players.OrderBy(_ => rnd.Next()).ToList();
        CurrentPlayer = PlayerQueue[0];
        CurrentPhase = Phase.Attack;

        foreach (var player in players)
            _playersHexCount[player] = 1;
    }

    public void ChangePhase() => CurrentPhase = Phase.Upgrade;

    public void PassTheMove()
    {
        if (PlayerQueue.Count == 0)
            throw new InvalidOperationException("No players in queue");

        var currentIndex = PlayerQueue.IndexOf(CurrentPlayer);
        if (currentIndex == -1)
            throw new InvalidOperationException("Current player not found in queue");

        var nextIndex = (currentIndex + 1) % PlayerQueue.Count;
        CurrentPlayer = PlayerQueue[nextIndex];
        CurrentPhase = Phase.Attack;
    }

    public void ValidateMove(string nickname, (int q, int r, int s, int power, string owner, bool size)[] moves)
    {
        if (CurrentPlayer != nickname)
            throw new InvalidOperationException($"This is not {nickname} move!");

        foreach (var move in moves)
        {
            var prevHexValues = _field[(move.q, move.r, move.s)];
            if (prevHexValues.Owner != move.owner && !string.IsNullOrEmpty(prevHexValues.Owner))
            {
                _playersHexCount[prevHexValues.Owner!] -= 1;
                _playersHexCount[move.owner] += 1;
            }
            else if (string.IsNullOrEmpty(prevHexValues.Owner)) 
                _playersHexCount[move.owner] += 1;
            
            _field[(move.q, move.r, move.s)].UpdateHex(move.power, move.owner);
        }
    }

    public List<string> CheckForLose()
    {
        var losePlayers = new List<string>();
        foreach (var player in _playersHexCount.Keys)
            if (_playersHexCount[player] < 1)
            {
                PlayerQueue.Remove(player);
                losePlayers.Add(player); 
            }

        return losePlayers;
    }
    
    public string? CheckForWinner()
    {
        var activePlayers = PlayerQueue
            .Where(p => _playersHexCount[p] > 0)
            .ToList();

        return activePlayers.Count == 1 ? activePlayers[0] : null;
    }

    public (int q, int r, int s, int power, string owner, bool size)[] GetField() =>
        _field.Values
            .Select(cell => cell.ToTuple())
            .ToArray();
}