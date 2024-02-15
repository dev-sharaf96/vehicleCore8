using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.IdentityApi.Controllers
{
    public class MobileController : BaseApiController
    {
        private readonly IRepository<MobileAppVersions> _mobileAppVersionsRepository;

        public MobileController(IRepository<MobileAppVersions> mobileAppVersionsRepository)
        {
            _mobileAppVersionsRepository = mobileAppVersionsRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/mobile/LatestAppVersion")]
        public async Task<IHttpActionResult> LatestAppVersion(string platform)
        {
           return Single(_mobileAppVersionsRepository.Table.Where(r => r.Platform.Trim().ToLower() == platform.Trim().ToLower()).OrderByDescending(r => r.CreationDate).FirstOrDefault());
        }
    }
}
