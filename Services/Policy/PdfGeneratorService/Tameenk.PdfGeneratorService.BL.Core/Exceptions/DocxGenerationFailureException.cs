using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PdfGeneratorService.BL.Core.Exceptions
{
    public class DocxGenerationFailureException : Exception
    {
        public DocxGenerationFailureException(Exception ex)
            : base("Error generating docx file from template", ex)
        {

        }
    }
}
