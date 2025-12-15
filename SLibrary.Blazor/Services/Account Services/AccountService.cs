using Blazored.LocalStorage;
using Shared;
using SLibrary.Blazor.Services.Authentication_Services;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using static SLibrary.Blazor.Services.Book_Services.BookService;

namespace SLibrary.Blazor.Services.Account_Services
{
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

        //public async Task<List<Userdto>> Read()
        //{
        //    var users = await _client.GetFromJsonAsync<List<Userdto>>("api/Account");
        //    return users;
        //}


        public async Task<(List<Userdto> Items, int TotalCount)> Read(
            int page = 1, int pageSize = 10, string search = "", string sortBy = "username", string sortDirection = "asc")
        {
            var url = $"api/Account?page={page}&pageSize={pageSize}&search={search}&sortBy={sortBy}&sortDirection={sortDirection}";
            var response = await _client.GetFromJsonAsync<PagedResult>(url);
            return (response?.Items ?? new(), response?.TotalCount ?? 0);
        }

        public record PagedResult(List<Userdto> Items, int TotalCount);

        public async Task<bool> Login(string user, string pass)
        {
            var response = await _client.PostAsJsonAsync("api/Account/Login", new LoginDto(user, pass));
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (string.IsNullOrEmpty(result?.token)) return false;

            await _auth.LoginAsync(result.token);

            return true;
        }

        public async Task Logout()
        {

            await _client.PostAsync("api/Account/Logout", null);

            await _auth.LogoutAsync();
        }

        public async Task Delete(string username)
        {
            var response = await _client.DeleteAsync($"api/Account?username={username}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{errorMessage}");
            }
        }

        public async Task<Userdto> GetAccountInfo()
        {
            var response = await _client.GetAsync("api/Account/GetAccountInfo");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Userdto>();
            }

            else
            {
                string errorMsg = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"{errorMsg}");
            }
        }

        public async Task EditEmail(string NewEmail)
        {
            var dto = new EditEmaildto { newEmail = NewEmail };

            var response = await _client.PatchAsJsonAsync("api/Account/EditEmail", dto);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }
        public async Task ChangePassword(ResetPassworddto dto)
        {
            var response = await _client.PatchAsJsonAsync("api/Account",dto);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }

        public async Task Register(dtoNewUser dto)
        {
            var response = await _client.PostAsJsonAsync("api/Account/Register", dto);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }

    }


    public record LoginDto(string Username, string password);
    public record LoginResponse(string token);
}
