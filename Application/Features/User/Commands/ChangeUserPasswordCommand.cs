using Application.Services.Identity;
using Common.Requests.Identity;
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
