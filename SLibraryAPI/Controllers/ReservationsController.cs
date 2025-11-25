using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared;
using SLibrary.Business.Interfaces;
using SLibrary.Business.Managers;
using System.Text.Json;

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationManager _reservationMng;
        private readonly ILogger<ReservationsController> _logger;


        public ReservationsController(IReservationManager reservationMng, ILogger<ReservationsController> logger)
        {
            _reservationMng = reservationMng;
            _logger = logger;
        }
        
        /// <summary>
        /// Get all Reservations.
        /// </summary>
        //[HttpGet]
        //public async Task<IActionResult> Read()
        //{
        //    _logger.LogInformation("GET called to fetch all reservations");
        //    var reservations = _reservationMng.GetAllReservations();
        //    _logger.LogInformation("Returned {Count} reservations.", reservations.Count);
        //    return Ok(reservations);

        //}

        /// <summary>
        /// Get a reservation by ID.
        /// </summary>

        [HttpGet("GetByID")]
        [Authorize]
        public async Task<IActionResult> GetReservation(int id)
        {
            _logger.LogInformation("GET called to fetch reservation with id : {id}",id);
            var reservation = _reservationMng.GetReservationById(id);
            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found");
                return NotFound();
            }
            _logger.LogInformation("Reservation retrieved successfully");
            return Ok(reservation);
        }


        /// <summary>
        /// Reserve a book from the library.
        /// </summary>

        [Authorize]
        [HttpPost("Reserve")]
        public async Task<IActionResult> Create([FromQuery] string title, [FromQuery] string clientname, [FromQuery] string phoneNo , [FromQuery] string address)
        {
            string username = User.Identity!.Name!;
            _logger.LogInformation("POST called to create new reservation");
            var result = _reservationMng.ReserveBook(title, clientname,username,phoneNo,address);

            if (result.Contains("Successfully"))
            {
                _logger.LogInformation("Book reserved successfully");
                return Ok("Book reserved successfully");
            }
            else if(result.Contains("Deleted"))
            {
                _logger.LogWarning("Failed - {result}", result);
                return BadRequest("Book Can Not be reserved - Deleted");
            }
            else
            {
                _logger.LogWarning("Failed - {result}", result);
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
            _logger.LogInformation("DELETE called to release a reservations with Title : {title} , Client name {clientname} ", title,clientname);
            var result = _reservationMng.ReleaseBook(title, clientname);

            if (result.Contains("Successfully"))
            {
                _logger.LogInformation("Reservation released successfully");
                return Ok("Book released successfully");
            }
            else
            {
                _logger.LogWarning("Failed to release reservations - {result}", result);
                return BadRequest("Book Can Not be released");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Read(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "reservedDate",
            [FromQuery] string sortDirection = "asc")
        {
            var (reservations, totalCount) = _reservationMng.GetReservationsPaged(
                page, pageSize, search, sortBy, sortDirection);

            return Ok(new
            {
                items = reservations,     
                totalCount                
            });
        }

        /// <summary>
        /// Get reservation for the current user.
        /// </summary>
        [HttpGet("GetUserReservation")]
        public IActionResult GetUserReservation()
        {
            string username = User.Identity!.Name!;

            if (string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized("User identity not found.");
            }

            try
            {
                int total = _reservationMng.GetTotalReservationsByUser(username);

                return Ok(new
                {
                    totalReservations = total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while fetching reservation summary.");
            }
        }


        /// <summary>
        /// Get active reservation for the current user.
        /// </summary>
        [HttpGet("GetActiveReservation")]
        public IActionResult GetActiveReservation()
        {
            string username = User.Identity!.Name!;

            if (string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized("User identity not found.");
            }

            try
            {
                int active = _reservationMng.GetActiveReservationsByUser(username);

                return Ok(new
                {
                    activeReservations = active
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while fetching reservation summary.");
            }
        }
    }
}
