using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using nvt_back.DTOs;
using nvt_back.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogInDto credentials)
        {
            try
            {
                User user = await _userService.GetByEmailAndPassword(credentials.Email, credentials.Password);

                if (user == null)
                {
                    return BadRequest("Invalid credentials");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret_key"));
                var credentialsJWT = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims,
                    expires: DateTime.Now.AddMinutes(100),
                    signingCredentials: credentialsJWT
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    Expires = DateTime.UtcNow.AddHours(2),

                });

                return Ok(new UserDTO(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

    }
}
