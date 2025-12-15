using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLibrary.DataAccess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared;
using SLibrary.Business.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using SLibrary.Business.Managers;
using static System.Reflection.Metadata.BlobBuilder;

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IUserManager userManager, IConfiguration config, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            configuration = config;
            _logger = logger;
        }

        /// <summary>
        /// Register.
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Create(dtoNewUser user)
        {
            _logger.LogInformation("POST called to register a new user");
            var exist = _userManager.GetByUsername(user.Username);
            if (exist != null)
            {
                _logger.LogWarning("Failed - Username already exist");
                return BadRequest("Username already exists.");
            }

            _userManager.Add(user);
            _logger.LogInformation("User registered successfully");
            return Ok("User registered successfully");

        }

        /// <summary>
        /// Login to Account.
        /// </summary>

        [HttpPost("Login")]
        public async Task<IActionResult> LogIn(dtoLogin login)
        {
            _logger.LogInformation("POST called to login");
            var isValid = _userManager.Validatelogin(login);

            if (!isValid)
            {
                _logger.LogWarning("login failed - Invalid username or password");
                return Unauthorized("Invalid username or password");
            }

            var u = _userManager.GetByUsername(login.Username);

            var user = new Userdto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                IsActive = true
            };

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.Username),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        ///<summary>
        ///Get all users.
        ///</summary>
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> Read()
        //{
        //    _logger.LogInformation("GET called to fetch all users");
        //    var users = _userManager.GetAllUsers();
        //    _logger.LogInformation("Returned {Count} users.", users.Count);
        //    return Ok(users);
        //}

        /// <summary>
        /// Delete User.
        /// </summary>

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(string username)
        {
            _logger.LogInformation("DELETE called to delete a user ");
            var currentUser = HttpContext.User.Identity?.Name;
            var user = _userManager.GetByUsername(currentUser);
            if (user == null || user.Role != Role.Admin)
            {
                _logger.LogWarning("Delete failed - Only admins can delete user");
                return StatusCode(403, "Only admins can delete users.");
            }

            var result = _userManager.Delete(username);
            if (result.Contains("successfully"))
            {
                _logger.LogInformation("User with Username: {username} deleted succssfully ", username);
                return Ok(result);
            }
            else
            {
                _logger.LogInformation("Deleting user with Username: {username} Failed ", username);
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Sets a user's status to inactive (when client logout).
        /// </summary>
        [HttpPatch("SetInActive/{username}")]
        [Authorize]
        public IActionResult SetInActive(string username)
        {
            _logger.LogInformation("PATCH called to set user {Username} to inactive.", username);

            var success = _userManager.SetUserInActive(username);

            if (success)
            {
                _logger.LogInformation("User {Username} successfully set to inactive.", username);
                return NoContent();
            }
            else
            {
                _logger.LogWarning("Failed to set user {Username} to inactive.", username);
                return StatusCode(500, "Failed to update user status.");
            }
        }

        /// <summary>
        /// Resets a user password
        /// </summary>
        [HttpPatch]
        [Authorize]
        public IActionResult RestPassword([FromBody] ResetPassworddto dto)
        {

            var username = User.Identity.Name;

            _logger.LogInformation("PATCH called to reset password for user {Username}", username);

            var user = _userManager.GetByUsername(username);

            if (user == null)
            {
                _logger.LogWarning("Password reset failed: user {Username} not found", username);
                return NotFound("User not found.");
            }

            var result = _userManager.ValidatePassword(user.Username, dto.OldPassword);

            if (!result)
            {
                _logger.LogWarning("Incorrect old password for user {Username}", username);
                return BadRequest("Incorrect old password.");
            }

            _userManager.ResetPassword(user.Username, dto.NewPassword, dto.OldPassword);
            _logger.LogInformation("Successfully reset password for user {Username}", username);
            return Ok();
        }

        /// <summary>
        /// Edit Email
        /// </summary>
        [HttpPatch("EditEmail")]
        [Authorize]
        public IActionResult EditEmail([FromBody] EditEmaildto dto)
        {

            var username = User.Identity.Name;

            _logger.LogInformation("PATCH called to edit email for user {Username}", username);

            var user = _userManager.GetByUsername(username);

            if (user == null)
            {
                _logger.LogWarning("Failed: user {Username} not found", username);
                return NotFound("User not found.");
            }

            var result = _userManager.EditEmail(username, dto.newEmail);

            if (!result)
            {
                _logger.LogWarning("Email already esixt");
                return BadRequest("Email already esixt");
            }

            _logger.LogInformation("Successfully edit email for user {Username}", username);
            return Ok();
        }

        ///<summary>
        ///Get users info.
        ///</summary>
        [HttpGet("GetAccountInfo")]
        [Authorize]
        public async Task<IActionResult> GetAccountInfo()
        {

            var username = User.Identity.Name;
            _logger.LogInformation("Get called to get account info for user {Username}", username);

            var user = _userManager.GetAccountInfo(username);

            if (user == null)
            {
                _logger.LogWarning("User Not Found");
                return NotFound("User Not Found");
            }

            _logger.LogInformation("Successfully get info for user {Username}", username);
            return Ok(user);
        }

        [HttpPatch("togglestatus")]
        [Authorize]
        public async Task<IActionResult> ToggleUserStatus(string username)
        {
            var user = _userManager.GetByUsername(username);
            if (user == null) return NotFound("User not found.");

            var result = _userManager.toggleUserStatus(username);

            if (result)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return BadRequest();

            var user = _userManager.GetByUsername(username);
            if (user != null)
            {
                var success = _userManager.SetUserInActive(username);

                if (success)
                {
                    _logger.LogInformation("User {Username} successfully set to inactive.", username);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning("Failed to set user {Username} to inactive.", username);
                    return StatusCode(500, "Failed to update user status.");
                }
            }

            return Ok(new { message = "Logged out and deactivated" });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Read(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "username",
            [FromQuery] string sortDirection = "asc")
        {
            var (users, totalCount) = _userManager.GetUsersPaged(
                page, pageSize, search, sortBy, sortDirection);

            return Ok(new
            {
                items = users,
                totalCount = totalCount
            });
        }


    }

}