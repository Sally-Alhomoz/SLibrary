using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using SLibrary.Business;

namespace SLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookManager _bookManager;
        public BooksController(BookManager bookMng)
        {
            _bookManager = bookMng;
        }

        /// <summary>
        /// Get all books from the library.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _bookManager.GetBooks();
            return Ok(books);
        }

        /// <summary>
        /// Reserve a book from the library.
        /// </summary>

        [HttpPut("Reserve")]
        public async Task<IActionResult> ReserveBook(int bookid, string clientname)
        {
            var result = await _bookManager.Reserve(bookid, clientname);

            if (result == false)
            {
                return BadRequest("Book Can Not be reserved");
            }

            return Ok("Book reserved successfully");
        }

        /// <summary>
        /// Release a book from the library.
        /// </summary>

        [HttpPut("Release")]
        public async Task<IActionResult> ReleaseBook(int bookid, string clientname)
        {
            var result = await _bookManager.Release(bookid, clientname);

            if (result == false)
            {
                return BadRequest("Book Can Not be released");
            }

            return Ok("Book released successfully");
        }

        /// <summary>
        /// Add a book to the library.
        /// </summary>

        [HttpPost("Add")]
        public async Task<IActionResult> AddBook(string name , string author)
        {
            await _bookManager.Addbook(name, author);
            return Ok("Book added successfully");

        }

    }
}
