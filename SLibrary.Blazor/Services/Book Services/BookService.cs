using Shared;
using System.Net.Http.Json;

namespace SLibrary.Blazor.Services.Book_Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _client;
        public BookService(HttpClient http)
        {
            _client = http;
        }

        //public async Task<List<Bookdto>> Read()
        //{
        //    var books = await _client.GetFromJsonAsync<List<Bookdto>>("/api/Books");
        //    return books;
        //}

        public async Task<(List<Bookdto> Items, int TotalCount)> Read(
            int page = 1, int pageSize = 10, string search = "", string sortBy = "title", string sortDirection = "asc")
        {
            var url = $"api/Books?page={page}&pageSize={pageSize}&search={search}&sortBy={sortBy}&sortDirection={sortDirection}";
            var response = await _client.GetFromJsonAsync<PagedResult>(url);
            return (response?.Items ?? new(), response?.TotalCount ?? 0);
        }

        public record PagedResult(List<Bookdto> Items, int TotalCount);

        public async Task Delete(int id)
        {
            var response = await _client.DeleteAsync($"api/Books?id={id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{errorMessage}");
            }
        }


        public async Task Create(Bookdto book)
        {

            var response = await _client.PostAsJsonAsync("api/Books", book);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{errorContent}");
            }
        }

        public async Task<bool> Reserve(string title, string clientname, string phoneNo, string address)
        {
            var endpoint = $"api/Reservations/Reserve?title={Uri.EscapeDataString(title)}&clientname={Uri.EscapeDataString(clientname)}&phoneNo={Uri.EscapeDataString(phoneNo)}&address={Uri.EscapeDataString(address)}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
