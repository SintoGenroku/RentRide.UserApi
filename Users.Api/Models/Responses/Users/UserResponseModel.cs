namespace Users.Api.Models.Responses.Users;

public class UserResponseModel
{
    public Guid Id { get; set; }
    
    public string? Fullname { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? MailAddress { get; set; }
    
    public bool IsActive { get; set; }
}