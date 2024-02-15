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
    public class BankCodesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBankCodeLookup BankCodeLookup;

        public BankCodesController(IUnitOfWork unitOfWork, IBankCodeLookup BankCodeLookup)
        {
            this.unitOfWork = unitOfWork;
            this.BankCodeLookup = BankCodeLookup;
        }


        // POST api/BankCodes
        [HttpPost]
        public IActionResult Post([FromBody] BankCode BankCode)
        {
            BankCodeLookup.Add(BankCode);
            return Ok(BankCode);
        }

        // PUT api/BankCodes
        [HttpPut]
        public IActionResult Put([FromBody] BankCode BankCode)
        {
            BankCodeLookup.Update(BankCode);
            return Ok(BankCode);
        }

        // DELETE api/BankCodes/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            BankCodeLookup.Remove(id);
            return Ok();
        }

        // GET api/BankCodes
        [HttpGet]
        public ActionResult<IEnumerable<BankCode>> GetBankCodes()
        {
            return Ok(BankCodeLookup.GetAll());
        }
    }
}