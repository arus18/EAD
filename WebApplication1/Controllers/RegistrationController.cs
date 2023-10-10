using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Models;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public RegistrationController(IConfiguration configuration, UserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationUser registrationUser)
        {
            if (registrationUser != null)
            {
                // You can add custom validation logic here.
                // For example, check if the username or email is already in use.
                // If validation fails, return a BadRequest with an error message.

                // Simulate a simple validation check (e.g., username should not be empty):
                if (string.IsNullOrWhiteSpace(registrationUser.UserName))
                {
                    return BadRequest("Username is required.");
                }

                // Perform user registration and save to the database.
                try
                {
                    // You should hash the password before saving it to the database for security.
                    // For simplicity, we'll assume it's already hashed.
                    // You may also want to perform additional validation for the NIC field.

                    // Save the user to the database (assuming _userService has the necessary methods):
                    RegistrationUser user = await _userService.CreateAsync(registrationUser);

                    // Generate a JWT token for the registered user (similar to your existing code).
                    var issuer = _configuration["Jwt:Issuer"];
                    var audience = _configuration["Jwt:Audience"];
                    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature
                    );

                    var subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, registrationUser.UserName),
                        new Claim(JwtRegisteredClaimNames.NameId, registrationUser.Id)
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Handle any exceptions that occur during registration or database storage.
                    // You can log the error, return an appropriate error response, or perform other actions.
                    return StatusCode(500, "Registration failed. Please try again later.");
                }
            }

            return BadRequest("Invalid registration data.");
        }


        // Add the remaining CRUD endpoints for user management

        [HttpGet("users")]
        [Authorize] // You can apply authorization as needed
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        [Authorize] // You can apply authorization as needed
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("users")]
        [Authorize] // You can apply authorization as needed
        public async Task<IActionResult> CreateUser([FromBody] RegistrationUser user)
        {
            await _userService.CreateAsync(user);
            return CreatedAtAction("GetUserById", new { id = user.Id }, user);
        }

        [HttpPut("users/{id}")]
        [Authorize] // You can apply authorization as needed
        public async Task<IActionResult> UpdateUser(string id, [FromBody] RegistrationUser user)
        {
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userService.UpdateAsync(id, user);
            return NoContent();
        }

        [HttpDelete("users/{id}")]
        [Authorize] // You can apply authorization as needed
        public async Task<IActionResult> DeleteUser(string id)
        {
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost("activate/{id}")]
        [Authorize] // You can apply authorization as needed
        public async Task<IActionResult> ActivateUser(string id)
        {
            // Call SetActivationAsync from the UserService to activate the user
            await _userService.SetActivationAsync(id, true);

            return Ok("User activated successfully.");
        }

        [HttpPost("deactivate/{id}")]
        [Authorize] // You can apply authorization as needed
        public async Task<IActionResult> DeactivateUser(string id)
        {
            // Call SetActivationAsync from the UserService to deactivate the user
            await _userService.SetActivationAsync(id, false);

            return Ok("User deactivated successfully.");
        }


    }
}