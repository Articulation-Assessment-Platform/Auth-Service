using AuthServiceLayer.Models;
using AuthServiceLayer.Services;
using AuthRepositoryLayer.Entities;
using AuthRepositoryLayer.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AuthServiceLayer.Tests
{
    public class AuthServiceTests
    {
        private IAuthService _authService;
        private Mock<IAuthRepository> _authRepositoryMock;
        private Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<PasswordHasherService> _passwordHasherMock;

        public AuthServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            _configurationMock = new Mock<IConfiguration>();
            _passwordHasherMock = new Mock<PasswordHasherService>();

            _authService = new AuthService(_jwtSettingsMock.Object, _configurationMock.Object, _authRepositoryMock.Object, _passwordHasherMock.Object);
        }



   


        [Fact]
        public void DeleteUser_ValidEmail_SuccessfullyDeleted()
        {
            // Arrange
            var email = "test@example.com";
            var userEntity = new UserCredentialsEntity { Email = email };
            _authRepositoryMock.Setup(x => x.GetUserByEmail(email)).Returns(userEntity);

            // Act
            _authService.DeleteUser(email);

            // Assert
            _authRepositoryMock.Verify(x => x.DeleteAsync(userEntity), Times.Once);
        }
    }
}
