using Tameenk.Api.Core;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.Administration.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers.Auth
{
    //[Authorize]
    [RoutePrefix("api/identityLogs")]
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class IdentityLogsController : ApiControllerBase
    {
        private readonly IdentityLogService identityLogService;

        public IdentityLogsController(IdentityLogService identityLogService)
        {
            this.identityLogService = identityLogService;
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public IActionResult Get(int id)
        {
            return Single(identityLogService.Get(id));
        }

        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            return Collection(identityLogService.GetAll());
        }
    }
}
