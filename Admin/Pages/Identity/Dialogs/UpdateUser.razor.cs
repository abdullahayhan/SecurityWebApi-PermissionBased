using Common.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Admin.Pages.Identity.Dialogs
{
    public partial class UpdateUser
    {
        [Parameter]
        public UpdateUserRequest UpdateUserRequest {get;set;} = new();
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;

        private async Task UpdateUserAsync()
        {
            var response = await _userService.UpdateUserAsync(UpdateUserRequest);
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

        public void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
