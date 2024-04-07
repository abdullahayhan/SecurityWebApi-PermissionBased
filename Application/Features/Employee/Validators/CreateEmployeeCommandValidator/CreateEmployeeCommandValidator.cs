using Application.Features.Employee.Commands;
using FluentValidation;

namespace Application.Features.Employee.Validators.CreateEmployeeCommandValidator;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(command => command.CreateEmployeeRequest)
            .SetValidator(new CreateEmployeeRequestValidator());
    }
}
