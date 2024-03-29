using Application.Services;
using Common.Requests.User;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.User.Commands;

public record class ChangeUserPasswordCommand(ChangeUserPasswordRequest ChangeUserPasswordRequest) : IRequest<IResponseWrapper>;


public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public ChangeUserPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _userService.ChangeUserPassword(request.ChangeUserPasswordRequest);
    }
}
