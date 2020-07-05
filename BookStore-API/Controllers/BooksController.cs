using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Books controller interacts with Books table in MS SQL Server database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, ILoggerService logger, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// HTTP 1.1 GET method to respond all Books.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks() 
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo("End user sent an HTTP 1.1 GET request to retrieve all Books!");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo("Successfully responded to end user the list of all Books!");
                return Ok(response);
            }
            catch (Exception exception)
            {
                return InternalError($"{location}: {exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 GET method to respond an Book by ID number.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id) 
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"End user sent an HTTP 1.1 GET request to retrieve an Book with ID number {id}!");
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"Book with ID number: {id} is not found!");
                    return NotFound();
                }
                else
                {
                    var response = _mapper.Map<BookDTO>(book);
                    _logger.LogInfo($"Successfully responded to end user Book information with number {id}!");
                    return Ok(response);
                }
            }
            catch (Exception exception)
            {
                return InternalError($"{location}: {exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 POST method to create a new Book.
        /// </summary>
        /// <param name="bookCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookCreateDTO) 
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"End user attempted to create a new Book.");
                if (bookCreateDTO == null)
                {
                    _logger.LogWarn($"End user sent an empty HTTP 1.1 POST request!");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"End user sent an empty HTTP 1.1 POST request to create a new Book with invalid Book data!");
                    return BadRequest(ModelState);
                }
                var newBook = _mapper.Map<Book>(bookCreateDTO);
                var isSuccess = await _bookRepository.Create(newBook);
                if (!isSuccess)
                {
                    return InternalError($"Failed creating new Book!");
                }
                _logger.LogInfo("Successfully created a new Book!");
                return Created("Create", new { newBook });
            }
            catch (Exception exception)
            {
                return InternalError($"{location}: {exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 PUT method to update an existing Book.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookUpdateDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"End user attempted to an update existing Book with ID number: {id}.");
                if (id < 1 || bookUpdateDTO == null || id != bookUpdateDTO.Id)
                {
                    _logger.LogWarn($"End user sent an HTTP 1.1 POST request to create a new Book with invalid Book data!");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"End user sent an HTTP 1.1 POST request to create a new Book with invalid Book data!");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookUpdateDTO);
                var isSuccess = await _bookRepository.Update(book);
                if (!isSuccess)
                {
                    _logger.LogWarn($"End user sent an HTTP 1.1 POST request to create a new Book with invalid Book data!");
                    return InternalError($"Failed updating Author!");
                }
                _logger.LogWarn($"{location}: Successfully updated Book with ID number {id}!");
                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalError($"{location}: {exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 DELETE method to delete an existing Book.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"End user attempted to delete existing Book with ID number: {id}.");
                if (id < 1)
                {
                    _logger.LogWarn($"Failed deleting Book with ID number {id}!");
                    return BadRequest();
                }
                var isExists = await _bookRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Book with ID number {id} is not found!");
                    return NotFound();
                }
                var book = await _bookRepository.FindById(id);
                var isSuccess = await _bookRepository.Delete(book);
                if (!isSuccess)
                {
                    return InternalError($"Failed deleting Book with ID number {id}!");
                }
                _logger.LogWarn($"Successfully delete Book with ID number {id}!");
                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalError($"{location}: {exception.Message} - {exception.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Internal Server Error. Contact Hieu for support by email: hieucoder@outlook.com.");
        }
    }
}
