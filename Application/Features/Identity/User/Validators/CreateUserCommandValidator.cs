using Application.Features.Identity.User.Commands;
using Application.Services.Identity;
using Common.Requests.Identity;
using FluentValidation;

namespace Application.Features.Identity.User.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
	public CreateUserCommandValidator(IUserService userService)
	{
		RuleFor(command => command.CreateUserRequest)
			.SetValidator(new CreateUserRequestValidator(userService));
	}
}
