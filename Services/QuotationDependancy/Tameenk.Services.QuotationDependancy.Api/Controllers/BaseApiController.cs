using System.Web.Http;
using Tameenk.Api.Core.ActionResults;
using Tameenk.Api.Core.Models;

namespace Tameenk.Services.QuotationDependancy.Api
{
    [CustomAuthorizeAttribute]
    public abstract class BaseApiController : ApiController
    {
        [NonAction]
        public RawJsonActionResult Ok<T>(T content, int totalCount = 0)
        {
            return new RawJsonActionResult(new CommonResponseModel<T>(content, totalCount).Serialize());
        }
        public IHttpActionResult Single(object result)
        {
            return base.Ok(result);
        }
    }
}