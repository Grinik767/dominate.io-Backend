﻿using System.Collections.Concurrent;

namespace Domain;

public class Lobby(int playersCount, (int q, int r, int s, int power, int owner, bool size)[] startField)
{
    public readonly State Situation = new(startField);
    private readonly ConcurrentDictionary<string, Player> _data = new();

    public DateTime LastAccess { get; private set; }
    public bool IsGameStarted { get; private set; }

    public void AddPlayer(string nickname)
    {
        LastAccess = DateTime.Now;

        if (_data.Count == playersCount)
            throw new InvalidOperationException("Lobby is full");
        if (IsContainsPlayer(nickname))
            throw new InvalidOperationException("Nickname already taken");

        _data[nickname] = new Player(nickname, GetFreeColor());
    }

    public bool TryRemovePlayer(string nickname)
    {
        LastAccess = DateTime.Now;
        return _data.TryRemove(nickname, out _);
    }

    public void CheckGameStart() => IsGameStarted = _data.Values.Count(player => player.IsReady) == playersCount;

    public void StartGame()
    {
        var players = _data.Keys.ToList();
        Situation.StartGame(players);
        LastAccess = DateTime.Now;
    }
    
    public bool IsLobbyFull() => _data.Count == playersCount; 

    public bool IsContainsPlayer(string nickname) => _data.ContainsKey(nickname);

    public Dictionary<string, object> GetPlayers()
    {
        return _data.Keys.Select(x => (Nickname: x, Info: _data[x]))
            .ToDictionary<(string Nickname, Player Info), string, object>(x => x.Nickname,
                x => new { Color = x.Info.Color.ToString(), x.Info.IsReady });
    }

    public Player? GetPlayer(string nickname) => _data.GetValueOrDefault(nickname);

    private Color GetFreeColor()
    {
        var allColors = Enum.GetValues(typeof(Color)).Cast<Color>().ToHashSet();
        allColors.ExceptWith(_data.Values.Select(player => player.Color).ToHashSet());

        if (allColors.Count == 0)
            throw new InvalidOperationException("No free color available");

        return allColors.First();
    }
}