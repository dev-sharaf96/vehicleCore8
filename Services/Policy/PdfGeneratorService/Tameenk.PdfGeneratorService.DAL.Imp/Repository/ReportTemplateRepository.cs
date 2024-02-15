using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.PdfGeneratorService.DAL.Core.Domain;
using Tameenk.PdfGeneratorService.DAL.Core.Repository;

namespace Tameenk.PdfGeneratorService.DAL.Imp.Repository
{
    public class ReportTemplateRepository : IReportTemplateRepository
    {
        readonly IEnumerable<ReportTemplate> _reportTemplates;
        public ReportTemplateRepository(IEnumerable<ReportTemplate> reportTemplates)
        {
            _reportTemplates = reportTemplates;
        }

        public ReportTemplate GetReportTemplate(int id)
        {
            return _reportTemplates.FirstOrDefault(r => r.Id == id);
        }

        public ReportTemplate GetReportTemplateByReportType(string reportType)
        {
            return _reportTemplates.FirstOrDefault(r => r.ReportType.Equals(reportType, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
