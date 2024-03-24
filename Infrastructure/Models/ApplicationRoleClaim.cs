using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Models;

public class ApplicationRoleClaim : IdentityRoleClaim<string> // key alanı string olacak demek <string>.
{
    public string? Description { get; set; }
    public string? Group { get; set; }
}
