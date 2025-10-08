using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SLibrary.Business.Interfaces;

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationManager _reservationMng;

        public ReservationsController(IReservationManager reservationMng)
        {
            _reservationMng = reservationMng;
        }
        
        /// <summary>
        /// Get all Reservations.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Read()
        {
            var reservations = _reservationMng.GetAllReservations();
            return Ok(reservations);

        }

        /// <summary>
        /// Get a reservation by ID.
        /// </summary>

        [HttpGet("GetByID")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = _reservationMng.GetReservationById(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }


        /// <summary>
        /// Reserve a book from the library.
        /// </summary>

        [Authorize]
        [HttpPost("Reserve")]
        public async Task<IActionResult> Create(string title, string clientname)
        {
            var result = _reservationMng.ReserveBook(title, clientname);

            if (result.Contains("Successfully"))
            {
                return Ok("Book reserved successfully");
            }
            else
            {
                return BadRequest("Book Can Not be reserved");
            }


        }

        /// <summary>
        /// Release a book from the library.
        /// </summary>
        [Authorize]
        [HttpDelete("Release")]
        public async Task<IActionResult> Delete(string title, string clientname)
        {
            var result = _reservationMng.ReleaseBook(title, clientname);

            if (result.Contains("Successfully"))
            {
                return Ok("Book released successfully");
            }
            else
            {
                return BadRequest("Book Can Not be released");
            }
        }
    }
}
