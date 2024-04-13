using Common.Requests.Identity;
using Common.Responses.Wrappers;
using System.Security.Claims;

namespace Admin.Services.Interfaces
{
    public interface ITokenService : ITransient
    {
        Task<IResponseWrapper> Login(TokenRequest request);

        Task<IResponseWrapper> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
    }
}
