namespace BackendDAL.Entities;

public class Courier
{
    public Guid Id { get; set; }
    public ICollection<Order>? Orders { get; set; }
}