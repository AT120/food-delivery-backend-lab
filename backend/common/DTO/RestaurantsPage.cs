namespace BackendCommon.DTO;

public class RestaurantsPage
{
    public int RangeStart { get; set; }
    public int RangeEnd { get; set; }
    public int Size { get; set; }
    public ICollection<RestaurantDTO> Restaurants { get; set; }
}