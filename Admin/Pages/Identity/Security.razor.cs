using Common.Requests.Identity;
using MudBlazor;

namespace Admin.Pages.Identity
{
    public partial class Security
    {
        private readonly ChangeUserPasswordRequest ChangeUserPasswordRequest = new();

        private bool _currentPasswordVisibility;
        private InputType _currentPasswordInput = InputType.Password;
        private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        private bool _newPasswordVisibility;
        private InputType _newPasswordInput = InputType.Password;
        private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private async Task ChangePasswordAsync()
        {
            var response = await _userService.ChangePasswordAsync(ChangeUserPasswordRequest);
            if (response.IsSuccessful)
            {
                _snackBar.Add("Password Changed!", Severity.Success);
                Reset();
            }
            else
            {
                foreach (var error in response.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private void TogglePasswordVisibility(bool newPassword)
        {
            if (newPassword)
            {
                if (_newPasswordVisibility)
                {
                    _newPasswordVisibility = false;
                    _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                    _newPasswordInput = InputType.Password;
                }
                else
                {
                    _newPasswordVisibility = true;
                    _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                    _newPasswordInput = InputType.Text;
                }
            }
            else
            {
                if (_currentPasswordVisibility)
                {
                    _currentPasswordVisibility = false;
                    _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                    _currentPasswordInput = InputType.Password;
                }
                else
                {
                    _currentPasswordVisibility = true;
                    _currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                    _currentPasswordInput = InputType.Text;
                }
            }
        }

        private void Reset()
        {
            ChangeUserPasswordRequest.CurrentPassword = string.Empty;
            ChangeUserPasswordRequest.NewPassword = string.Empty;
            ChangeUserPasswordRequest.ConfirmedNewPassword = string.Empty;
        }
    }
}
