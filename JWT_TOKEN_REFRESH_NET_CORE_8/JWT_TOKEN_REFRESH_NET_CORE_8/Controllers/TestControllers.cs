using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using JWT_TOKEN_REFRESH_NET_CORE_8.Models;
using JWT_TOKEN_REFRESH_NET_CORE_8.Services;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TestControllers : Controller
    {

        private readonly IAuthService _authService;
        private readonly DBEngine _DbEngine;

        public TestControllers(IAuthService authService,DBEngine DbEngine)
        {
            _authService = authService;
            _DbEngine = DbEngine;
        }

        [HttpGet("Test")]
        public IActionResult Index()
        {
            return Ok();
        }

        [Authorize]
        [HttpPost("AuthorizedTest")]
        public IActionResult AuthorizedTest() //string token
        {
            var authorizationHeader = this.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            //var authorizationHeader = "Bearer " + token;
            string jwtTokenString = authorizationHeader.Replace("Bearer ", "");

            var jwt = new JwtSecurityToken(jwtTokenString);

            var response = $"Authenticated!{Environment.NewLine}";

            //response += $"{Environment.NewLine}Exp Time : { jwt.ValidTo.ToLongTimeString()}, Time : {DateTime.Now.ToLongTimeString()}";
            return Ok(response);
        }


        [Authorize]
        [HttpGet("Users")]
        public IActionResult Users() 
        {
            var response = _DbEngine.Tbl_Users1.ToList(); 
             return Ok(response);
        }
    }
}
