using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.PdfGeneratorService.DAL.Core.Repository;

namespace Tameenk.PdfGeneratorService.DAL.Core.UnitOfWork
{
    public interface IPdfGeneratorUnitOfWork
    {
        IReportTemplateRepository ReportTemplateRepository { get; }
    }
}
