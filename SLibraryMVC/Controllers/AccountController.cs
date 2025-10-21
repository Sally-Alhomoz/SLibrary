using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Shared; 

namespace SLibraryMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _client;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("api");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(dtoNewUser model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _client.PostAsJsonAsync("api/Account/Register", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User registered successfully! Please log in.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }
        }


        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(dtoLogin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _client.PostAsJsonAsync("api/Account/Login", model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid username or password.";
                return View(model);
            }

            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();

            await SignInUser(data.token);

            HttpContext.Session.SetString("JWToken", data.token);

            return RedirectToAction("Index","Home");
        }

        [HttpPost] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("JWToken");
            return RedirectToAction("Login");
        }

        [Authorize] 
        public async Task<IActionResult> UsersList()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token)) return RedirectToAction("Logout");

            var request = new HttpRequestMessage(HttpMethod.Get, "api/Account");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                TempData["Error"] = "Session expired or access denied.";
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            var users = await response.Content.ReadFromJsonAsync<List<Userdto>>();
            return View(users);
        }


        [Authorize] 
        [HttpPost] 
        public async Task<IActionResult> Delete(string username)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token)) return RedirectToAction("Logout");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/Account?username={username}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                TempData["Message"] = $"User {username} deleted successfully!";
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                TempData["Message"] = "Delete failed: Only Admins can delete users.";
            else
                TempData["Message"] = "Delete failed: " + await response.Content.ReadAsStringAsync();

            return RedirectToAction("UsersList");
        }


        /// <summary>
        /// Parses the JWT and establishes the local authentication cookie.
        /// </summary>
        private async Task SignInUser(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            // Use claims (Name, Role, NameIdentifier) from the JWT
            var claims = token.Claims;

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = token.ValidTo
            };

            // This creates the authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }

    public class LoginResponse
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
}