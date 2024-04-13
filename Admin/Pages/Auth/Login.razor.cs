using Common.Authorization;
using Common.Requests.Identity;
using MudBlazor;

namespace Admin.Pages.Auth
{
    public partial class Login
    {
        private TokenRequest _tokenRequest = new();

        public Uri ReturnUrl { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            if (state.User.Identity?.IsAuthenticated is true)
            {
                _navigationManager.NavigateTo(ReturnUrl.AbsoluteUri);
            }
        }

        private async Task SubmitAsync()
        {
            var result = await _tokenService.Login(_tokenRequest);
            if (result.IsSuccessful)
            {
                _navigationManager.NavigateTo("/", true);
                _snackBar.Add(string.Format("Hoşgeldiniz {0}", _tokenRequest.Email), Severity.Success);
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }

        private void FillAdministratorCredentials()
        {
            _tokenRequest.Email = AppCredentials.Email;
            _tokenRequest.Password = "uw8876!@!@(#dg!k!!";
        }
    }
}
