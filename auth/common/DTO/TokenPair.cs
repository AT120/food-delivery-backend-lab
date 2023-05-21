namespace AuthCommon.DTO;

public class TokenPair
{
    public required string RefreshToken { get; set; }
    public required string AccessToken { get; set; }
}