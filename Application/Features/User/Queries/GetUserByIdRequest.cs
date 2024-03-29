using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.User.Queries;

public record GetUserByIdRequest(string Id) : IRequest<IResponseWrapper>;


public class GetUserByIdRequestHandler : IRequestHandler<GetUserByIdRequest, IResponseWrapper>
{
    private readonly IUserService _userService;

    public GetUserByIdRequestHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByIdAsync(request.Id);
    }
}
