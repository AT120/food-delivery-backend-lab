using System.ComponentModel.DataAnnotations;
using AuthCommon.Validators;

namespace AuthCommon.DTO;

public class LoginCreds
{
    [EmailAddress]
    public string Email { get; set; }
    public string Password  { get; set; }
}