using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Microsoft.AspNet.Identity;
using Tameenk.Services.Generic.Component;
using Tameenk.Services.Generic.Components.Output;
using Tameenk.Services.Generic.Components.Models;

namespace Tameenk.Services.Generic.Controllers
{
    public class ContactUsController : BaseApiController
    {
        private readonly IGenericContext _genericContext;

        public ContactUsController(IGenericContext genericContext)
        {
            this._genericContext = genericContext;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("api/contact-us/post")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<ErrorModel>))]
        public IHttpActionResult RequestContactUs(ContactUsModel model)
        {
            var output = new ContactUsOutput();
            try
            {
                if (model == null )
                {
                    output.ErrorCode = ContactUsOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.Checkout.CheckoutResources.InvalidData;
                    return Ok(output);
                }
                if (!Utilities.IsValidMail(model.Email))
                {
                    output.ErrorCode = ContactUsOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.InvalidEmail;
                    return Ok(output);
                }
                var contactus = new ContactUs();
                contactus.Message = model.Message;
                contactus.Nin = model.Nin;
                contactus.Email = model.Email;
                contactus.CreatedDateTime = DateTime.Now;
                contactus.ServerIp = ServicesUtilities.GetServerIP();
                contactus.Channel = model.Channel.ToString();
                contactus.UserIP = Utilities.GetUserIPAddress();
                contactus.UserAgent = Utilities.GetUserAgent();
                contactus.Createdby = User.Identity.GetUserId();
                var res = _genericContext.SaveContactUsRequest(contactus);
                if (res.Id > 0)
                {
                    output.ErrorCode = ContactUsOutput.ErrorCodes.Success;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Success;
                }
                else
                {
                    output.ErrorCode = ContactUsOutput.ErrorCodes.Failed;
                    output.ErrorDescription = Resources.ContactUs.ContactUsResource.Failed;
                }
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
