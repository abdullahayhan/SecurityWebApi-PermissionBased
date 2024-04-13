using Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Admin.Services.Auth
{
    public class MustHavePermissionAttribute : AuthorizeAttribute
    {
        public MustHavePermissionAttribute(string action, string resource) =>
            Policy = AppPermission.NameFor(action, resource);
    }
}
