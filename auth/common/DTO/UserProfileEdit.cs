using System.ComponentModel.DataAnnotations;
using ProjCommon.Enums;

namespace AuthCommon.DTO;

public class UserProfileEdit
{
    public string? Name { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }   
}