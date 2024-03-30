namespace Common.Requests.Identity;

public class UpdateUserRequest
{
    public required string UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
}
