using System.ComponentModel.DataAnnotations.Schema;
using AuthCommon.DTO;
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


    public User() { }

    public User(RegisterCreds creds)
    {
        this.UserName = creds.Email;
        this.Email = creds.Email;
        this.FullName = creds.Name;
        this.PhoneNumber = creds.PhoneNumber;
        this.Gender = creds.Gender;
        this.BirthDate = creds.BirthDate;
    }
}