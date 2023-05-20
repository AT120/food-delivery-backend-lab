using System.ComponentModel.DataAnnotations;

namespace ProjCommon.DTO;

public class LoginCreds
{
    [EmailAddress]
    public string Email { get; set; } = "";
    public string Password  { get; set; } = "";
}