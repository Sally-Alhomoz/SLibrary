using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using SLibrary.Business;
using SLibrary.Business.Interfaces;

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookManager _bookManager;
        private readonly IReservationManager _reservationMng;
        public BooksController(IBookManager bookMng , IReservationManager reservationMng)
        {
            _bookManager = bookMng;
            _reservationMng = reservationMng;
        }

        /// <summary>
        /// Get all books from the library.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books =  _bookManager.GetAllBooks();
            return Ok(books);
        }

        /// <summary>
        /// Reserve a book from the library.
        /// </summary>

        [Authorize]
        [HttpPut("Reserve")]
        public async Task<IActionResult> ReserveBook(string title, string clientname)
        {
            var result = _reservationMng.ReserveBook(title, clientname);

            if(result.Contains("Successfully"))
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
        [HttpPut("Release")]
        public async Task<IActionResult> ReleaseBook(string title, string clientname)
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

        /// <summary>
        /// Add a book to the library.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBook(string name , string author)
        {
            CreateBookdto dto = new CreateBookdto
            {
                Title = name,
                Author= author
            };
            _bookManager.Add(dto);
            return Ok("Book added successfully");

        }

    }
}
