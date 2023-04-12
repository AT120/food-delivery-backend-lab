using System.ComponentModel;

namespace AuthCommon.DTO;

public class PasswordPair
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}