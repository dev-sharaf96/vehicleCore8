using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.PdfGeneratorService.DAL.Core.Domain;
using Tameenk.PdfGeneratorService.DAL.Core.Repository;
using Tameenk.PdfGeneratorService.DAL.Core.UnitOfWork;
using Tameenk.PdfGeneratorService.DAL.Imp.Properties;
using Tameenk.PdfGeneratorService.DAL.Imp.Repository;

namespace Tameenk.PdfGeneratorService.DAL.Imp.UnitOfWork
{
    public class PdfGeneratorUnitOfWork : IPdfGeneratorUnitOfWork
    {
        private readonly string jsonFilePath;
        public IReportTemplateRepository ReportTemplateRepository { get; private set; }

        public PdfGeneratorUnitOfWork()
        {
            jsonFilePath = HttpContext.Current.Server.MapPath(Resources.ReportTemplateJsonFilePath);
            string jsonFileContent = File.ReadAllText(jsonFilePath);
            IEnumerable<ReportTemplate> reportTemplates = JsonConvert.DeserializeObject<IEnumerable<ReportTemplate>>(jsonFileContent);

            ReportTemplateRepository = new ReportTemplateRepository(reportTemplates);
        }
    }
}
