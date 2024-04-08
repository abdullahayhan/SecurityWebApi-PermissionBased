using Application.Features.Employee.Commands;
using Application.Services;
using FluentValidation;

namespace Application.Features.Employee.Validators.UpdateEmployeeCommandValidator;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{

    public UpdateEmployeeCommandValidator(IEmployeeService employeeService)
    {
        RuleFor(command => command.UpdateEmployeeRequest)
            .SetValidator(new UpdateEmployeeRequestValidator(employeeService));

    }
}
