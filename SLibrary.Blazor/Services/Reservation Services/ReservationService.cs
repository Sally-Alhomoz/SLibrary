
using Shared;
using System.Net.Http.Json;

namespace SLibrary.Blazor.Services.Reservation_Services
{
    public class ReservationService : IReservationService
    {
        private readonly HttpClient _client;

        public ReservationService(HttpClient http)
        {
            _client = http;
        }

        public async Task<(List<Reservationdto> Items, int TotalCount)> Read(
            int page = 1, int pageSize = 10, string search = "", string sortBy = "clientname", string sortDirection = "asc")
        {
            var url = $"api/Reservations?page={page}&pageSize={pageSize}&search={search}&sortBy={sortBy}&sortDirection={sortDirection}";
            var response = await _client.GetFromJsonAsync<PagedResult>(url);
            return (response?.Items ?? new(), response?.TotalCount ?? 0);
        }

        public record PagedResult(List<Reservationdto> Items, int TotalCount);

        public async Task<bool> Release(string clientname, string title)
        {
            var endpoint = $"api/Reservations/Release?title={Uri.EscapeDataString(title)}&clientname={Uri.EscapeDataString(clientname)}";

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                string errormsg = await response.Content.ReadAsStringAsync();
                return false;
            }
        }

        public async Task<Clientdto> GetClientInfo(string username)
        {
            var name = Uri.EscapeDataString(username);
            var endpoint = $"api/Clients/{name}";

            var response = await _client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Clientdto>()
                    ?? throw new Exception("Received success status but client data was empty or invalid.");
            }

            else
            {
                string errorMsg = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"{errorMsg}");
            }
        }
    }
}
