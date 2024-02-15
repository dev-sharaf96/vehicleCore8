using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tameenk.Cancellation.BLL.Business;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IReasonLookup reasonLookup;
        private readonly ILogger logger;

        public ReasonsController(IUnitOfWork unitOfWork, IReasonLookup reasonLookup , ILogger<ReasonsController> logger)
        {
            this.unitOfWork = unitOfWork;
            this.reasonLookup = reasonLookup;
            this.logger = logger;
        }


        // POST api/Reasons
        [HttpPost]
        public IActionResult Post([FromBody] Reason reason)
        {
            logger.LogInformation($"Method : {nameof(Post)} , Message : Post reason ");
            reasonLookup.Add(reason);
            return Ok(reason);
        }

        // PUT api/Reasons
        [HttpPut]
        public IActionResult Put([FromBody] Reason reason)
        {
            reasonLookup.Update(reason);
            return Ok(reason);
        }

        // DELETE api/Reasons/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            reasonLookup.Remove(id);
            return Ok();
        }

        // GET api/Reasons
        [HttpGet]
        public ActionResult<IEnumerable<Reason>> GetReasons()
        {
            return Ok(reasonLookup.GetAll());
        }
    }
}