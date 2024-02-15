using Newtonsoft.Json;
using System;
using Tameenk.Api.Core;
using Tameenk.Common.Utilities;
using Tameenk.Security.Encryption;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Tameenk.Services.AdministrationApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers.Auth
{
    //[Authorize]
    [RoutePrefix("api/users")]
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public IActionResult Get(int id)
        {
            return Single(userService.Get(id));
        }

        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            return Collection(userService.GetAll());
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromBody] AppUser user)
        {
            userService.Add(user);
            return Result<int>(userService.SaveChangesAsync());
        }

        [HttpPost]
        [Route("update")]
        public IActionResult Update([FromBody] AppUser user)
        {
            userService.Update(user);
            return Result<int>(userService.SaveChangesAsync());
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        public IActionResult Delete([FromQuery] int id)
        {
            userService.Remove(id);
            return Result<int>(userService.SaveChangesAsync());
        }
    }
}
