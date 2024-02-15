using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Security.CustomAttributes;
using Tameenk.Services.Profile.Component;

namespace Tameenk.Services.IdentityApi.Controllers
{
    [IntegrationAuthentication]
    public class ConfirmationController : ApiController
    {
        private readonly IAuthenticationContext _AuthenticationContext;
 
        public ConfirmationController(IAuthenticationContext authenticationContext)
        {
            _AuthenticationContext = authenticationContext;
        }

        [HttpPost]
        [Route("api/Confirmation/Phone-Email")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]

        public async Task<IHttpActionResult> ConfirmPhoneAndEmail([FromBody] ConfirmModel model)
        {
              return Ok(_AuthenticationContext.ConfirmPhoneAndEmail(model));
        }
    }
}
