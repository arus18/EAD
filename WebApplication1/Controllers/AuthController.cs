//Authentication Controller
// This controller handles user authentication.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration configuration;
        private readonly UserService _userService;

        // Constructor for AuthController
        public AuthController(IConfiguration configuration,UserService userService)
        {
            this.configuration = configuration;
            this._userService = userService;
        }

        // Authenticate user
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] User user)
        {
            IActionResult response = Unauthorized();

            if (user != null)
            {
                // Retrieve the user by NIC from the UserService.
                RegistrationUser dbUser = await _userService.GetUserByNicAsync(user.NIC);

                if (dbUser != null && user.Password.Equals(dbUser.Password))
                {
                    var issuer = configuration["Jwt:Issuer"];
                    var audience = configuration["Jwt:Audience"];
                    var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature
                    );

                    var subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, dbUser.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, "email"),
                        new Claim(JwtRegisteredClaimNames.NameId, dbUser.Id)
                        // You can add other claims as needed
                    });

                    var expires = DateTime.UtcNow.AddMinutes(10);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = subject,
                        Expires = expires,
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = signingCredentials
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);

                    return Ok(jwtToken);
                }
            }

            return response;
        }

    }
}