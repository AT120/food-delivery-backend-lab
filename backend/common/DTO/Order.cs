namespace BackendCommon.DTO;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public int Price { get; set; }
    public string Address { get; set; }
    public OrderStatus Status { get; set; }
}
