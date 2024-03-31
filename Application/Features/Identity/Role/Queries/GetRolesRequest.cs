using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Role.Queries;

public record class GetRolesRequest(string? RoleName)
    : IRequest<IResponseWrapper>;

public class GetRolesRequestHandler : IRequestHandler<GetRolesRequest, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetRolesRequestHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(GetRolesRequest request, CancellationToken cancellationToken)
    {
        return await _roleService.GetRolesAsync(request.RoleName);
    }
}
