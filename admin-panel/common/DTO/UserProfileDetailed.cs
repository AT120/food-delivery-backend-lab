using System.ComponentModel.DataAnnotations;
using ProjCommon.Enums;

namespace AdminCommon.DTO;

public class UserProfileDetailed
{
    public Guid Id { get; set; }
    public DateTime? BirthDate { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }
    public required IEnumerable<RoleType> Roles { get; set; }
    
    public required IEnumerable<AvailableRestaurant> AvailableRestaurants { get; set; }
}