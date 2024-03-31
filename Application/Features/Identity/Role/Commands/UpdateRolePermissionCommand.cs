using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Role.Commands;

public record UpdateRolePermissionCommand(UpdateRolePermissionsRequest UpdateRolePermissionsRequest) 
    : IRequest<IResponseWrapper>;

public class UpdateRolePermissionCommandHandler : IRequestHandler<UpdateRolePermissionCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public UpdateRolePermissionCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
    {
        return await _roleService
            .UpdateRolePermissionsAsync(request.UpdateRolePermissionsRequest);
    }
}
