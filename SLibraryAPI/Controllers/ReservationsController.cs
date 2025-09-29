using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SLibrary.Business;

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly BookManager _bookManager;

        public ReservationsController(BookManager bookmng)
        {
            _bookManager = bookmng;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _bookManager.GetReservations();
            return Ok(reservations);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _bookManager.GetReservationById(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }
    }
}
