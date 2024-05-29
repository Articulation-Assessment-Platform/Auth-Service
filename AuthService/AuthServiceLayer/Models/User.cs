namespace AuthServiceLayer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
