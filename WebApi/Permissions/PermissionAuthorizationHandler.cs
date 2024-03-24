using Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Permissions;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        if (context.User is null)
        {
            await Task.CompletedTask;
        }

        var permission = context.User?.Claims
            .Where(claim => claim.Type == AppClaim.Permission
                && claim.Value == requirement.Permission
                && claim.Issuer == "LOCAL AUTHORITY");

        if (permission.Any())
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }
}
