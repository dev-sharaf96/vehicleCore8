using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tameenk.Api.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> LatestAppVersion(string platform)
        {
           return Single(_mobileAppVersionsRepository.Table.Where(r => r.Platform.Trim().ToLower() == platform.Trim().ToLower()).OrderByDescending(r => r.CreationDate).FirstOrDefault());
        }
    }
}
