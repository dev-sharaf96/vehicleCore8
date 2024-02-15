using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Context;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Security.Services;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.Vehicles;
using Tameenk.Services.Logging;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Newtonsoft.Json;
using Tameenk.Services.Core.Drivers;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Core.Leasing.Models;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation;
using Tameenk.Core.Domain.Enums;
using Tameenk.Resources.WebResources;
using System.Globalization;
using Tameenk.Core.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    public class UserClaimsController : AdminBaseApiController
    {
        #region Fields
        private readonly IAuthorizationService _authorizationService;
        private readonly IAdministrationPolicyService _administrationPolicyService;
        #endregion

        #region Ctor
        public UserClaimsController(IAuthorizationService authorizationService, IAdministrationPolicyService administrationPolicyService)
        {
            _authorizationService = authorizationService;
            _administrationPolicyService = administrationPolicyService;
        }
        #endregion

        #region endpoints
        [HttpPost]
        [Route("api/Policy/GetUserClaimsByfilter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IActionResult))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetUserClaimsByfilter(ClaimsFilter model, string lang)
        {
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                currentUserId = User.Identity.GetUserId();

            var output = _administrationPolicyService.GetUserClaimsData(model, currentUserId, lang);
            return Single(output);
        }

        [HttpGet]
        [Route("api/Policy/GetAllClaimsStatus")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IActionResult))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetAllClaimsStatus(string lang)
        {
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                currentUserId = User.Identity.GetUserId();

            var output = _administrationPolicyService.GetAllClaimsStatus(currentUserId, lang);
            return Single(output);
        }

        [HttpPost]
        [Route("api/Policy/Details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IActionResult))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetUserClaimDetails(ClaimsFilter claimsFilter, string lang)
        {
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                currentUserId = User.Identity.GetUserId();

            var output = _administrationPolicyService.GetUserClaimDetails(claimsFilter, currentUserId, lang);
            return Single(output);
        }

        [HttpPost]
        [Route("api/Policy/UpdateClaims")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IActionResult))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult UpdateClaims(ClaimsUpdateModel model, string lang)
        {
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                currentUserId = User.Identity.GetUserId();

            var output = _administrationPolicyService.UpdateClaim(model, currentUserId, lang);
            return Single(output);
        }

        [HttpGet]
        [Route("api/Policy/DownloadClaimFilebyFileId")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IActionResult))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult DownloadPolicyFile(int fileId, string lang)
        {
            string currentUserId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                currentUserId = User.Identity.GetUserId();

            var output = _administrationPolicyService.DownloadClaimFilebyFileId(fileId, currentUserId, lang);
            return Single(output);
        }

        [HttpGet]
        [Route("api/Policy/GetClaimRequesterTypes")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IActionResult))]
        [AdminAuthorizeAttribute(pageNumber: 0)]
        public IActionResult GetClaimRequesterTypes()
        {
            try
            {
                LeaseOutput<List<ClaimRequesterTypeModel>> output = new LeaseOutput<List<ClaimRequesterTypeModel>>();

                output.Result =  Enum.GetValues(typeof(ClaimRequesterType)).Cast<ClaimRequesterType>().Select(c => new ClaimRequesterTypeModel
                {
                    Id = (int)c,
                    Key= c.ToString()
                    //Key = WebResources.ResourceManager.GetString(c.ToString(), CultureInfo.GetCultureInfo(lang))
                }).ToList();

                output.ErrorCode = LeaseOutput<List<ClaimRequesterTypeModel>>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return Single(output);
            }
            catch (Exception ex)
            {
                return Error(ex.ToString());
            }
        }
        #endregion
    }
}
