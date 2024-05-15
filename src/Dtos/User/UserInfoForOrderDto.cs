namespace Dtos.User
{
    public class UserInfoForOrderDto
    {
        public Guid UserID { get; set; }
        public required string Username { get; set; }
    }
}