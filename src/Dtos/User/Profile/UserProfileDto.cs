namespace Dtos.User.Profile
{
    public class UserProfileDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string image { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}