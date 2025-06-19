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

    public void UpdateHex(int playersPower, string nickname)
    {
        Power = playersPower;
        if (Owner != nickname)
            Owner = nickname;
    }
    
    public (int q, int r, int s, int power, string owner, bool size) ToTuple() 
        => (Q, R, S, Power, Owner ?? string.Empty, Size);
}