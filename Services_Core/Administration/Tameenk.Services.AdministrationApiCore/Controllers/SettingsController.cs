using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
   
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class SettingsController : AdminBaseApiController
    {

        #region Fields
        private readonly ISettingService settingService;

        #endregion

        #region The Ctor

        /// <summary>
        /// the constructor.
        /// </summary>
        public SettingsController(ISettingService settingService)
        {
            this.settingService = settingService ?? throw new TameenkArgumentNullException(nameof(IExcelService));
        }

        #endregion


        #region Methods


        /// <summary>
        /// Get Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/settings/get")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Setting))]
        public IActionResult GetSetting()
        {
            try
            {
                return Single(settingService.GetSetting());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
        }

        /// <summary>
        /// Save Setting
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/settings/save")]
        public IActionResult Save(Setting setting)
        {
            try
            {
                settingService.Save(setting);
                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return Error("an error has occured");
            }
        }
        #endregion

    }
}