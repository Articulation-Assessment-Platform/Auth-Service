using AuthServiceLayer.Models;

namespace AuthServiceLayer.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        User Authenticate(string email, string password);
        Task<RegisterResponse> Register(User user);
    }
}
