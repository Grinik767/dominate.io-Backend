using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LobbyController(Storage storage, CodeGenerator codeGenerator) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] LobbyCreateRequest req)
    {
        var code = codeGenerator.GenerateCode(storage.GetAllCodes());
        var lobbyId = storage.AddNewLobby(code, req.LeaderNickname);

        return Ok(new { Code = code, SessionId = lobbyId });
    }
}

public record LobbyCreateRequest(string LeaderNickname);