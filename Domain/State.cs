using System.Collections.Concurrent;

namespace Domain;

public class State
{
    public string CurrentPlayer { get; private set; }
    public Phase CurrentPhase { get; private set; }
    public List<string> PlayerQueue { get; private set; }
    private readonly ConcurrentDictionary<(int q, int r, int s), HexCell> _field = new();
    public readonly ConcurrentDictionary<string, int> _playersHexCount = new();
    private readonly ConcurrentDictionary<int, (int q, int r, int s)> _playerPositions = new();

    public State((int q, int r, int s, int power, int owner, bool size)[] field)
    {
        foreach (var hex in field)
        {
            if (hex.owner != -1)
                _playerPositions[hex.owner] = (hex.q, hex.r, hex.s);
            _field[(hex.q, hex.r, hex.s)] = new HexCell(hex.q, hex.r, hex.s, hex.power, null, hex.size);
        }
            
    }

    public void StartGame(List<string> players)
    {
        if (players.Count < 1)
            throw new ArgumentException("Lobby is empty");
        if (players.Count != _playerPositions.Count)
            throw new InvalidOperationException("The count of players and started fields is not similar"); 

        var rnd = new Random();
        PlayerQueue = players.OrderBy(_ => rnd.Next()).ToList();
        CurrentPlayer = PlayerQueue[0];
        CurrentPhase = Phase.Attack;

        for (var i = 0; i < PlayerQueue.Count; i++)
            if (_playerPositions.TryGetValue(i, out var hex))
                _field[(hex.q, hex.r, hex.s)].UpdateHex(PlayerQueue[i]);

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

    public void RemovePlayer(string nickname)
    {
        if (nickname == CurrentPlayer)
            PassTheMove();

        _playersHexCount.TryRemove(nickname, out _);
        RemovePlayerFromField(nickname);

        PlayerQueue.Remove(nickname);
    }

    private void RemovePlayerFromField(string nickname)
    {
        foreach (var (coord, cell) in _field)
            if (cell.Owner == nickname)
                cell.UpdateHex(cell.Power, null);
    }
}