using ProjCommon.Enums;

namespace AdminCommon.DTO;

public class UserProfile
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }
    public IEnumerable<RoleType> Roles { get; set; }
}