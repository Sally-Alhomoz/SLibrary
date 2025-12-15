using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazored.LocalStorage;
using System.Text.Json;

namespace SLibrary.Blazor.Services.Authentication_Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("jwt");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaims(token);

            var nameClaim = claims.FirstOrDefault(c => c.Type == "name");
            if (nameClaim != null)
            {
                claims = claims.Where(c => c.Type != ClaimTypes.Name)
                               .Append(new Claim(ClaimTypes.Name, nameClaim.Value));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task LoginAsync(string token)
        {
            await _localStorage.SetItemAsync("jwt", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("jwt");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static IEnumerable<Claim> ParseClaims(string jwt)
        {
            try
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? ""))
                       ?? Enumerable.Empty<Claim>();
            }
            catch
            {
                return Enumerable.Empty<Claim>();
            }
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public async Task<string?> GetUserNameAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.Identity?.Name;
        }

        public async Task<string?> GetUserRoleAsync()
        {
            var state = await GetAuthenticationStateAsync();
            var user = state.User;

            return user.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }




    }
}