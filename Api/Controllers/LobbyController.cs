using Api.Contracts;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LobbyController(Storage storage, CodeGenerator codeGenerator) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateLobbyRequest r)
    {
        var code = codeGenerator.GenerateCode(storage.GetAllCodes());
        var field = r.Field
            .Select(x => (x.Q, x.R, x.S, x.Power, x.Owner, x.Size))
            .ToArray();
        storage.AddNewLobby(code, r.PlayersCount, field);
        return Ok(new { Code = code });
    }

    [HttpGet("{code}/{nickname}")]
    public IActionResult IsLobbyExist(string code, string nickname)
    {
        var isExist = storage.IsLobbyExist(code);
        if (!isExist)
            return Ok(new
            {
                IsExist = isExist,
                HasFreeSpace = false,
                CanJoin = false
            });

        var lobby = storage.GetLobby(code);
        var hasFreeSpace = !lobby.IsLobbyFull();

        return Ok(new
        {
            IsExist = storage.IsLobbyExist(code),
            HasFreeSpace = hasFreeSpace,
            CanJoin = hasFreeSpace && !lobby.IsContainsPlayer(nickname)
        });
    }
}