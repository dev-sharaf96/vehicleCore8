using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Tameenk.Cancellation.BLL.Business;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        private readonly ILogger<LookupsController> logger;

        public LookupsController(ILogger<LookupsController> logger)
        {
            this.logger = logger;
        }

        // GET api/lookups/getReasons
        [HttpGet]
        [Route("getReasons")]
        public ActionResult<IEnumerable<Reason>> GetReasons([FromServices]IReasonLookup reasonLookup)
        {
            logger.LogInformation($"Method : {nameof(GetReasons)} , Message : Get Active reasons ");
            return Ok(reasonLookup.GetActive());
        }

        // GET api/lookups/getBankCodes
        [HttpGet]
        [Route("getBankCodes")]
        public ActionResult<IEnumerable<BankCode>> GetBankCodes([FromServices]IBankCodeLookup bankCodeLookup)
        {
            return Ok(bankCodeLookup.GetActive());
        }

        // GET api/lookups/getErrorCodes
        [HttpGet]
        [Route("getErrorCodes")]
        public ActionResult<IEnumerable<ErrorCode>> GetErrorCodes([FromServices]IErrorCodeLookup errorCodeLookup)
        {
            return Ok(errorCodeLookup.GetActive());
        }

        // GET api/lookups/getVehicleIDTypes
        [HttpGet]
        [Route("getVehicleIDTypes")]
        public ActionResult<IEnumerable<VehicleIDType>> GetVehicleIDTypes([FromServices]IVehicleIDTypeLookup vehicleIDTypeLookup)
        {
            return Ok(vehicleIDTypeLookup.GetActive());
        }

        // GET api/lookups/getInsuranceTypes
        [HttpGet]
        [Route("getInsuranceTypes")]
        public ActionResult<IEnumerable<InsuranceType>> GetInsuranceTypes([FromServices]IInsuranceTypeLookup insuranceTypeLookup)
        {
            return Ok(insuranceTypeLookup.GetActive());
        }

        // GET api/lookups/getInsuranceCompanies
        [HttpGet]
        [Route("getInsuranceCompanies")]
        public ActionResult<IEnumerable<InsuranceType>> getInsuranceCompanies([FromServices]IInsuranceCompanyBusiness insuranceCompanyBusiness)
        {
            return Ok(insuranceCompanyBusiness.GetActive());
        }
    }
}