using Admin.Extensions;
using Microsoft.AspNetCore.Components;

namespace Admin.Shared.Components
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; } = string.Empty;
        [Parameter] public string Style { get; set; } = string.Empty;
        private string FirstName { get; set; }
        private string LastName { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfName { get; set; }
        private bool _isLoaded;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            _isLoaded = true;
        }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            Email = user.GetEmail();
            FirstName = user.GetFirstName();
            LastName = user.GetLastName();
            if (FirstName.Length > 0)
            {
                FirstLetterOfName = FirstName[0];
            }
        }
    }
}
