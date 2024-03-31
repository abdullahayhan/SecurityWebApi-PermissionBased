using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.User.Queries;

public record class GetRolesRequest(string UserId) : IRequest<IResponseWrapper>;

public class GetRolesRequestHandler : IRequestHandler<GetRolesRequest, IResponseWrapper>
{
    private IUserService _userService;

    public GetRolesRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(GetRolesRequest request,
        CancellationToken cancellationToken)
    {
        return await _userService.GetRolesAsync(request.UserId);
    }
}
