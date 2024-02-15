using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
// using System.Web.Http;

namespace Tameenk.Services.QuotationNew.Api.ActionResults
{
    public class RawJsonActionResult :  ActionResult
    {

        #region Ctor
        public RawJsonActionResult(string jsonString, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            JsonString = jsonString;
            HttpStatus = httpStatusCode;
        }
        #endregion

        #region Methods
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var content = new StringContent(JsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = new HttpResponseMessage(HttpStatus) { Content = content };
            return Task.FromResult(response);
        }

        public string JsonString {
            get;
            private set;
        }

        public HttpStatusCode HttpStatus
        {
            get;
            private set;
        }
        #endregion
    }
}