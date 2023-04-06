using System.ComponentModel.DataAnnotations;
using AuthDAL.Models;
using Npgsql.Internal.TypeHandlers;

namespace AuthApi.Models;

public class RegisterCreds
{
    [EmailAddress]
    public string Email { get; set; }
    public string Passwrod { get; set; }
    public string Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
}
