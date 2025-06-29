﻿using System.Net.WebSockets;
using System.Text.Json;
using Domain;
using Infrastructure;

namespace Application.Commands;

public class SwitchReadinessCommand : ICommand
{
    public string Type => "SwitchReadiness";

    public async Task ExecuteAsync(Lobby lobby, string lobbyCode, string nickname, ConnectionManager manager,
        WebSocket socket, JsonElement data)
    {
        if (!lobby.IsContainsPlayer(nickname))
            throw new InvalidOperationException("Player is not in lobby");
        if (lobby.IsGameStarted)
            throw new InvalidOperationException("Game is started");

        manager.AddSocket(lobbyCode, nickname, socket);

        var player = lobby.GetPlayer(nickname)!;
        player.SwitchReadiness();
        await manager.BroadcastAsync(lobbyCode,
            new { type = "Readiness", nickname = player.Nickname, isReady = player.IsReady });

        lobby.CheckGameStart();
        if (lobby.IsGameStarted)
        {
            lobby.StartGame();
            var field = lobby.Situation.GetField()
                .Select(cell => new
                {
                    cell.q,
                    cell.r,
                    cell.s,
                    cell.power,
                    cell.owner,
                    cell.size
                })
                .ToArray();
            var playersQueue = lobby.Situation.PlayerQueue.ToArray();
            await manager.BroadcastAsync(lobbyCode, new { type = "GameStarted", playersQueue, field });
        }
    }
}