using Common.Requests.Identity;
using FluentValidation;

namespace Application.Features.Identity.User.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(60).WithMessage("Ad karakter uzunluğu 60'dan büyük olamaz");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.")
            .MaximumLength(60).WithMessage("Soyad karakter uzunluğu 60'dan büyük olamaz");
    }
}
