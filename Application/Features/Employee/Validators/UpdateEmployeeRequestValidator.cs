using Common.Requests.Employee;
using FluentValidation;

namespace Application.Features.Employee.Validators;

internal class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
	public UpdateEmployeeRequestValidator()
	{
		RuleFor(request => request.FirstName)
			.NotEmpty()
			.MaximumLength(60);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Salary)
            .NotEmpty();
    }
}
