namespace BackendDAL.Entities;

public class Manager
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
}