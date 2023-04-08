using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AuthCommon.Enums;
using AuthCommon.Validators;

namespace AuthCommon.DTO;

public class RegisterCreds
{
    [RequiredOR(typeof(RegisterCreds), nameof(PhoneNumber))]
    [EmailAddress]
    public string? Email { get; set; }
    public string Passwrod { get; set; }
    public string Name { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    
}
