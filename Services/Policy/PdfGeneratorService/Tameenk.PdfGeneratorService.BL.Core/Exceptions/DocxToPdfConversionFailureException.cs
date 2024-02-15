using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PdfGeneratorService.BL.Core.Exceptions
{
    public class DocxToPdfConversionFailureException : Exception
    {
        public DocxToPdfConversionFailureException(Exception ex)
            : base("Error converting generated document to pdf", ex)
        {

        }
    }
}
