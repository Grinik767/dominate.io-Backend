namespace Domain;

public class Lobby(Player[] players)
{
    public readonly State Situation = new();
    public readonly Player[] Players = new Player[4];
    public readonly Player Leader = players[0];

    public DateTime LastAccess = DateTime.Now;
}