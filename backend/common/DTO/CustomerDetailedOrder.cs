using BackendCommon.Enums;
using ProjCommon.Enums;

namespace BackendCommon.DTO;

public class CustomerDetailedOrder
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime DeliveryTime { get; set; }
    public int FinalPrice { get; set; }
    public string Address { get; set; }
    public OrderStatus Status { get; set; }
    public Guid? RestaurantId { get; set; }
    public ICollection<DishShort> Dishes { get; set; }
}