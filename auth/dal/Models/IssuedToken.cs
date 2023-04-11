namespace AuthDAL.Models;

public class IssuedToken
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ValidUntil { get; set; }
}