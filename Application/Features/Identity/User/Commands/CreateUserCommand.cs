﻿using Application.Pipelines;
using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.User.Commands;

public record CreateUserCommand(CreateUserRequest CreateUserRequest) 
    : IRequest<IResponseWrapper>
      ,IValidateMe;


public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.CreateUserAsync(request.CreateUserRequest);
    }
}
