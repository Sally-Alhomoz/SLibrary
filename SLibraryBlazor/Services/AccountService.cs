using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Writers;
using Shared;
using System.Net.Http.Headers;

namespace SLibraryBlazor.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _client;
        private readonly ProtectedSessionStorage _sessionStorage;

        public AccountService(HttpClient client, ProtectedSessionStorage sessionStorage)
        {
            _client = client;
            _sessionStorage = sessionStorage;
        }

        public async Task<bool> Login(dtoLogin model)
        {
            var response = await _client.PostAsJsonAsync("api/Account/Login", model);

            if (!response.IsSuccessStatusCode)
                return false;

            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            var token = data?.token;

            await _sessionStorage.SetAsync("JWToken", token);

            return true;
        }

        public async Task<string?> GetJwtToken()
        {
            var result = await _sessionStorage.GetAsync<string>("JWToken");
            return result.Success ? result.Value : null;
        }

        public async Task Logout()
        {
            await _sessionStorage.DeleteAsync("JWToken");
        }
    }

    public class LoginResponse
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
}
