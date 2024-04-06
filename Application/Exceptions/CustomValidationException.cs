namespace Application.Exceptions;

public class CustomValidationException : Exception
{
    public List<string> ErrorsMessage { get; set; } = new();
    public string? FriendlyErrorsMessage { get; set; }

    public CustomValidationException(List<string> errorsMessage, string? friendlyErrorsMessage)
        : base(friendlyErrorsMessage)
    {
        ErrorsMessage = errorsMessage;
        FriendlyErrorsMessage = friendlyErrorsMessage;
    }
}
