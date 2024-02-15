using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.ServicesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id?}")]

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id?}")]

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id?}")]

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
