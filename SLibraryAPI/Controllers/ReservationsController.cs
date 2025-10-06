using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using SLibrary.Business;
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
        public async Task<IActionResult> GetReservations()
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
    }
}
