using Ecommerce.Domain.Models;
using Ecommerce.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserCredentials credentials)
        {
            try
            {
                _authService.RegisterUser(credentials.Username, credentials.Password);
                return Ok("User registered successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            if (_authService.Authenticate(credentials.Username, credentials.Password, out var token))
            {
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid credentials");
        }
    }
}
