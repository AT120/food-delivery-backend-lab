namespace AuthDAL.Models;

public class Customer
{
    public Guid Id { get; set; }
    public string Address { get; set; }
    public User BaseUser { get; set; }
}