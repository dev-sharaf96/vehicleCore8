using System;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.PdfGeneratorService.BL.Core.Models;
using Tameenk.PdfGeneratorService.BL.Core.Services;

namespace Tameenk.PdfGeneratorService.Api.Controllers
{
    public class PolicyPdfGeneratorController : ApiController
    {
        private readonly IPdfGeneratorServices _pdfGeneratorServices;
        public PolicyPdfGeneratorController(IPdfGeneratorServices pdfGeneratorServices)
        {
            _pdfGeneratorServices = pdfGeneratorServices;
        }

        // Post api/PolicyPdfGenerator
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ReportGenerationModel reportGenerationData)
        {
            //var reportGenerationModel = new ReportGenerationModel()
            //{
            //    Username = username,
            //    ReportType = reportType,
            //    ReportDataAsJsonString = reportDataAsJson
            //};
            //var generatedPdfFilePath = await _pdfGeneratorServices.GenerateReport(reportGenerationModel);
            //if (!string.IsNullOrEmpty(generatedPdfFilePath))
            //    return new FileActionResult(Request, generatedPdfFilePath);

            //return InternalServerError();
            try
            {
                return Ok<byte[]>(await _pdfGeneratorServices.GenerateReport(reportGenerationData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().ToString());
            }

        }

        //[HttpGet]
        //public IHttpActionResult HTMLtoPDF()
        //{
        //    _pdfGeneratorServices.HTMLtoPDF();
        //    return Ok();
        //}
    }
}