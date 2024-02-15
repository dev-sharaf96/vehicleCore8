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
    public class InsuranceCompaniesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IInsuranceCompanyBusiness insuranceCompanyBusiness;

        public InsuranceCompaniesController(IUnitOfWork unitOfWork, IInsuranceCompanyBusiness insuranceCompanyBusiness)
        {
            this.unitOfWork = unitOfWork;
            this.insuranceCompanyBusiness = insuranceCompanyBusiness;
        }


        // POST api/insuranceCompanies
        [HttpPost]
        public IActionResult Post([FromBody] InsuranceCompany insuranceCompany)
        {
            insuranceCompanyBusiness.Add(insuranceCompany);
            return Ok(insuranceCompany);
        }

        // PUT api/insuranceCompanies
        [HttpPut]
        public IActionResult Put([FromBody] InsuranceCompany insuranceCompany)
        {
            insuranceCompanyBusiness.Update(insuranceCompany);
            return Ok(insuranceCompany);
        }

        // DELETE api/insuranceCompanies/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            insuranceCompanyBusiness.Remove(id);
            return Ok();
        }

        // GET api/insuranceCompanies
        [HttpGet]
        public ActionResult<IEnumerable<InsuranceType>> getInsuranceCompanies()
        {
            return Ok(insuranceCompanyBusiness.GetAll());
        }
    }
}