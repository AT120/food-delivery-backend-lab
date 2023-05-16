namespace AuthDAL.Models;

public class Cook
{
    public Guid BaseUserId { get; set; }
    public User BaseUser { get; set; }
}