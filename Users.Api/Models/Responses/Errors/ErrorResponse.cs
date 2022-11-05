namespace Users.Api.Models.Responses.Errors;

public class ErrorResponse
{
    public int StatusCode { get; set; }
        
    public string Message { get; set; }
        
    public string Details { get; set; }
        
    public ErrorResponse InnerError { get; set; }
}