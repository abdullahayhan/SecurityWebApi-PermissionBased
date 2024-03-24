namespace Common.Requests;

public class TokenRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
