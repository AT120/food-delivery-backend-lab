namespace AuthDAL.Models;

public class Courier
{
    public Guid BaseUserId { get; set; }
    public User BaseUser { get; set; }
}