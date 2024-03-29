using Common.Requests.User;
using Common.Responses.Wrappers;

namespace Application.Services;

public interface IUserService
{
    Task<IResponseWrapper> CreateUserAsync(CreateUserRequest createUserRequest);
    Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
    Task<IResponseWrapper> GetAllUsersAsync();
    Task<IResponseWrapper> ChangeUserPassword(ChangeUserPasswordRequest changeUserPasswordRequest);
}
