using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Admin.Extensions;
using Common.Authorization;

namespace Admin.Shared
{
    public partial class NavMenu
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;
        private bool _canViewUsers;
        private bool _canViewRoles;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;
            _canViewUsers = await AuthService.HasPermissionAsync(user, AppFeature.Users, AppAction.Read);
            _canViewRoles = await AuthService.HasPermissionAsync(user, AppFeature.UserRoles, AppAction.Read);
        }
    }
}
