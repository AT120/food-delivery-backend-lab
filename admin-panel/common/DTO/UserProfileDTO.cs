using ProjCommon.Enums;

namespace AdminCommon.DTO;

public class UserProfile
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }
    public required IEnumerable<RoleType> Roles { get; set; }
}