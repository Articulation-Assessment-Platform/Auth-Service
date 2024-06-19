namespace AuthServiceLayer.Subscriptions
{
    public class UserMessage
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
