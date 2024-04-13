using Common.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Admin.Pages.Identity.Dialogs
{
    public partial class RegisterUser
    {
        private CreateUserRequest _userRegistration = new();
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;
        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;        

        private async Task SubmitAsync()
        {
            var response = await _userService.RegisterUserAsync(_userRegistration);
            if (response.IsSuccessful)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void TogglePasswordVisibility()
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

        private void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
