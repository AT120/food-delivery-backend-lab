using System.ComponentModel.DataAnnotations;

namespace AuthCommon.DTO;

public class LoginCreds
{
    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public string Password  { get; set; }
}