using System.ComponentModel.DataAnnotations;

namespace api.Dtos.User
{
    public class LoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Please enter a valid password")]
        public string Password { get; set; } = string.Empty;
    }
}