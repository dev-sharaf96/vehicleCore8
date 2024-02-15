using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Security.Services;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers.Auth
{
    [RoutePrefix("api/TameenkUsers")]
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class TameenkUsersController : ApiControllerBase
    {
        private readonly IAuthorizationService userService;
        public TameenkUsersController(IAuthorizationService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult Get(string id)
        {
            // return Single(userService.GetUser(id));
            return Single(userService.GetUsers().FirstOrDefault(x => x.Id == id));
        }

        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            return Collection(userService.GetUsers());
        }

        [HttpPost]
        [Route("update")]
        public IActionResult Update([FromBody] UpdateCustomertModel user)
        {
           var result =  userService.Update(user);
            return Ok();
        }
    }
}
