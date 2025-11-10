//using Microsoft.AspNetCore.Components.Authorization; 
//using System.Security.Claims;
//using System.Text.Json;
//using System.Collections.Generic;

//namespace Blazor2.Services
//{
//    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
//    {
//        private readonly ITokenService _tokenService;

//        public CustomAuthenticationStateProvider(ITokenService tokenService)
//        {
//            _tokenService = tokenService;
//        }

//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            var token = await _tokenService.GetToken();

//            if (string.IsNullOrEmpty(token))
//                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

//            var claims = ParseClaimsFromJwt(token);
//            var identity = new ClaimsIdentity(claims, "jwt");
//            var user = new ClaimsPrincipal(identity);

//            return new AuthenticationState(user);
//        }

//        public void NotifyUserAuthentication(string token)
//        {
//            var claims = ParseClaimsFromJwt(token);
//            var identity = new ClaimsIdentity(claims, "jwt");
//            var user = new ClaimsPrincipal(identity);

//            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//        }

//        public void NotifyUserLogout()
//        {
//            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
//            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
//        }

//        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
//        {
//            var claims = new List<Claim>();

//            // JWT format: header.payload.signature
//            var payload = jwt.Split('.')[1];
//            var jsonBytes = ParseBase64WithoutPadding(payload);

//            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

//            if (keyValuePairs != null)
//            {
//                foreach (var kvp in keyValuePairs)
//                {
//                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
//                }
//            }

//            return claims;
//        }

//        private byte[] ParseBase64WithoutPadding(string base64)
//        {
//            switch (base64.Length % 4)
//            {
//                case 2: base64 += "=="; break;
//                case 3: base64 += "="; break;
//            }
//            return Convert.FromBase64String(base64);
//        }
//    }
//}


using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace Blazor2.Authentication;

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
        var identity = string.IsNullOrEmpty(token)
            ? new ClaimsIdentity()
            : new ClaimsIdentity(ParseClaims(token), "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void Notify() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Pad(payload)));
        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
        return dict.Select(k => new Claim(k.Key, k.Value.ToString()!));
    }
    private static string Pad(string s)
    {
        return s.Length % 4 == 0 ? s : s + "===".Substring(0, 4 - s.Length % 4);
    }
}