using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Shared;
using System.Net;

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

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(dtoNewUser model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _client.PostAsJsonAsync("api/Account/Register", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User added successfully";
                return RedirectToAction("UsersList");
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

            var username = User.Identity.Name;


            if (!string.IsNullOrEmpty(username))
            {
                var endpoint = $"api/Account/SetInActive/{Uri.EscapeDataString(username)}";

                var response = await _client.PatchAsync(endpoint, new StringContent(""));
            }

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

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Logout");

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


        public async Task<IActionResult> EditAccount()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.GetAsync($"api/Account/GetAccountInfo");

            if(!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load account information.";
                return View(new Userdto());
            }

            var user = await response.Content.ReadFromJsonAsync<Userdto>();
            return View("EditAccount",user);
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(EditAccountdto model)
        {
            if (!ModelState.IsValid) 
            {
                return View("EditAccount", model);
            }


            if (model.ResetPassworddto.NewPassword != model.ResetPassworddto.ConfirmPassword)
            {
                ModelState.AddModelError("ResetPassworddto.ConfirmPassword", "The new password and confirmation do not match.");
                return View("EditAccount",model);
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.PatchAsJsonAsync("api/Account", model.ResetPassworddto);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                if (errorMessage.Contains("Incorrect old password"))
                {
                    ModelState.AddModelError("ResetPassworddto.OldPassword", "The current password entered is incorrect.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
                return View("EditAccount", model);
            }
            Logout();
            return RedirectToAction("Login");
        }

        public IActionResult EditEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditEmail(EditAccountdto model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditAccount", model);
            }

            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                Logout();
                return RedirectToAction("Login");
            }

            var request = new HttpRequestMessage(HttpMethod.Patch, "api/Account/EditEmail")
            {
                Content = JsonContent.Create(model.EditEmaildto)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                if (errorMessage.Contains("Email already esixt"))
                {
                    ModelState.AddModelError("EditEmaildto.newEmail", "Email already esixt");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
                return View("EditAccount", model);
            }
            Logout();
            return RedirectToAction("Login");
        }


    }

    public class LoginResponse
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
}