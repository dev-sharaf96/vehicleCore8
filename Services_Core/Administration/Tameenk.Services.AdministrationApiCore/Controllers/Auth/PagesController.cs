using Tameenk.Api.Core;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;
using Tameenk.Services.Administration.Identity.Core.Servicies;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers.Auth
{
    [RoutePrefix("api/pages")]
    [AdminAuthorizeAttribute(pageNumber: 10000)]
    public class PagesController : ApiControllerBase
    {
        private readonly IPageService pageService;

        public PagesController(IPageService pageService)
        {
            this.pageService = pageService;
        }

        [HttpGet]
        [Route("get/{id:int}")]
        public IActionResult Get(int id)
        {
            return Single(pageService.Get(id));
        }

        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            return Collection(pageService.GetAll());
        }

        [HttpGet]
        [Route("getActive")]
        public IActionResult getActive()
        {
            return Collection(pageService.Find(x=>x.IsActive));
        }

        [HttpPost]
        public IActionResult Add([FromBody] Page page)
        {
            pageService.Add(page);
            return Result<int>(pageService.SaveChangesAsync());
        }

        [HttpPost]
        [Route("update")]
        public IActionResult Update([FromBody] Page page)
        {
            pageService.Update(page);
            return Result<int>(pageService.SaveChangesAsync());
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        public IActionResult Delete([FromQuery] int id)
        {
            pageService.Remove(id);
            return Result<int>(pageService.SaveChangesAsync());
        }
    }
}
