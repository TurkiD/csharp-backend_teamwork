namespace api.Dtos.User
{
    public class SignupDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}