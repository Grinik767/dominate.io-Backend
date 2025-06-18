using System.ComponentModel.DataAnnotations;

namespace Api.Contracts;

public class CreateLobbyRequest
{
    [Range(2, 4)] public int PlayersCount { get; set; }
}