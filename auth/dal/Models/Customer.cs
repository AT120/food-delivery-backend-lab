namespace AuthDAL.Models;

public class Customer
{
    public Guid BaseUserId { get; set; }
    public string? Address { get; set; }
    public User BaseUser { get; set; }
}