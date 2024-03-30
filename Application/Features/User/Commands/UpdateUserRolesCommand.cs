using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.User.Commands;

public record class UpdateUserRolesCommand(UpdateUserRolesRequest UpdateUserRolesRequest) 
    : IRequest<IResponseWrapper>;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public UpdateUserRolesCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        return await _userService
            .UpdateUserRolesAsync(request.UpdateUserRolesRequest);
    }
}
