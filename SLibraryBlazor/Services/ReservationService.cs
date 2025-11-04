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
    }
}
