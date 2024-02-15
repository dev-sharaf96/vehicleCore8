using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PdfGeneratorService.BL.Core.Exceptions
{
    public class ReportTypeNotFoundException : Exception
    {
        public ReportTypeNotFoundException(string reportType)
            : base("There's no report type with name: " + reportType)
        {

        }
    }
}
