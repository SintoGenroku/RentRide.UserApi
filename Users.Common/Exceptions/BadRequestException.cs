namespace Users.Common.Exceptions;

public class BadRequestException : Exception
{
    public Dictionary<string, string[]?>? Errors; 
    
    public BadRequestException() {}
        
    public BadRequestException(string message) : base(message) {}

    public BadRequestException(string message, Exception inner) : base(message, inner) {}

    public BadRequestException(string message, Dictionary<string, string[]?> errors) : base(message)
    {
        Errors = errors;
    }
}