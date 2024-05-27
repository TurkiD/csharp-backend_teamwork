using System.ComponentModel.DataAnnotations;

namespace api.Dtos.User
{
    public class SignupDto
    {
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]{3,29}$", ErrorMessage = "Please enter a valid Username")]
        public required string Username { get; set; }
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", ErrorMessage = "Please enter a valid email")]
        public required string Email { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Please enter a valid password")]
        public required string Password { get; set; }
    }
}