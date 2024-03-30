using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> CreateUserAsync(CreateUserRequest createUserRequest);
    Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
    Task<IResponseWrapper> GetAllUsersAsync();
    Task<IResponseWrapper> ChangeUserPassword(ChangeUserPasswordRequest changeUserPasswordRequest);
    Task<IResponseWrapper> GetRolesAsync(string userId);
    Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest updateUserRolesRequest);
}
