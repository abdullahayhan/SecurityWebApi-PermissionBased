using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Role.Queries;

public record class GetPermissionsRequest(string RoleId) : IRequest<IResponseWrapper>;

public class GetPermissionsRequestHandler : IRequestHandler<GetPermissionsRequest, IResponseWrapper>
{
    private readonly IRoleService _roleService;

    public GetPermissionsRequestHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IResponseWrapper> Handle(GetPermissionsRequest request, CancellationToken cancellationToken)
    {
        return await _roleService.GetPermissionsAsync(request.RoleId);
    }
}
