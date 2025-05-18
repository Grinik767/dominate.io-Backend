using System.Security.Cryptography;
using System.Text;

namespace Infrastructure;

public class CodeGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    private readonly int _codeLength;

    public CodeGenerator(int codeLength = 6)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(codeLength);
        _codeLength = codeLength;
    }

    public string GenerateCode(HashSet<string> issuedCodes)
    {
        while (true)
        {
            var code = GenerateRandomString(_codeLength);
            if (!issuedCodes.Contains(code))
                return code;
        }
    }

    private static string GenerateRandomString(int length)
    {
        var buffer = new byte[length];
        Rng.GetBytes(buffer);

        var result = new StringBuilder(length);
        foreach (var b in buffer)
            result.Append(Chars[b % Chars.Length]);

        return result.ToString();
    }
}