using Admin.Services.Auth;
using Admin.Services.Interfaces;
using Blazored.LocalStorage;
using Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Admin.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        private const string ClientName = "ABC Api";
        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var permission in AppPermissions.AllPermissions)
            {
                options.AddPolicy(permission.Name, policy => policy.RequireClaim(AppClaim.Permission, permission.Name));
            }
        }

        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            builder.Services
                .AddAuthorizationCore(options =>
                {
                    RegisterPermissionClaims(options);
                })
                .AddBlazoredLocalStorage()
                .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 6000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = true;
                })
                //.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddTransientServices()
                .AddScoped<ApplicationStateProvider>()
                .AddScoped<AuthenticationStateProvider, ApplicationStateProvider>()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiEndpoints:BaseApiUrl"));
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();
            return builder;
        }

        public static IServiceCollection AddTransientServices(this IServiceCollection services)
        {
            var transientServices = typeof(ITransient);

            var types = transientServices
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (transientServices.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }
    }
}
