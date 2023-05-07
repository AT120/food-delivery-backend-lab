namespace AdminCommon.DTO;

public class UserProfileEditDTO
{
    public Guid UserId { get; set; }
    public bool[] Roles { get; set; } = new bool[5]; //TODO: max roleType
    public Guid? NewRestaurantId { get; set; } 
}