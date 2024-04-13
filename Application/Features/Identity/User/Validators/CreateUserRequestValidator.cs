using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Identity;
using FluentValidation;

namespace Application.Features.Identity.User.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator(IUserService userService)
    {
        RuleFor(request => request.Email)
                .NotEmpty()
        .EmailAddress()
        .MaximumLength(256)
                .MustAsync(async (email, ct) =>
                {
                    var result = await userService.GetUserByEmailAsync(email);
                    return !result.IsSuccessful;
                })
                .WithMessage("Email'e ait bir kayıt bulunmaktadır.");

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.UserName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.Password)
            .NotEmpty();

        RuleFor(request => request.ConfirmPassword)
            .Must((req, confirmed) => req.Password == confirmed)
            .WithMessage("Şifreler aynı olmalıdır.");
    }
}
