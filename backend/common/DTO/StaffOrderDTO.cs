using BackendCommon.Enums;

namespace BackendCommon.DTO;

public class StaffOrderDTO
{
    public int Id { get; set; }
    public Guid? CustomerId { get; set; }
    // public string? Address { get; set; }
    public OrderStatus Status { get; set; }
    public Guid? CookId { get; set; }
    public Guid? CourierId { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime DeliveryTime { get; set; }
    public ICollection<DishShort> Dishes { get; set; }
}