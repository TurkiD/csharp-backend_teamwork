using System.ComponentModel.DataAnnotations;

namespace api.Dtos.User
{
    public class SignupDto
    {
        public Guid UserID { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}