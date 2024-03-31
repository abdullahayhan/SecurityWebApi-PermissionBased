using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Role.Queries;

public record GetRoleByIdRequest(string RoleId) : IRequest<IResponseWrapper>;

public class GetRoleByIdRequestHandler : IRequestHandler<GetRoleByIdRequest, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRoleByIdRequestHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(GetRoleByIdRequest request, CancellationToken cancellationToken)
    {
        return await _roleService.GetRoleAsync(request.RoleId);
    }
}
