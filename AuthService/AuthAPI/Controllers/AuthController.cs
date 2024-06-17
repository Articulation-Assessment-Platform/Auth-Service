using AuthAPI.DTOs;
using AuthServiceLayer.Models;
using AuthServiceLayer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authenticationService;
        private readonly PasswordHasherService _passwordHasherService;

        public AuthController(IAuthService authenticationService, PasswordHasherService passwordHasherService)
        {
            _authenticationService = authenticationService;
            _passwordHasherService = passwordHasherService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var speechTherapist = _authenticationService.Authenticate(model.Email, model.Password);

            if (speechTherapist == null)
                return Unauthorized();

            var token = _authenticationService.GenerateToken(speechTherapist);

            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisteDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var registerResponse = await _authenticationService.Register(new User
                {
                    UserId = model.UserId,
                    Email = model.Email,
                    Role = model.Role,
                    Password = model.Password
                });

                if (registerResponse.Success)
                {
                    return Ok(registerResponse);
                }
                else
                {
                    return BadRequest(new { message = registerResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteUser([FromBody] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _authenticationService.DeleteUser(email);

            return Ok();
        }

    }

}
