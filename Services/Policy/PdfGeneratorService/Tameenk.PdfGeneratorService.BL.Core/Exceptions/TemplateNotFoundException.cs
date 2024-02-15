using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PdfGeneratorService.BL.Core.Exceptions
{
    public class TemplateNotFoundException : Exception
    {
        public TemplateNotFoundException(string reportType)
            : base("Couldn't found template file for: " + reportType)
        {

        }
    }
}
