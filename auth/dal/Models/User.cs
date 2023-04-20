using System.ComponentModel.DataAnnotations.Schema;
using AuthCommon.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthDAL.Models;

public class User : IdentityUser<Guid>
// where T : IEquatable<T>
{
    public string FullName { get; set; }

    [Column(TypeName = "date")]
    public DateTime? BirthDate { get; set; }

    public Gender Gender { get; set; }
}