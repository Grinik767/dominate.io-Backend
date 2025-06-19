using System.ComponentModel.DataAnnotations;

namespace Api.Contracts;

public class CreateLobbyRequest
{
    [Range(2, 4)] public int PlayersCount { get; set; }
    [Required] public required (int q, int r, int s, int power, string owner, bool size)[] Field { get; set; }
}