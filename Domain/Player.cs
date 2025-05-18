namespace Domain;

public class Player(string nickname, Color color, Guid id)
{
    public readonly string Nickname = nickname;
    public readonly Color Color = color;
    public readonly Guid Id = id;
    public bool IsReady;
    public DateTime LastAccess = DateTime.Now;
}