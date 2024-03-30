using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface IRoleService
{
    Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest);
}
