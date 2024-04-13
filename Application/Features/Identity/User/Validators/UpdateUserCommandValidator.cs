using Application.Features.Identity.User.Commands;
using Common.Requests.Identity;
using FluentValidation;

namespace Application.Features.Identity.User.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	public UpdateUserCommandValidator()
	{
		RuleFor(command => 
			command.UpdateUserRequest)
			.SetValidator(new UpdateUserRequestValidator());
	}
}
