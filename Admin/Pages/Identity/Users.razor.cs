using Common.Responses.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Admin.Extensions;
using Common.Authorization;
using MudBlazor;
using Admin.Pages.Identity.Dialogs;
using Common.Requests.Identity;

namespace Admin.Pages.Identity
{
    public partial class Users
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;
        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;

        private List<UserResponse> _userList = new();
        private UserResponse _user = new();
        private bool _canCreateUsers;
        private bool _canViewRoles;
        private bool _canUpdateUsers;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;
            _canCreateUsers = await AuthService.HasPermissionAsync(user, AppFeature.Users, AppAction.Read);
            _canUpdateUsers = await AuthService.HasPermissionAsync(user, AppFeature.Users, AppAction.Update);
            _canViewRoles = await AuthService.HasPermissionAsync(user, AppFeature.UserRoles, AppAction.Read);
            await GetUsersAsync();
            _loaded = true;
        }

        private async Task GetUsersAsync()
        {
            var response = await _userService.GetAllAsync();
            if (response.IsSuccessful)
            {
                _userList = response.ResponseData;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void ManageUserRoles(string userId, string email)
        {
            if (email.Equals(AppCredentials.Email, StringComparison.CurrentCultureIgnoreCase))
            {
                _snackBar.Add("Admin kullanıcısının profili düzenlenemez.", Severity.Error);
            }
            else {
                _navigationManager.NavigateTo($"/pages/identity/user-roles/{userId}");            
            }
        }

        private async Task UpdatUserDialog(string userId, string email)
        {
            if (email.Equals(AppCredentials.Email, StringComparison.CurrentCultureIgnoreCase))
            {
                _snackBar.Add("Admin kullanıcısının profili düzenlenemez.", Severity.Error);
            }
            else
            {
                var userToUpdate = _userList.First(user => user.Id == userId);

                var parameters = new DialogParameters()
            {
                { nameof(UpdateUser.UpdateUserRequest), new UpdateUserRequest
                    {
                        UserId = userId,
                        FirstName = userToUpdate.FirstName,
                        LastName = userToUpdate.LastName,
                        IsActive = userToUpdate.IsActive
                    }
                }
            };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
                var dialog = _dialogService.Show<UpdateUser>("Kullanıcı Güncelle.", parameters, options);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await GetUsersAsync();
                }
            }
        }

        private async Task UserRegistrationDialog()
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<RegisterUser>("Yeni Kullanıcı Oluştur.", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await GetUsersAsync();
            }
        }

        private void Cancel()
        {
            _navigationManager.NavigateTo("/");
        }
    }
}
