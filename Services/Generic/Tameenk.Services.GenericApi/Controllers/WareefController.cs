using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Generic.Components;
using Tameenk.Services.Generic.Components.Output;
using Tameenk.Services;

namespace Tameenk.Services.GenericApi.Controllers
{
    [AllowAnonymous]
    public class WareefController : BaseApiController
    {
        private readonly IWareefService _wareefService;

        public WareefController(IWareefService wareefService)
        {
            _wareefService = wareefService;

        }

        [HttpGet]
        [Route("api/wareef/All")]
        public IHttpActionResult GetAll()
        {
            string exception = string.Empty;
            var output =_wareefService.GatAllWareefData(out  exception );

            if (string.IsNullOrEmpty(exception))
                return Ok(output);
            else
                return Error(output);
        }
        [HttpPost]        [Route("api/wareef/CategoryItemByCategoryId")]        public IHttpActionResult CategoryItemByCategoryId(int id)        {            string exception = string.Empty;
            var output = _wareefService.GetAllWareefDataByCategryId(id, out exception);            if (string.IsNullOrEmpty(exception))                return Ok(output);            else                return Error(output);        }
        [HttpPost]        [Route("api/wareef/Category")]        public IHttpActionResult GetAllCategory()        {            string exception = string.Empty;            var output = _wareefService.GetAllCategory(out exception);            if (string.IsNullOrEmpty(exception))                return Ok(output);            else                return Error(output);        }

    }
}
