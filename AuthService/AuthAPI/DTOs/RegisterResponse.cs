using System.ComponentModel.DataAnnotations;

namespace AuthAPI.DTOs
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

}
