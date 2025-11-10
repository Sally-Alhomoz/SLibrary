using Blazor2.Authentication;
using Blazored.LocalStorage;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Blazor2.Services;

public record LoginDto(string Username, string password);
public record LoginResponse(string token);

public class AccountService : IAccountService
{
    private readonly HttpClient _client;
    private readonly ILocalStorageService _local;
    private readonly CustomAuthStateProvider _auth;

    public AccountService(HttpClient http, ILocalStorageService local, CustomAuthStateProvider auth)
    {
        _client = http;
        _local = local;
        _auth = auth;
    }

    public async Task<List<Userdto>> Read()
    {
        var users = await _client.GetFromJsonAsync<List<Userdto>>("api/Account");
        return users;
    }

    public async Task<bool> Login(string user, string pass)
    {
        var response = await _client.PostAsJsonAsync("api/Account/Login", new LoginDto(user, pass));
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (string.IsNullOrEmpty(result?.token)) return false;

        await _local.SetItemAsync("jwt", result.token);
        _auth.Notify();
        return true;
    }

    public async Task Logout()
    {
        var jwt = await _local.GetItemAsync<string>("jwt");

        await _local.RemoveItemAsync("jwt");

        if (!string.IsNullOrWhiteSpace(jwt))
        {
            var username = GetUsernameFromJwt(jwt);
            if (!string.IsNullOrWhiteSpace(username))
            {
                var endpoint = $"api/Account/SetInActive/{Uri.EscapeDataString(username)}";
                await _client.PatchAsync(endpoint, new StringContent(string.Empty));
            }
        }
        _auth.Notify();
    }

    private static string? GetUsernameFromJwt(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                ?? token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        }
        catch
        {
            return null;
        }
    }
}

