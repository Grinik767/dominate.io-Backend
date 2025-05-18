namespace Domain;

public class Player(string nickname, Color color)
{
    public readonly string Nickname = nickname;
    public readonly Color Color = color;
    public bool IsReady;
    public DateTime LastAccess = DateTime.Now;
}