using Application.Features.Employee.Commands;
using Application.Services;
using FluentValidation;

namespace Application.Features.Employee.Validators.DeleteEmployeeCommandValidator;

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
	private readonly IEmployeeService _employeeService;
    public DeleteEmployeeCommandValidator(IEmployeeService employeeService)
    {
        _employeeService = employeeService;

        RuleFor(command => command.id)
            .MustAsync(async (id,ct) => await _employeeService.GetEmployeeByIdAsync(id) 
            is Domain.Employee employee && employee.Id != 0)
            .NotEmpty().WithMessage("Id zorunlu alan.");
    }
}
