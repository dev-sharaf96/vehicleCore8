using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.PdfGeneratorService.DAL.Core.Domain;

namespace Tameenk.PdfGeneratorService.DAL.Core.Repository
{
    public interface IReportTemplateRepository
    {
        ReportTemplate GetReportTemplate(int id);
        ReportTemplate GetReportTemplateByReportType(string reportType);
    }
}
