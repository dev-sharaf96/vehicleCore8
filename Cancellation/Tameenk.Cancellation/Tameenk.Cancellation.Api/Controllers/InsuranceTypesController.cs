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
    public class InsuranceTypesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IInsuranceTypeLookup InsuranceTypeLookup;

        public InsuranceTypesController(IUnitOfWork unitOfWork, IInsuranceTypeLookup InsuranceTypeLookup)
        {
            this.unitOfWork = unitOfWork;
            this.InsuranceTypeLookup = InsuranceTypeLookup;
        }


        // POST api/InsuranceTypes
        [HttpPost]
        public IActionResult Post([FromBody] InsuranceType InsuranceType)
        {
            InsuranceTypeLookup.Add(InsuranceType);
            return Ok(InsuranceType);
        }

        // PUT api/InsuranceTypes
        [HttpPut]
        public IActionResult Put([FromBody] InsuranceType InsuranceType)
        {
            InsuranceTypeLookup.Update(InsuranceType);
            return Ok(InsuranceType);
        }

        // DELETE api/InsuranceTypes/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            InsuranceTypeLookup.Remove(id);
            return Ok();
        }

        // GET api/InsuranceTypes
        [HttpGet]
        public ActionResult<IEnumerable<InsuranceType>> GetInsuranceTypes()
        {
            return Ok(InsuranceTypeLookup.GetAll());
        }
    }
}