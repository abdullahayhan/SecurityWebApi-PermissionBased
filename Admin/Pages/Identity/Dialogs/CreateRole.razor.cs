using Common.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Admin.Pages.Identity.Dialogs
{
    public partial class CreateRole
    {
        private CreateRoleRequest _roleRequest { get; set; } = new();
        [CascadingParameter] 
        private MudDialogInstance MudDialog { get; set; } = default!;

        private async Task SaveRoleAsync()
        {
            var response = await _roleService.CreateAsync(_roleRequest);
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
