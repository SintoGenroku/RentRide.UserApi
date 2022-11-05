using System.ComponentModel.DataAnnotations;

namespace Users.Api.Models.Requests.Users;

public class UserCreateRequestModel
{
    [Required(ErrorMessage = "Please Enter your Login")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Please Enter your fullname")]
    public string Fullname { get; set; }
    
    [Required(ErrorMessage = "Please Enter your Password")]
    [RegularExpression(@"[\x00-\x7F]{6,100}", ErrorMessage = "Only english, numeric and special symbols allowed")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Please confirm your Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
    
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    public string MailAddress { get; set; }
    
    [RegularExpression(@"^[+][\d]{10,13}$")]
    public string PhoneNumber { get; set; }
}