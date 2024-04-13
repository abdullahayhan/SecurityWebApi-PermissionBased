using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Admin.Services.Auth
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthenticationHeaderHandler(ILocalStorageService localStorage)
            => _localStorage = localStorage;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Headers.Authorization?.Scheme != "Bearer")
                {
                    var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.AuthToken, cancellationToken);

                    if (!string.IsNullOrWhiteSpace(savedToken))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                    }
                }

                return await base.SendAsync(request, cancellationToken);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
