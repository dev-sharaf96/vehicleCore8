using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Exceptions;
using Tameenk.Resources.WebResources;
using Tameenk.Services.InquiryGateway.Extensions;
using Tameenk.Services.InquiryGateway.Models;
using Tameenk.Services.InquiryGateway.Services.Core.SaudiPost;
using Tameenk.Services.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.InquiryGateway.Controllers
{
    public class SaudiPostController : BaseApiController
    {
        private readonly ISaudiPostService _saudiPostService;
        private readonly ILogger _logger;


        public SaudiPostController(ISaudiPostService saudiPostService, ILogger logger)
        {
            _saudiPostService = saudiPostService ?? throw new TameenkArgumentNullException(nameof(saudiPostService));
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(logger));

        }
        /// <summary>
        /// Get address from saudi post
        /// </summary>
        /// <returns></returns>
        [Route("api/saudi-post/address")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<SaudiPostApiResultModel>))]
        public async Task<IActionResult> GetAddress(string iqamaId)
        {
            _logger.Log($"Inguiry Api -> SaudiPostController -> GetAddress -> calling  SaudiPostService (iqamaId : {iqamaId} )", LogLevel.Info);

            var saudiPostResult = await _saudiPostService.GetAddresses(iqamaId);
            _logger.Log($"Inguiry Api -> SaudiPostController -> GetAddress -> calling  SaudiPostService result is: ({JsonConvert.SerializeObject(saudiPostResult)})", LogLevel.Info);

            var model = saudiPostResult.ToModel();
            var updatedmodel = ChangeZeroOrEmptyStreetName(model);
            return Ok(updatedmodel);
        }

        private SaudiPostApiResultModel ChangeZeroOrEmptyStreetName(SaudiPostApiResultModel model)
        {
            if(model !=null && model.Addresses !=null)
            {
                foreach(var address in model.Addresses)
                {
                    if(string.IsNullOrEmpty(address.Street) || string.IsNullOrWhiteSpace(address.Street.Trim()) || address.Street == "0")
                    {
                        address.Street = "غير معروف";//WebResources.Unknown;
                    }
                }
            }
            return model;
        }
    }
}
