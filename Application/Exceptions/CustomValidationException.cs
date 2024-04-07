namespace Application.Exceptions;

public class CustomValidationException : Exception
{
    public List<string> ErrorsMessage { get; set; } = new();
    public string? Description { get; set; }

    public CustomValidationException(List<string> errorsMessage, string? description)
        : base(description)
    {
        ErrorsMessage = errorsMessage;
        Description = description;
    }
}
