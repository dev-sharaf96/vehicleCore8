using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Services.QuotationDependancy.Component;

namespace Tameenk.Services.QuotationDependancy.Api
{
    public class CompaniesController : BaseApiController
    {
        public ICompaniesGradesService _companiesGradesService { get; }

        public CompaniesController(ICompaniesGradesService companiesGradesService)
        {
            _companiesGradesService = companiesGradesService;
        }

        [HttpGet]
        [Route("api/getAllCompanies")]
        public async Task<IHttpActionResult> GetAllCompany()
        {
            var result = await _companiesGradesService.GetCompaniesWithGrades();
            return Ok(result);
        }
    }
}
