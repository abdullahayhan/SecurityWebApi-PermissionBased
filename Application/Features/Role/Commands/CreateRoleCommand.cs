using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Role.Commands;

public record CreateRoleCommand(CreateRoleRequest CreateRoleRequest) 
    : IRequest<IResponseWrapper>;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public CreateRoleCommandHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(CreateRoleCommand request, 
        CancellationToken cancellationToken)
    {
        return await _roleService
            .CreateRoleAsync(request.CreateRoleRequest);
    }
}
