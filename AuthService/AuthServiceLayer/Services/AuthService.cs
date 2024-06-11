using AuthServiceLayer.Models;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AuthRepositoryLayer.Repository;
using AuthRepositoryLayer.Entities;
using System.Diagnostics.Tracing;

namespace AuthServiceLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly PasswordHasherService _passwordHasher;

        public AuthService(IOptions<JwtSettings> jwtSettings, IConfiguration configuration, IAuthRepository authRepository, PasswordHasherService passwordHasherService)
        {
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
            _authRepository = authRepository;
            _passwordHasher = passwordHasherService;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User Authenticate(string email, string password)
        {
            UserCredentialsEntity user = _authRepository.GetUserByEmail(email);

            // No speech therapist with this email 
            if (user == null) { return null; }

            // Password hash
            if (_passwordHasher.VerifyPassword(password, user.HashedPassword, user.Salt))
            {
                // hash matches
                return new User()
                {
                    Email = user.Email,
                    Role = user.Role,
                    UserId = user.UserID
                };
            }

            // Authentication was not successful 
            return null;
        }
        public async Task<RegisterResponse> Register(User user)
        {
            var (hashedPassword, salt) = _passwordHasher.HashPassword(user.Password);
            UserCredentialsEntity u = await _authRepository.AddAsync(new UserCredentialsEntity()
            {
                UserID = user.UserId,
                Email = user.Email,
                HashedPassword = hashedPassword,
                Role = user.Role,
                Salt = salt
            });

            if (u != null)
            {
                var token = GenerateToken(new User() { UserId = u.UserID, Role = u.Role, Email = u.Email });
                return new RegisterResponse
                {
                    Success = true,
                    Token = token,
                    Message = "User registered successfully",
                    UserId = u.UserID,
                    Email = u.Email,
                    Role = u.Role
                };
            }
            else
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "User registration failed"
                };
            }
        }

        public void DeleteUser(string email)
        {
            UserCredentialsEntity user = _authRepository.GetUserByEmail(email);
            _authRepository.DeleteAsync(user);
        }
    }
}
