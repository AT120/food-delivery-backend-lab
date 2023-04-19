namespace BackendCommon.DTO;

public class CourierOrderDTO
{
    public int Id { get; set; }
    public string Address { get; set; }
    public Guid RestaurantId { get; set; }
}