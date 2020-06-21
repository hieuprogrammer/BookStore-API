using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// HTTP 1.1 GET response to retrieve all Authors
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
                _logger.LogError($"{exception.Message} - {exception.InnerException}");
                return StatusCode(500, "Internal Server Error. Contact Hieu for further support by email: hieu.minhle@nashtechglobal.com");
            }
        }
    }
}
