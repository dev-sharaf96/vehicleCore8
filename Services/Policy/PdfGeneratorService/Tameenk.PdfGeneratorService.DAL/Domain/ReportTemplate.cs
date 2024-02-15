using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PdfGeneratorService.DAL.Core.Domain
{
    public class ReportTemplate
    {
        public int Id { get; set; }

        public string ReportType { get; set; }

        public string TemplateRelativePath { get; set; }
    }
}
