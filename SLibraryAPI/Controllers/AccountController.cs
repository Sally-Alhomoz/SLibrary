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

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserManager  _userManager;
        private readonly IConfiguration configuration;
        public AccountController(IUserManager userManager , IConfiguration config)
        {
            _userManager = userManager;
            configuration = config;
        }

        /// <summary>
        /// Register.
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Create(dtoNewUser user)
        {
            var exist = _userManager.GetByUsername(user.Username);
            if (exist != null)
                return BadRequest("Username already exists.");

            _userManager.Add(user);

            return Ok(new { message = "User registered successfully" });

        }

        /// <summary>
        /// Login to Account.
        /// </summary>

        [HttpPost("Login")]
        public async Task<IActionResult> LogIn(dtoLogin login)
        {
            var isValid = _userManager.Validatelogin(login);

            if (!isValid)
                return Unauthorized("Invalid username or password");

            var u = _userManager.GetByUsername(login.Username);

            var user = new Userdto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role
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

     
    }
}
