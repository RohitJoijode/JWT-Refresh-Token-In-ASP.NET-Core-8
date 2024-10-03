using JWT_TOKEN_REFRESH_NET_CORE_8.Models;
using JWT_TOKEN_REFRESH_NET_CORE_8.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly DBEngine _DbEngine;

        public AuthController(IAuthService authService,DBEngine DbEngine)
        {
            _authService = authService;
            _DbEngine = DbEngine;
        }

        public class RegisterUserModel
        {
            public string? UserName { get; set; }
            public string? UserPassword { get; set; }
            public string? UserRole { get; set; }
            public string? UserEmail { get; set; }
            public string? UserMobile { get; set; }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterUserModel user)
        {
            if (await _authService.RegisterUser(user))
            {
                return Ok("Successfully done");
            }
            return BadRequest("Something went wrong");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LogInUser user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var loginResult = await _authService.Login(user);
            if(loginResult.IsLogedIn)
            {
                return Ok(loginResult);
            }
            return Unauthorized();
        }

        //[Authorize]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenModel model)
        {
            var loginResult = await _authService.RefreshToken(model);
            if (loginResult.IsLogedIn)
            {
                return Ok(loginResult);
            }
            return Unauthorized();
        }

      


    }
}
