using System;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.PdfGeneratorService.BL.Core.Models;
using Tameenk.PdfGeneratorService.BL.Core.Services;
using Tameenk.PdfGeneratorService.BL.Imp.Services;

namespace Tameenk.PdfGeneratorService.Api.Controllers
{
    public class PolicyPdfGeneratorController : ApiController
    {
        public PolicyPdfGeneratorController()
        {
        }

        // Post api/PolicyPdfGenerator
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ReportGenerationModel reportGenerationData)
        {
            IPdfGeneratorServices pdfGeneratorServices = new PdfGeneratorServices();
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
                if (reportGenerationData.ReportType.Contains("IndividualQuotationsFormTemplate")
                    || reportGenerationData.ReportType.Contains("QuotationsFormTemplate"))
                    return Ok<byte[]>(await pdfGeneratorServices.GenerateReportAutoLease(reportGenerationData));
                else
                    return Ok<byte[]>(await pdfGeneratorServices.GenerateReport(reportGenerationData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().ToString());
            }

        }
        [Route("api/generatepdftemplate")]
        [HttpPost]
        public async Task<IHttpActionResult> GeneratePdfTemplate([FromBody] ReportGenerationModel reportGenerationData)
        {
            IPdfGeneratorServices pdfGeneratorServices = new PdfGeneratorServices();
            try
            {
                if (reportGenerationData.ReportType.Contains("IndividualQuotationsFormTemplate")
                    || reportGenerationData.ReportType.Contains("QuotationsFormTemplate"))
                    return Ok<byte[]>(await pdfGeneratorServices.GenerateReportAutoLease(reportGenerationData));
                else
                    return Ok<byte[]>(await pdfGeneratorServices.GeneratePdf(reportGenerationData));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }


    }
}