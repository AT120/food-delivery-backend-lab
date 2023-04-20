using System.Net;
using BackendCommon.Enums;

namespace BackendCommon.DTO;

public class CustomerOrderShort
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public int Price { get; set; }
    // public string Address { get; set; }
    public OrderStatus Status { get; set; }
    // public Guid? CourierId { get; set; }
    // public ICollection<DishShort> Dishes { get; set; }
}
