namespace Domain;

public class HexCell
{
    public readonly int Q;
    public readonly int R;
    public readonly int S;
    public readonly bool Size;
    
    public int Power { get; set; }
    public string? Owner { get; private set; }
    private static readonly ThreadLocal<Random> Random = new(() => new Random());

    public HexCell(int q, int r, int s, int power, string owner, bool size)
    {
        Q = q;
        R = r;
        S = s;

        Power = power;
        Owner = owner;
        Size = size;
    }

    public bool PlayerMove(int playersPower, string nickname)
    {
        if (string.IsNullOrEmpty(Owner))
        {
            ChangeOwner(playersPower, nickname);
            return true;
        }
        else if (Owner == nickname)
        {
            Power += playersPower;
            return true;
        }
        else
            return AttemptedCapture(playersPower, nickname);
    }

    private bool AttemptedCapture(int playersPower, string nickname)
    {
        var captureChance = Power - playersPower switch
        {
            <= -2 => 0.0,   
            -1 => 0.25,     
            0 => 0.5,
            1 => 0.75,
            >= 2 => 1.0
        };

        var newPower = Math.Max(1, int.Abs(Power - playersPower));
        if (Random.Value!.NextDouble() <= captureChance)
        {
            ChangeOwner(newPower, nickname);
            return true;
        }

        Power = newPower;
        return false;
    }

    private void ChangeOwner(int playersPower, string nickname)
    {
        Power = playersPower;
        Owner = nickname;
    }
}