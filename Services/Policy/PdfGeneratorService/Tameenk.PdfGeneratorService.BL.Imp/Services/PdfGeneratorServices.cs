using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Tameenk.PdfGeneratorService.BL.Core.Exceptions;
using Tameenk.PdfGeneratorService.BL.Core.Models;
using Tameenk.PdfGeneratorService.BL.Core.Services;
using Tameenk.PdfGeneratorService.BL.Imp.Helpers;
using Tameenk.PdfGeneratorService.DAL.Core.Repository;
using Tameenk.PdfGeneratorService.DAL.Core.UnitOfWork;
using Tameenk.PdfGeneratorService.DAL.Imp.UnitOfWork;

namespace Tameenk.PdfGeneratorService.BL.Imp.Services
{
    public class PdfGeneratorServices : IPdfGeneratorServices
    {
        public PdfGeneratorServices()
        {
        }

        public async Task<byte[]> GenerateReport(ReportGenerationModel reportGenerationModel)
        {
            IPdfGeneratorUnitOfWork reportTemplatesUow = new PdfGeneratorUnitOfWork();
            var reportTemplate = reportTemplatesUow.ReportTemplateRepository
                .GetReportTemplateByReportType(reportGenerationModel.ReportType);

            if (reportTemplate == null)
                throw new ReportTypeNotFoundException(reportGenerationModel.ReportType);

            if (string.IsNullOrEmpty(reportTemplate.TemplateRelativePath))
                throw new TemplateNotFoundException(reportGenerationModel.ReportType);

            var reportTemplatePath = Path.Combine(RepositoryConstants.ReportTemplatesBasePath, reportTemplate.TemplateRelativePath);

            var generatedReortFilePath = await DocxGeneratorHelper.GenerateDocxReport(reportTemplatePath,
                reportGenerationModel.ReportType, reportGenerationModel.ReportDataAsJsonString);
            var generatedPdfFilePath = await DocxToPdfConverterHelper.ConvertDocxToPfd(generatedReortFilePath);

            var generatedPdfBytes = GenerateFileAndDeleteTempFile(generatedPdfFilePath, generatedReortFilePath, "GenerateReport");
            return generatedPdfBytes;
        }

        public async Task<byte[]> GenerateReportAutoLease(ReportGenerationModel reportGenerationModel)
        {
            IPdfGeneratorUnitOfWork reportTemplatesUow = new PdfGeneratorUnitOfWork();
            var reportTemplate = reportTemplatesUow.ReportTemplateRepository
                .GetReportTemplateByReportType(reportGenerationModel.ReportType);

            if (reportTemplate == null)
                throw new ReportTypeNotFoundException(reportGenerationModel.ReportType);

            if (string.IsNullOrEmpty(reportTemplate.TemplateRelativePath))
                throw new TemplateNotFoundException(reportGenerationModel.ReportType);

            var reportTemplatePath = Path.Combine(RepositoryConstants.ReportTemplatesBasePath, reportTemplate.TemplateRelativePath);
            var generatedReortFilePath = await AutoLeasingDocxGeneratorHelper.GenerateDocxReport(reportTemplatePath,
                reportGenerationModel.ReportType, reportGenerationModel.ReportDataAsJsonString);
            var generatedPdfFilePath = await DocxToPdfConverterHelper.ConvertDocxToPfd(generatedReortFilePath);

            var generatedPdfBytes = GenerateFileAndDeleteTempFile(generatedPdfFilePath, generatedReortFilePath, "GenerateReportAutoLease");
            return generatedPdfBytes;
        }

        public async Task<byte[]> GeneratePdf(ReportGenerationModel reportGenerationModel)
        {
            IPdfGeneratorUnitOfWork reportTemplatesUow = new PdfGeneratorUnitOfWork();
            if (reportGenerationModel == null)
                throw new ReportTypeNotFoundException("reportGenerationModel is null");
            if (string.IsNullOrEmpty(reportGenerationModel.ReportType))
                throw new ReportTypeNotFoundException("reportGenerationModel.ReportType is null or empty");
           // var reportTemplatePath = Path.Combine(RepositoryConstants.ReportTemplatesBasePath, reportGenerationModel.ReportType);
            var generatedReortFilePath = await DocxGeneratorHelper.GenerateDocxReport(reportGenerationModel.TemplatePath,
                reportGenerationModel.ReportType, reportGenerationModel.ReportDataAsJsonString);
            var generatedPdfFilePath = await DocxToPdfConverterHelper.ConvertDocxToPfd(generatedReortFilePath);

            var generatedPdfBytes = GenerateFileAndDeleteTempFile(generatedPdfFilePath, generatedReortFilePath, "GeneratePdf");
            return generatedPdfBytes;
        }

        private byte[] GenerateFileAndDeleteTempFile(string pdfPath, string wordPath, string method)
        {
            try
            {
                if (string.IsNullOrEmpty(pdfPath))
                {
                    System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\PdfGeneratorServiceApi\logs\DelteTempFile_" + method + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_Empty.txt", "Path is empty");
                    return null;
                }

                var generatedPdfBytes = File.ReadAllBytes(pdfPath);
                File.Delete(wordPath);
                File.Delete(pdfPath);

                return generatedPdfBytes;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\PdfGeneratorServiceApi\logs\DelteTempFile_" + method + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + "_Exception.txt", ex.ToString());
                return null;
            }
        }
    }
}
