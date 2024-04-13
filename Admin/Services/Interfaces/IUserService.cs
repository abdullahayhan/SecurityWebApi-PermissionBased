using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;

namespace Admin.Services.Interfaces
{
    public interface IUserService : ITransient
    {
        Task<IResponseWrapper<List<UserResponse>>> GetAllAsync();
        Task<IResponseWrapper<UserResponse>> GetByIdAsync(string userId);
        Task<IResponseWrapper<List<UserRoleViewModel>>> GetRolesAsync(string userId);
        Task<IResponseWrapper> RegisterUserAsync(CreateUserRequest request);
        Task<IResponseWrapper<string>> UpdateUserAsync(UpdateUserRequest request);
        //Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);
        Task<IResponseWrapper> UpdateRolesAsync(UpdateUserRolesRequest request);
        Task<IResponseWrapper> ChangePasswordAsync(ChangeUserPasswordRequest model);
    }
}
