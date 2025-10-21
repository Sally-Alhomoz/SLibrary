using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Net.Http.Headers;

namespace SLibraryMVC.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly HttpClient _client;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("api");
        }

        public async Task<IActionResult> Index()
        {
            var books = await _client.GetFromJsonAsync<List<Bookdto>>("api/Books")
                        ?? new List<Bookdto>();
            return View(books);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Title , string Author)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync($"api/Books?name={Uri.EscapeDataString(Title)}&author={Uri.EscapeDataString(Author)}",null);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Error saving book");
            return View("Create");
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Session expired. Please log in again.";
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.DeleteAsync($"api/Books?id={id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"API DELETE Failed! Status: {response.StatusCode}. Message: {errorMessage}");

                ModelState.AddModelError("", $"Error deleting book. API Status: {response.StatusCode}");
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Reserve(string title)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var clientname = User.Identity.Name;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(clientname))
            {
                TempData["ErrorMessage"] = "Session context lost. Please log in again.";
            }


            var endpoint = $"api/Reservations/Reserve?title={Uri.EscapeDataString(title)}&clientname={Uri.EscapeDataString(clientname)}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["ErrorMessage"] = "Reservation failed: " + await response.Content.ReadAsStringAsync();
            }
            else
            {
                TempData["ErrorMessage"] = $"Reservation failed (Status: {(int)response.StatusCode}).";
            }

            return RedirectToAction("Index");
        }
    }
}
