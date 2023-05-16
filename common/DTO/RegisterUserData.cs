using System.ComponentModel.DataAnnotations;
using ProjCommon.Enums;

namespace AuthCommon.DTO;

public class RegisterUserData
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }

}
