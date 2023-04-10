using AuthCommon.Enums;
using ProjCommon.Enums;

namespace AuthCommon.DTO;

public class UserProfile
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public ICollection<RoleType> Roles { get; set; }
    public string? Address { get; set; }
}