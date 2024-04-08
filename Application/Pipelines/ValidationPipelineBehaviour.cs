using Application.Exceptions;
using Common.Responses.Wrappers;
using FluentValidation;
using MediatR;

namespace Application.Pipelines;

public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> , IValidateMe
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            List<string> errors = new();

            var validationResult = await Task
                .WhenAll(_validators
                    .Select(validationResult => validationResult.ValidateAsync(context,
                    cancellationToken)));

            if (!validationResult.Any(vr => vr.IsValid)) // birden fazla validasyon var, en az biri valid değilse yazdır.
            {
                var failures = validationResult.SelectMany(vr => vr.Errors)
               .Where(failure => failure != null)
               .ToList();

                foreach (var failure in failures)
                {
                    errors.Add(failure.ErrorMessage);
                }

                return (TResponse)await ResponseWrapper.FailAsync(errors);
                //throw new CustomValidationException(errors, "Bir veya daha fazla hata meydana geldi."); // yalan geceler.
            }
        }

        return await next();
    }
}
