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

        public BooksController(IBookManager bookMng, ILogger<BooksController> logger)
        {
            _bookManager = bookMng;
            _logger = logger;
        }

        /// <summary>
        /// Get all books from the library.
        /// </summary>
        //[HttpGet]
        //public async Task<IActionResult> Read()
        //{
        //    _logger.LogInformation("GET called to fetch all books.");
        //    var books =  _bookManager.GetAllBooks();
        //    _logger.LogInformation("Returned {Count} books.", books.Count);
        //    return Ok(books);
        //}

        /// <summary>
        /// Add a book to the library.
        /// </summary>
        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> Create(string name , string author)
        //{
        //    _logger.LogInformation("POST called to add a new book");
        //    CreateBookdto dto = new CreateBookdto
        //    {
        //        Title = name,
        //        Author= author
        //    };
        //    _bookManager.Add(dto);
        //    _logger.LogInformation("Book '{BookTitle}' added successfully.", name);
        //    return Ok("Book added successfully");

        //}

        /// <summary>
        /// Add a book to the library.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateBookdto dto)
        {
            _logger.LogInformation("POST called to add a new book");


            _bookManager.Add(dto);
            _logger.LogInformation("Book '{BookTitle}' added successfully.", dto.Title);

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

        [HttpGet]
        [Authorize]
        public IActionResult Read(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "",
            [FromQuery] string sortBy = "title",
            [FromQuery] string sortDirection = "asc")
        {
            var (users, totalCount) = _bookManager.GetBooksPaged(
                page, pageSize, search, sortBy, sortDirection);

            return Ok(new
            {
                items = users,
                totalCount = totalCount
            });
        }

        /// <summary>
        /// Get Availabe book count.
        /// </summary>
        [HttpGet("AvailableCount")]
        [Authorize]
        public IActionResult GetAvailableCount()
        {
            _logger.LogInformation("GET called to fetch available book count.");
            try
            {
                int count = _bookManager.GetAvailableBookCount();
                _logger.LogInformation("Returned available book count: {Count}.", count);
                return Ok(new { availableCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch available book count.");
                return StatusCode(500, "Internal server error while fetching count.");
            }
        }
    }
}