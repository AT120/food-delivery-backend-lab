namespace BackendCommon.DTO;

public class RestaurantsPage
{
    public PageInfo Page { get; set; }
    public ICollection<RestaurantDTO> Restaurants { get; set; }
}
