using System.ComponentModel.DataAnnotations;

namespace Api.Contracts;

public class ConnectRequest
{
    [RegularExpression("^[A-Z0-9]{6}$")] public string Code { get; set; }

    [RegularExpression("^[a-zA-Z0-9_-]+$")]
    public string Nickname { get; set; }
}