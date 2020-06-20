using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;
using NLog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;
        public HomeController(ILoggerService logger) 
        {
            this._logger = logger;
        }
        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("In Home Controller!");
            return new string[] { "value1", "value2" };
        }

        // GET api/<HomeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            _logger.LogDebug("In GET by entity's ID method!");
            return "value";
        }

        // POST api/<HomeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _logger.LogError("In POST method! This is not an error!");
        }

        // PUT api/<HomeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HomeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.LogWarn("This is a warning!");
        }
    }
}
