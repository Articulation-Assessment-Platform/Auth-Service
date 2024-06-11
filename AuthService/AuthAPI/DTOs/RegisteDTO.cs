using System.ComponentModel.DataAnnotations;

namespace AuthAPI.DTOs
{
    public class RegisteDTO
    {
        [Required(ErrorMessage = "User was not created no ID present which is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}
