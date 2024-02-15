using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tameenk.PdfGeneratorService.DAL.Core.Repository
{
    public class RepositoryConstants
    {
        private const string _reportTemplatesRelativeBasePath = "~/bin/Resources/Templates";
        private const string _generatedReportsRelativePath = "~/RuntimeTempGeneratedReports";

        public static readonly string ReportTemplatesBasePath;
        public const string DocxExtension = "docx";
        public const string PdfExtension = "pdf";
        public const string HtmlExtension = "html";
        public const string DocxWSchema = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        public static string GeneratedReportsBasePath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(
                    Path.Combine(_generatedReportsRelativePath, DateTime.Now.ToString("yyyy-MM-dd")));
            }
        }

        static RepositoryConstants()
        {
            ReportTemplatesBasePath = HttpContext.Current.Server.MapPath(_reportTemplatesRelativeBasePath);
        }
    }
}
