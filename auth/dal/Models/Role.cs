using Microsoft.AspNetCore.Identity;
using ProjCommon.Enums;

namespace AuthDAL.Models;

public class Role : IdentityRole<Guid> 
{
    public RoleType RoleType { get; set; }
    public ICollection<User> Users { get; set; }
}