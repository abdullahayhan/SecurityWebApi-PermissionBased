using Application.Features.Employee.Commands;
using FluentValidation;

namespace Application.Features.Employee.Validators;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
	public UpdateEmployeeCommandValidator()
	{
		RuleFor(command => command.UpdateEmployeeRequest)
			.SetValidator(new UpdateEmployeeRequestValidator());
	}
}
