using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [AdminAuthorizeAttribute(pageNumber: 0)]
    public class YakeenCityCenterController : AdminBaseApiController
    {
        #region Fields

        private readonly IYakeenCityCenterService _yakeenCityCenterService;
        private readonly IExcelService _excelService;

        #endregion

        #region CTOR

        public YakeenCityCenterController(IYakeenCityCenterService yakeenCityCenterService, IExcelService excelService)
        {
            _yakeenCityCenterService = yakeenCityCenterService ?? throw new TameenkArgumentNullException(nameof(IYakeenCityCenterService));
            _excelService = excelService ?? throw new TameenkArgumentNullException(nameof(IYakeenCityCenterService));
        }

        #endregion

        #region Methods

        /// <summary>
        /// add new yakeen city center data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/YakeenCetyCenter/new")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<PolicyOutput>))]
        public IActionResult AddNew([FromBody]YakeenCityCenter model)
        {
            try
            {
                var dbModel = new YakeenCityCenterModel()
                {
                    CityId = model.CityId,
                    CityName = model.CityName,
                    EnglishName = model.EnglishName,
                    ZipCode = model.ZipCode,
                    RegionId = model.RegionId,
                    RegionArabicName = model.RegionArabicName,
                    RegionEnglishName = model.RegionEnglishName,
                    ElmCode = model.ElmCode
                };

                var output = _yakeenCityCenterService.AddorNewYakeenCityCenter(dbModel);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Return list of all yakeen city center with filter
        /// </summary>
        /// <param name="cityId">city Id</param>
        /// <param name="cityName">city Name</param>
        /// <param name="zipCode">zip Code</param>
        /// <param name="elmCode">elm Code</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/YakeenCetyCenter/all-withFilter")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<YakeenCityCenter>>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<string>))]
        public IActionResult GetVehicleMakers(int cityId, string cityName, int zipCode, int elmCode, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var result = _yakeenCityCenterService.GetYakeenCityCentersWithFilter(out int count, false, cityId, cityName, zipCode, elmCode, pageIndex, pageSize);
                if (result == null)
                    return Ok("");

                return Ok(result.Select(res => res.ToYakeenCityCentersListingModel()), count);
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Export Yakeen City Centers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/YakeenCetyCenter/export")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<YakeenCityCenter>>))]
        public IActionResult ExportAllNewServiceRequest(int cityId, string cityName, int zipCode, int elmCode)
        {
            try
            {

                var result = _yakeenCityCenterService.GetYakeenCityCentersWithFilter(out int count, true, cityId, cityName, zipCode, elmCode, 0, 0);

                if (result == null)
                    return Ok("");

                byte[] file = _excelService.ExportYakeenCityCenters(result, "Yakeen City Centers");

                if (file != null && file.Length > 0)
                    return Ok(Convert.ToBase64String(file));
                else
                    return Ok("");
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        #endregion
    }
}