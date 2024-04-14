using Common.Responses.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Admin.Extensions;
using Common.Authorization;
using MudBlazor;
using Common.Requests.Identity;

namespace Admin.Pages.Identity
{
    public partial class UserRoles
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;
        [Parameter]
        public string UserId { get; set; } = string.Empty;


        private List<UserRoleViewModel> _userRolesList = new();
        private UserResponse _user = new();
        private UserRoleViewModel _userRole = new();
        private bool _canUpdateUserRoles;
        private bool _loaded;
        private string _title = string.Empty;
        private string _descr = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;
            _canUpdateUserRoles = await AuthService.HasPermissionAsync(user, AppFeature.UserRoles, AppAction.Update);

            await GetUserByIdsAsync(UserId);
            await GetRolesAsync(UserId);
            _loaded = true;
        }

        private async Task GetUserByIdsAsync(string userId)
        {
            var response = await _userService.GetByIdAsync(userId);
            if (response.IsSuccessful)
            {
                _user = response.ResponseData;
                _title = $"{_user.FirstName} {_user.LastName} Rol Bilgisi";
                _descr = $"{_user.FirstName} {_user.LastName} kullanıcısına ait rolleri yönet!";
            }
        }

        private async Task GetRolesAsync(string userId)
        {
            var response = await _userService.GetRolesAsync(userId);
            if (response.IsSuccessful)
            {
                _userRolesList = response.ResponseData;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task UpdateUserRolesAsync()
        {
            var request = new UpdateUserRolesRequest()
            {
                UserId = UserId,
                Roles = _userRolesList
            };
            var result = await _userService.UpdateRolesAsync(request);
            if (result.IsSuccessful)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                _navigationManager.NavigateTo("/pages/identity/users");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private void Cancel()
        {
            _navigationManager.NavigateTo("/pages/identity/users");
        }
    }
}
