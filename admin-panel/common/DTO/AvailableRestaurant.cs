namespace AdminCommon.DTO;

public class AvailableRestaurant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public bool UserWorkingHere { get; set; } = false;
}