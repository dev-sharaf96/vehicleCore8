using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities; 
using Tameenk.Services.Generic.Component;
using Tameenk.Services.Generic.Components.Output;
using Tameenk.Services.Generic.Components.Models;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.Generic.Controllers
{
    public class CareerController : BaseApiController
    {
        private readonly IGenericContext _genericContext;

        public CareerController(IGenericContext genericContext)
        {
            this._genericContext = genericContext;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("api/career/post")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        public IActionResult RequestCareer(CareerModel model)
        {
            var output = new CareerOutput();
            try
            {
                if (model == null)
                {
                    output.ErrorCode = CareerOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    return Ok(output);
                }
                if (!Utilities.IsValidMail(model.Email))
                {
                    output.ErrorCode = CareerOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.InvalidEmail;
                    return Ok(output);
                }
                string exception = string.Empty;
                var res = _genericContext.SaveCareerRequest(model , User.Identity.GetUserId(), out exception);
                if (res != null)
                {
                    output.ErrorCode = CareerOutput.ErrorCodes.Success;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                }
                else
                {
                    output.ErrorCode = CareerOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                }
                return Ok(output);
            }
            catch (Exception ex)
            { 
                return Ok(ex.ToString());
            }
        }
    }
}
