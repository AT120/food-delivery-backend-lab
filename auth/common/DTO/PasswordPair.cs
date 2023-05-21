using System.ComponentModel;

namespace AuthCommon.DTO;

public class PasswordPair
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}