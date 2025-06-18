using System.ComponentModel.DataAnnotations;

namespace Api.Contracts;

public class ConnectRequest
{
    [RegularExpression("^[A-Z0-9]{6}$")] public string Code { get; set; }

    [RegularExpression("^[a-zA-Z0-9_-]{1,20}$")]
    public string Nickname { get; set; }
}