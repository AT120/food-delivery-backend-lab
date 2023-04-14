using BackendCommon.Enums;

namespace BackendDAL.Entities;

public class Order
{
    public int Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public int FinalPrice { get; set; }
    public string Address { get; set; }

    public Restaurant Restaurant { get; set; }
    public OrderStatus Status { get; set; }
    public Cook  Cook { get; set; }
    public Courier Courier { get; set; }
    
}