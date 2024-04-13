using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Common.Responses.Identity;
using Admin.Extensions;
using Common.Authorization;
using MudBlazor;
using Common.Requests.Identity;

namespace Admin.Pages.Identity
{
    public partial class RolePermissions
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;
        [Parameter]
        public string RoleId { get; set; } = string.Empty;
        private string _title = string.Empty;
        private string _descr = string.Empty;

        private RoleClaimResponse _roleClaimResponse = new();

        private Dictionary<string, List<RoleClaimViewModel>> RoleClaimsGrouped { get; } = new();
        private RoleClaimViewModel _roleClaimViewModel = new();
        private RoleClaimViewModel _selectedRoleClaimViewModel = new();

        private bool _canUpdateRolePermissions;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;
            _canUpdateRolePermissions = await AuthService.HasPermissionAsync(user, AppFeature.RoleClaims, AppAction.Update);

            await GetRolePermissionsAsync();
            _loaded = true;
        }

        private async Task GetRolePermissionsAsync()
        {
            var response = await _roleService.GetPermissionsAsync(RoleId);
            if (response.IsSuccessful)
            {
                _roleClaimResponse = response.ResponseData;

                if (_roleClaimResponse is not null)
                {
                    _title = "İzinleri yönet";
                    _descr = string.Format("{0} rolüne ait izinleri yönet", _roleClaimResponse.Role.Name);
                }

                RoleClaimsGrouped.Add("All Permissions", _roleClaimResponse.RoleClaims);

                foreach (var permission in _roleClaimResponse.RoleClaims)
                {
                    if (RoleClaimsGrouped.ContainsKey(permission.Group))
                    {
                        RoleClaimsGrouped[permission.Group].Add(permission);
                    }
                    else
                    {
                        RoleClaimsGrouped.Add(permission.Group, new List<RoleClaimViewModel> { permission });
                    }
                }
            }
            else
            {
                foreach (var error in response.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
                _navigationManager.NavigateTo("/pages/identity/roles");
            }
        }

        private async Task UpdateRolePermissionsAsync()
        {
            var updatePermissionsRequest = new UpdateRolePermissionsRequest { RoleId = RoleId, RoleClaims = _roleClaimResponse.RoleClaims };
            var response = await _roleService.UpdatePermissionsAsync(updatePermissionsRequest);
            if (response.IsSuccessful)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _navigationManager.NavigateTo("/pages/identity/roles");
            }
            else
            {
                foreach (var error in response.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private Color GetGroupBadgeColor(int selected, int all)
        {
            if (selected == 0)
                return Color.Error;

            if (selected == all)
                return Color.Success;

            return Color.Warning;
        }

        private void Cancel()
        {
            _navigationManager.NavigateTo("/pages/identity/roles");
        }
    }
}
