using Shared;
using System.Collections.Generic;

namespace SLibraryBlazor.Services
{
    public class ReservationService : IReservationService
    {
        private readonly HttpClient _client;

        public ReservationService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Reservationdto>> Read()
        {
            var reservations = await _client.GetFromJsonAsync<List<Reservationdto>>("api/Reservations");
            return reservations;
        }

        public async Task<(bool,string)> Release(string clientname, string title)
        {
            var endpoint = $"api/Reservations/Release?title={Uri.EscapeDataString(title)}&clientname={Uri.EscapeDataString(clientname)}";

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return (true, $"Book '{title}' successfully released by {clientname}.");
            }
            else
            {
                string errormsg = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to release book: {response.StatusCode}. Details: {errormsg}");
            }
        }
    }

}
