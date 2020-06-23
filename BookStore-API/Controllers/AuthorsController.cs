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
using NLog;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// AuthorsController interact with Authors table in MS SQL Server database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// HTTP 1.1 GET method to respond all Authors.
        /// </summary>
        /// <returns>List of all Authors.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors() 
        {
            try
            {
                _logger.LogInfo("End user sent an HTTP 1.1 GET request to retrieve all Authors!");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully responded to end user the list of all Authors!");
                return Ok(response);
            }
            catch (Exception exception)
            {
                return InternalError($"{exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 GET method to respond an Author by ID number.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Author with according ID number's record.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"End user sent an HTTP 1.1 GET request to retrieve an Author with ID number {id}!");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"Author with ID number: {id} is not found!");
                    return NotFound();
                }
                else 
                {
                    var response = _mapper.Map<AuthorDTO>(author);
                    _logger.LogInfo($"Successfully responded to end user Author information with number {id}!");
                    return Ok(response);
                }
            }
            catch (Exception exception)
            {
                return InternalError($"{exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 POST method to create a new Author.
        /// </summary>
        /// <param name="authorCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorCreateDTO) 
        {
            try
            {
                _logger.LogInfo($"End user attempted to create a new Author.");
                if (authorCreateDTO == null) 
                {
                    _logger.LogWarn($"End user sent an empty HTTP 1.1 POST request!");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid) 
                {
                    _logger.LogWarn($"End user sent an HTTP 1.1 POST request to create a new Author with invalid Author data!");
                    return BadRequest(ModelState);
                }
                var newAuthor = _mapper.Map<Author>(authorCreateDTO);
                var isSuccess = await _authorRepository.Create(newAuthor);
                if (!isSuccess) 
                {
                    return InternalError($"Failed creating new Author!");
                }
                _logger.LogInfo("Successfully created a new Author!");
                return Created("Create", new { newAuthor });
            }
            catch (Exception exception)
            {
                return InternalError($"{exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 PUT method to update an existing Author.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorUpdateDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorUpdateDTO)
        {
            try
            {
                _logger.LogInfo($"End user attempted to update an existing Author with ID number: {id}.");
                if (id < 1 || authorUpdateDTO == null || id != authorUpdateDTO.Id) 
                {
                    _logger.LogWarn($"End user sent an HTTP 1.1 POST request to create a new Author with invalid Author data!");
                    return BadRequest();
                }
                if (!ModelState.IsValid) 
                {
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorUpdateDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess) 
                {
                    return InternalError($"Failed updating Author!");
                }
                _logger.LogWarn($"Successfully updated Author with ID number {id}!");
                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalError($"{exception.Message} - {exception.InnerException}");
            }
        }

        /// <summary>
        /// HTTP 1.1 DELETE method to delete an existing Author.
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
            try
            {
                _logger.LogInfo($"End user attempted to delete existing Author with ID number: {id}.");
                if (id < 1)
                {
                    _logger.LogWarn($"Failed deleting Author with ID number {id}!");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if (!isExists) 
                {
                    _logger.LogWarn($"Author with ID number {id} is not found!");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess) 
                {
                    return InternalError($"Failed deleting Author with ID number {id}!");
                }
                _logger.LogWarn($"Successfully delete Author with ID number {id}!");
                return NoContent();
            }
            catch (Exception exception)
            {
                return InternalError($"{exception.Message} - {exception.InnerException}");
            }
        }

        private ObjectResult InternalError(string message) 
        {
            _logger.LogError(message);
            return StatusCode(500, "Internal Server Error.Contact Hieu for further support by email: hieucoder@outlook.com");
        }
    }
}
