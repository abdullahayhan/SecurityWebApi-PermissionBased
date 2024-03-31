using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Role.Commands;

public record DeleteRoleCommand(string RoleId) : IRequest<IResponseWrapper>;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public DeleteRoleCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        return await _roleService.DeleteRoleAsync(request.RoleId);
    }
}
