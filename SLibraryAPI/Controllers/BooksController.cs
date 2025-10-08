using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
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
        public async Task<IActionResult> Read()
        {
            var books =  _bookManager.GetAllBooks();
            return Ok(books);
        }

        /// <summary>
        /// Add a book to the library.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(string name , string author)
        {
            CreateBookdto dto = new CreateBookdto
            {
                Title = name,
                Author= author
            };
            _bookManager.Add(dto);
            return Ok("Book added successfully");

        }


        /// <summary>
        /// Delete a book to the library.
        /// </summary>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string result = _bookManager.Delete(id);

            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);

        }

    }
}
