using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Tameenk.Api.Core.ActionResults;
using Tameenk.Api.Core.Attributes;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Configuration;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.CustomAttributes;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.IVR.TicketApi
{
    [WebApiLanguage]
    //[Authorize]
    public abstract class BaseApiController : ApiController
    {
        public IHttpActionResult Single(object result)
        {
            return base.Ok(result);
        }

        public RawJsonActionResult Error(string error, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var commonModel = new CommonResponseModel<bool>(false);
            commonModel.Errors = new List<ErrorModel>() { new ErrorModel(error) };
            return new RawJsonActionResult(commonModel.Serialize(), httpStatusCode);
        }
    }
}