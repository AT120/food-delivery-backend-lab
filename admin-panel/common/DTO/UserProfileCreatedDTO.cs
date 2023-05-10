using System.ComponentModel.DataAnnotations;
using AuthCommon.DTO;
using ProjCommon.Enums;

namespace AdminCommon.DTO;

public class UserProfileCreateDTO
{
    public RegisterUserData UserData { get; set; }
    public Guid? RestaurantId { get; set; }
    public bool[] Roles { get; set; } = new bool[5];

}