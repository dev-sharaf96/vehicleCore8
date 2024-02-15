using Tameenk.Services.Logging;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Inquiry.Components;
using Tameenk.Security.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.YakeenIntegrationApi
{
    [YakeenAuthenticationAttribute]
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IInquiryContext inquiryContext;
        private readonly ILogger _logger;
        public AddressController(IInquiryContext _inquiryContext, ILogger logger)
        {
            inquiryContext = _inquiryContext;
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(ILogger));

        }
        [HttpGet]
        [Route("~/api/address/GetAddressByNationalId")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<InquiryResponseModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        public IActionResult GetAddressByNationalId(string nationalId, string birthDate, string externalId, string channel)
        {
            try
            {
                var result = inquiryContext.GetAddressByNationalId(nationalId, birthDate, externalId, channel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Log("GetAddressByNationalId error occured.", ex);
                return BadRequest(ex.ToString());
            }
        }
    }
}
