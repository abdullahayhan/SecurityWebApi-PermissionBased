namespace Common.Responses.Identity;

public class RoleClaimViewModel
{
    public required string RoleId { get; set; }
    public required string ClaimType { get; set; }
    public required string ClaimValue { get; set; }
    public string? Description { get; set; }
    public string? Group { get; set; }
    public bool IsAssignedToRole { get; set; }
}