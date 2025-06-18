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
        storage.AddNewLobby(code, r.PlayersCount);
        return Ok(new { Code = code });
    }
}