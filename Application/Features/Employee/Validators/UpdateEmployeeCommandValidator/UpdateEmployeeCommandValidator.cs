using Application.Features.Employee.Commands;
using Application.Services;
using FluentValidation;

namespace Application.Features.Employee.Validators.UpdateEmployeeCommandValidator;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    private readonly IEmployeeService _employeeService;

    public UpdateEmployeeCommandValidator(IEmployeeService employeeService)
    {
        _employeeService = employeeService;

        RuleFor(command => command.UpdateEmployeeRequest)
            .SetValidator(new UpdateEmployeeRequestValidator(_employeeService));

    }
}
