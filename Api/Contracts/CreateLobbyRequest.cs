using System.ComponentModel.DataAnnotations;
using Api.DTO;

namespace Api.Contracts;

public class CreateLobbyRequest
{
    [Range(2, 4)] public int PlayersCount { get; set; }
    [Required] public required HexCellDto[] Field { get; set; }
}