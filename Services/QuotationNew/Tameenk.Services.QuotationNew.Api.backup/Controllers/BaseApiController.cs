using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tameenk.Services.QuotationNew.Api.ActionResults;
using Tameenk.Services.QuotationNew.Api.Models;

namespace Tameenk.Services.QuotationNew.Api.Controllers
{
    [Authorize]
    public abstract class BaseApiController : ApiController
    {
        [NonAction]
        public RawJsonActionResult Ok<T>(T content, int totalCount = 0)
        {
            return new RawJsonActionResult(new QuotationNewCommonResponseModel<T>(content, totalCount).Serialize());
        }
        public IHttpActionResult Single(object result)
        {
            return base.Ok(result);
        }

        [NonAction]
        public RawJsonActionResult Error(IEnumerable<string> errors, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return Error(errors.Select(e => new QuotationNewErrorModel(e)), httpStatusCode);
        }

        [NonAction]
        public RawJsonActionResult Error(string error, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return Error(new List<string> { error }, httpStatusCode);
        }

        [NonAction]
        public RawJsonActionResult Error(IEnumerable<QuotationNewErrorModel> errors, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var commonModel = new QuotationNewCommonResponseModel<bool>(false);
            commonModel.Errors = errors;
            return new RawJsonActionResult(commonModel.Serialize(), httpStatusCode);
        }
    }
}