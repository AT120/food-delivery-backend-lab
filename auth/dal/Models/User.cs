using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using ProjCommon.Enums;

namespace AuthDAL.Models;

public class User : IdentityUser<Guid>
// where T : IEquatable<T>
{
    public string FullName { get; set; } = "";

    [Column(TypeName = "date")]
    public DateTime? BirthDate { get; set; }

    public Gender Gender { get; set; }

    public ICollection<Role> Roles { get; set; }
}