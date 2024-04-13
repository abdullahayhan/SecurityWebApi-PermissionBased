using Admin.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Headers;
using Toolbelt.Blazor;

namespace Admin.Services.Implementations
{
    public class HttpInterceptorService : IHttpInterceptorService
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly ITokenService _tokenService;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackbar _snackBar;

        public HttpInterceptorService(HttpClientInterceptor interceptor, ITokenService tokenService, NavigationManager navigationManager, ISnackbar snackBar)
        {
            _interceptor = interceptor;
            _tokenService = tokenService;
            _navigationManager = navigationManager;
            _snackBar = snackBar;
        }

        public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request.RequestUri.AbsolutePath;
            //To Do: User registration need to be bipassed as well. Update register route.
            if (!absPath.Contains("token"))
            {
                try
                {
                    var token = await _tokenService.TryForceRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add("You are Logged Out.", Severity.Error);
                    await _tokenService.Logout();
                    _navigationManager.NavigateTo("/");
                }
            }
        }
    }
}
