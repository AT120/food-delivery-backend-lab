using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthDAL.Models;

public class User : IdentityUser<Guid> 
    // where T : IEquatable<T>
{
    public string FullName
    {
        get { return UserName; }
        set { UserName = value; }
    }

    [Column(TypeName="date")]
    public DateTime? BirthDate { get; set; }

    public Gender Gender { get; set; }
}