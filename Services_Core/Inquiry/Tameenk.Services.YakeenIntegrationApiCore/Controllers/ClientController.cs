﻿using Tameenk.Services.Logging;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Exceptions;
using Tameenk.Security.CustomAttributes;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.YakeenIntegrationApi
{
    [YakeenAuthenticationAttribute]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientServices clientServices;
        private readonly ILogger _logger;
        public ClientController(IClientServices _clientServices, ILogger logger)
        {
            clientServices = _clientServices;
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(ILogger));
        }
        [HttpPost]
        [Route("~/api/Customer/GetCustomerByNationalId")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        public IActionResult GetCustomerByNationalId(ClientRequestModel clientRequestModel)
        {
            try
            {
                var result = clientServices.GetClientInfo(clientRequestModel);
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
