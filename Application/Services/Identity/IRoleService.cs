using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface IRoleService
{
    Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest);
    Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest);
    Task<IResponseWrapper> GetRoleAsync(string roleId);
    Task<IResponseWrapper> GetRolesAsync(string? roleName);
    Task<IResponseWrapper> DeleteRoleAsync(string roleId);
}
