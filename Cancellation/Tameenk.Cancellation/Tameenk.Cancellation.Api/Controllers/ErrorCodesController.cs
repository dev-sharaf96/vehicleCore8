using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tameenk.Cancellation.BLL.Business;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorCodesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IErrorCodeLookup ErrorCodeLookup;

        public ErrorCodesController(IUnitOfWork unitOfWork, IErrorCodeLookup ErrorCodeLookup)
        {
            this.unitOfWork = unitOfWork;
            this.ErrorCodeLookup = ErrorCodeLookup;
        }


        // POST api/ErrorCodes
        [HttpPost]
        public IActionResult Post([FromBody] ErrorCode ErrorCode)
        {
            ErrorCodeLookup.Add(ErrorCode);
            return Ok(ErrorCode);
        }

        // PUT api/ErrorCodes
        [HttpPut]
        public IActionResult Put([FromBody] ErrorCode ErrorCode)
        {
            ErrorCodeLookup.Update(ErrorCode);
            return Ok(ErrorCode);
        }

        // DELETE api/ErrorCodes/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            ErrorCodeLookup.Remove(id);
            return Ok();
        }

        // GET api/ErrorCodes
        [HttpGet]
        public ActionResult<IEnumerable<ErrorCode>> GetErrorCodes()
        {
            return Ok(ErrorCodeLookup.GetAll());
        }
    }
}