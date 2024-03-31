using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.User.Commands;

public record class UpdateUserCommand(UpdateUserRequest UpdateUserRequest) : IRequest<IResponseWrapper>;


public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserAsync(request.UpdateUserRequest);
    }
}