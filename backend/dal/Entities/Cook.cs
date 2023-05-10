namespace BackendDAL.Entities;

public class Cook
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
    public ICollection<Order>? Orders { get; set; }
}