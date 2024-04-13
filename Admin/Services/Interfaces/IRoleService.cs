using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;

namespace Admin.Services.Interfaces
{
    public interface IRoleService : ITransient
    {
        Task<IResponseWrapper<List<RoleResponse>>> GetRolesAsync();
        Task<IResponseWrapper<string>> CreateAsync(CreateRoleRequest request);
        Task<IResponseWrapper<string>> UpdateAsync(UpdateRoleRequest request);
        Task<IResponseWrapper<string>> DeleteAsync(string roleId);
        Task<IResponseWrapper<RoleClaimResponse>> GetPermissionsAsync(string roleId);
        Task<IResponseWrapper<string>> UpdatePermissionsAsync(UpdateRolePermissionsRequest request);
    }
}
