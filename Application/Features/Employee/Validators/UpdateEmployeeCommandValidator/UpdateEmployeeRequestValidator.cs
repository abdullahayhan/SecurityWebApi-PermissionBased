using Application.Services;
using Common.Requests.Employee;
using FluentValidation;

namespace Application.Features.Employee.Validators.UpdateEmployeeCommandValidator;

internal class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator(IEmployeeService employeeService)
    {
        RuleFor(request => request.Id)
            .MustAsync(async (id, ct) => await employeeService.GetEmployeeByIdAsync(id)
            is Domain.Employee employeeInDb && employeeInDb.Id == id)
            .WithMessage("Employee bulunumadı.");

        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("Employee FirstName zorunludur")
            .MaximumLength(60);

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Employee LastName zorunludur")
            .MaximumLength(60);

        RuleFor(request => request.Email)
            .EmailAddress()
            .NotEmpty().WithMessage("Employee Email zorunludur")
            .MaximumLength(100);

        RuleFor(request => request.Salary)
            .NotEmpty().WithMessage("Employee Salary zorunludur");
    }
}
