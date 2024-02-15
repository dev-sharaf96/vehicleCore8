using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Tameenk.Api.Core.ActionResults;
using Tameenk.Api.Core.Attributes;
using Tameenk.Api.Core.Models;

namespace Tameenk.IVR.RenewalApi
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