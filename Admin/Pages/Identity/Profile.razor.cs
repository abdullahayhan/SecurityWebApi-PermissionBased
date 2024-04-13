using Admin.Extensions;
using Common.Requests.Identity;
using MudBlazor;

namespace Admin.Pages.Identity
{
    public partial class Profile
    {
        private UpdateUserRequest UpdateUserRequest = new UpdateUserRequest();
        public string UserId { get; set; } = string.Empty;
        private char _firstLetterOfFirstName;
        private string _email = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            _email = user.GetEmail();
            UpdateUserRequest.UserId = user.GetUserId();
            UpdateUserRequest.FirstName = user.GetFirstName();
            UpdateUserRequest.LastName = user.GetLastName();
            if (UpdateUserRequest.FirstName.Length > 0)
            {
                _firstLetterOfFirstName = UpdateUserRequest.FirstName[0];
            }
        }

        private async Task UpdateUserAsync()
        {
            var response = await _userService.UpdateUserAsync(UpdateUserRequest);
            if (response.IsSuccessful)
            {
                await _tokenService.Logout();
                _snackBar.Add("Profil güncellendi. Devam etmek için tekrar giriş yapınız.", Severity.Success);
                _navigationManager.NavigateTo("/");
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}
