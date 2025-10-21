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
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookManager bookMng , ILogger<BooksController> logger)
        {
            _bookManager = bookMng;
            _logger = logger;
        }

        /// <summary>
        /// Get all books from the library.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Read()
        {
            _logger.LogInformation("GET called to fetch all books.");
            var books =  _bookManager.GetAllBooks();
            _logger.LogInformation("Returned {Count} books.", books.Count);
            return Ok(books);
        }

        /// <summary>
        /// Add a book to the library.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(string name , string author)
        {
            _logger.LogInformation("POST called to add a new book with Title: {BookTitle}, Author: {BookAuthor}", name, author);
            CreateBookdto dto = new CreateBookdto
            {
                Title = name,
                Author= author
            };
            _bookManager.Add(dto);
            _logger.LogInformation("Book '{BookTitle}' added successfully.", name);
            return Ok("Book added successfully");

        }


        /// <summary>
        /// Delete a book to the library.
        /// </summary>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE called to delete a book with id : {id} ", id);
            string result = _bookManager.Delete(id);

            if (result.Contains("successfully"))
            {
                _logger.LogInformation("book with id : {id} deleted successfully", id);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Failed to delete book with id : {id} - {result}", id, result);
                return BadRequest(result);
            }
        }

    }
}
