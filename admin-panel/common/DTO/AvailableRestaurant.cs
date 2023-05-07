namespace AdminCommon.DTO;

public class AvailableRestaurant
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = "";
    public bool UserWorkingHere { get; set; } = false;
}