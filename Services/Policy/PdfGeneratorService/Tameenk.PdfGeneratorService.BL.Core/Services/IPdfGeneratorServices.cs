using System.Threading.Tasks;
using Tameenk.PdfGeneratorService.BL.Core.Models;

namespace Tameenk.PdfGeneratorService.BL.Core.Services
{
    public interface IPdfGeneratorServices
    {
        Task<byte[]> GenerateReport(ReportGenerationModel reportGenerationModel);
        Task<byte[]> GenerateReportAutoLease(ReportGenerationModel reportGenerationModel);
        Task<byte[]> GeneratePdf(ReportGenerationModel reportGenerationModel);
    }
}
