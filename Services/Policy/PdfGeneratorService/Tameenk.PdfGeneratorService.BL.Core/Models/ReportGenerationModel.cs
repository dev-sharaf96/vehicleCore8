using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PdfGeneratorService.BL.Core.Models
{
    public class ReportGenerationModel
    {
        public string ReportType { get; set; }
        public string ReportDataAsJsonString { get; set; }
        public string TemplatePath { get; set; }
    }
}
