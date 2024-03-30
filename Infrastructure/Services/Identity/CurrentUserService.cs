using Application.Services.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		UserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?
			.Value!;
	}
    public string UserId { get; }
}
