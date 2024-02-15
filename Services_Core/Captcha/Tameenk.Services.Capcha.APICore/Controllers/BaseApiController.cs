using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using Tameenk.Api.Core.ActionResults;
using Tameenk.Api.Core.Attributes;
using Tameenk.Api.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.Capcha.API
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        [NonAction]
        public RawJsonActionResult Ok<T>(T content, int totalCount = 0)
        {
            return new RawJsonActionResult(new CommonResponseModel<T>(content, totalCount).Serialize());
        }
        public IActionResult Single(object result)
        {
            return base.Ok(result);
        }
    }
}