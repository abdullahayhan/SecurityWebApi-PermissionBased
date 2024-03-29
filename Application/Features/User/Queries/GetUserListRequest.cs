using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.User.Queries;

public record GetUserListRequest : IRequest<IResponseWrapper>;

public class GetUserListRequestHandler : IRequestHandler<GetUserListRequest, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetUserListRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(GetUserListRequest request, CancellationToken cancellationToken)
    {
        return await _userService.GetAllUsersAsync();
    }
}
