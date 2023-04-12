namespace BackendDAL.Entities;

public class Cook
{
    public Guid Id { get; set; } //TODO: имена курьеров и поваров
    public Restaurant Restaurant { get; set; }
    public ICollection<Order> Orders { get; set; }
}