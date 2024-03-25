using Common.Requests.User;
using Common.Responses.Wrappers;

namespace Application.Services;

public interface IUserService
{
    Task<IResponseWrapper> CreateUserAsync(CreateUserRequest createUserRequest);
    Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest updateUserRequest);
}
