using Toolbelt.Blazor;

namespace Admin.Services.Interfaces
{
    public interface IHttpInterceptorService : ITransient
    {
        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void RegisterEvent();
    }
}
