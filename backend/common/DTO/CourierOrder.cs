namespace BackendCommon.DTO;

public class CourierOrder
{
    public int Id { get; set; }
    public string Address { get; set; }
    public Guid RestaurantId { get; set; }
}