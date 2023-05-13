using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FsCodeProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            // Perform user authentication (e.g., verify credentials)
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                return Unauthorized(); // Invalid credentials
            }

            // Generate and return JWT token
            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token });
        }
    }
}
