using System.Collections.Generic;
using System.Linq;
using Tameenk.Api.Core;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    [RoutePrefix("api/userPages")]
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class UserPageController : ApiControllerBase
    {
        private readonly IUserPageService userPageService;

        public UserPageController(IUserPageService userPageService)
        {
            this.userPageService = userPageService;
        }

        [HttpGet]
        [Route("getUserPages/{Id:int}")]
        // [Authorize(Roles = HealthCareContext.RoleList.UserPagesView)]
        public IActionResult GetUserPages(int Id)
        {
            var data = userPageService.Find(u => u.UserId == Id && u.Page.IsActive && !u.Page.ParentId.HasValue, null, "Page.Children")
                                      .Select(x => x.Page).ToList();
           return Collection<Page>(data);
        }

        [HttpGet]
        [Route("getUserPagesIds/{Id:int}")]
        // [Authorize(Roles = HealthCareContext.RoleList.UserPagesView)]
        public IActionResult GetUserPagesIds(int Id)
        {
            var data = userPageService.Find(u => u.UserId == Id && u.Page.IsActive)
                                      .Select(x => x.PageId).ToList();
            return Collection<int>(data);
        }

        [HttpPost]
        [Route("save/{id:int}")]
        // [Authorize(Roles = HealthCareContext.RoleList.UserPagesSave)]
        public IActionResult SaveUserPages(int id, List<UserPage> UserPages)
        {
            if (ModelState.IsValid)
            {
                userPageService.SaveUserPages(id, UserPages);
                return Result<int>(userPageService.SaveChangesAsync());
            }
            return Ok(false);
        }
    }
}
