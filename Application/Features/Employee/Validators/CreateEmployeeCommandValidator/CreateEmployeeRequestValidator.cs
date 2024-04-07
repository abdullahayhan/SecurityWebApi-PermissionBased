using Common.Requests.Employee;
using FluentValidation;

namespace Application.Features.Employee.Validators.CreateEmployeeCommandValidator;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("Employee FirstName zorunludur")
            .MaximumLength(60).WithMessage("FirstName uzunluğu 60'dan fazla olamaz.");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Employee LastName zorunludur")
            .MaximumLength(60).WithMessage("LastName uzunluğu 60'dan fazla olamaz.");

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Employee Email zorunludur");

        RuleFor(request => request.Salary)
            .NotEmpty().WithMessage("Employee Salary zorunludur");
    }
}
