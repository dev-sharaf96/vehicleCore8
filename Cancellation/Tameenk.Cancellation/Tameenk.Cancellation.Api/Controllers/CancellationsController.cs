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
    public class CancellationsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICancellationRequestBusiness CancellationRequestBusiness;
        private readonly ILogger logger;

        public CancellationsController(IUnitOfWork unitOfWork, ICancellationRequestBusiness CancellationRequestBusiness,
            ILogger<CancellationsController> logger )
        {
            this.unitOfWork = unitOfWork;
            this.CancellationRequestBusiness = CancellationRequestBusiness;
            this.logger = logger;
        }

        // POST api/cancellations/
        [HttpPost]
        [Route("getActivePolicies")]
        public IActionResult GetActivePolicies([FromBody] CancellationRequest  CancellationRequest)
        {
            logger.LogInformation($"Type:HttpPost , Method: {nameof(GetActivePolicies)} , Message:Get Active Policies");
            var policies = CancellationRequestBusiness.GetActivePolicies(CancellationRequest);
            return Ok(policies);
        }

        [HttpPost]
        [Route("policyCancellation ")]
        public IActionResult PolicyCancellation([FromBody] CancellationRequest CancellationRequest)
        {
            logger.LogInformation($"Type:HttpPost , Method: {nameof(PolicyCancellation)} , Message:User Post Policy Cancellation");
            var policies = CancellationRequestBusiness.GetActivePolicies(CancellationRequest);
            return Ok(policies);
        }
    }
}