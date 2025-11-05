using Newtonsoft.Json.Linq;
using Shared;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SLibraryBlazor.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _client;
        public BookService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Bookdto>> Read()
        {
            var books = await _client.GetFromJsonAsync<List<Bookdto>>("/api/Books");
            return books;
        }

        public async Task Delete(int id)
        {
            var response = await _client.DeleteAsync($"api/Books?id={id}");
        }


        public async Task Create(Bookdto book)
        {
            string url = $"api/Books?name={Uri.EscapeDataString(book.Title)}&author={Uri.EscapeDataString(book.Author)}";

            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API request failed: {response.StatusCode}. Content: {errorContent}");
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
