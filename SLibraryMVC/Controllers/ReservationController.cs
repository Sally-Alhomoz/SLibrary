using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace SLibraryMVC.Controllers
{
    public class ReservationController : Controller
    {
        private readonly HttpClient _client;

        public ReservationController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("api");
        }

        public async Task<IActionResult> ReservationsList()
        {
            var reservations = await _client.GetFromJsonAsync<List<Reservationdto>>("api/Reservations");
            return View(reservations);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(string clientname , string title)
        {
            var token = HttpContext.Session.GetString("JWToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //if(clientname != User.Identity.Name)
            //{
            //    TempData["ErrorMessage"] = "Cant relase book";
            //    return Redirect("ReservationsList");
            //}

            var endpoint = $"api/Reservations/Release?title={Uri.EscapeDataString(title)}&clientname={Uri.EscapeDataString(clientname)}";

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            var response = await _client.SendAsync(request);


            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Book '{title}' successfully released by {clientname}.";
            }
            else
            {
                string errormsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Failed to release book: {response.StatusCode}. Details: {errormsg}");
            }
            return RedirectToAction("ReservationsList");
        }

        public async Task<IActionResult> ClientInfo(string clientName)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                TempData["ErrorMessage"] = "Authentication token missing. Please log in.";
                return RedirectToAction("Login", "Account"); 
            }

            if (string.IsNullOrWhiteSpace(clientName))
            {
                TempData["ErrorMessage"] = "Client name not provided.";
                return RedirectToAction("ReservationsList"); 
            }

            var endpoint = $"api/Clients/{Uri.EscapeDataString(clientName)}";

            var client = await _client.GetFromJsonAsync<Clientdto>(endpoint);

            if (client == null)
            {
                TempData["ErrorMessage"] = $"Client details for '{clientName}' not found.";
                return RedirectToAction("ReservationsList", "Reservation");
            }
            return View(client);
        }
    }
}
