namespace Common.Responses;

public class Error
{
    public List<string> ErrorsMessage { get; set; } = new();
    public string? Description { get; set; }
}
