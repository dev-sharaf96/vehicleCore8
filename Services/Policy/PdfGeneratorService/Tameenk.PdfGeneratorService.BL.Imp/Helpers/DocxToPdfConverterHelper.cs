using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using Tameenk.PdfGeneratorService.BL.Core.Exceptions;
using Tameenk.PdfGeneratorService.DAL.Core.Repository;

namespace Tameenk.PdfGeneratorService.BL.Imp.Helpers
{
    class DocxToPdfConverterHelper
    {
        public static async Task<string> ConvertDocxToPfd(string docxFilePath)
        {
            object readOnly = false;
            object isVisible = false;
            object objFalse = false;
            object Save = false;
            object missing = System.Reflection.Missing.Value;
            object objMiss = Type.Missing;
            object sourceFile = (object)docxFilePath;
            Document wordDocument = null;
            Application appWord = null;
            try
            {
                string pdfFilePath = Path.ChangeExtension(docxFilePath, RepositoryConstants.PdfExtension);

                appWord = new Application();
                appWord.Visible = false;
                wordDocument = appWord.Documents.Open(ref sourceFile,
                                    ref missing, ref readOnly, ref missing, ref missing,
                                    ref missing, ref missing, ref missing, ref missing, ref missing,
                                    ref missing, ref isVisible, ref missing, ref missing, ref missing, ref missing);

                wordDocument.ExportAsFixedFormat(
                            pdfFilePath,
                            ExportFormat: WdExportFormat.wdExportFormatPDF,
                            OpenAfterExport: false,
                            OptimizeFor: WdExportOptimizeFor.wdExportOptimizeForOnScreen,
                            Range: WdExportRange.wdExportAllDocument,
                            From: 0,
                            To: 0,
                            Item: WdExportItem.wdExportDocumentContent,
                            IncludeDocProps: true,
                            KeepIRM: true,
                            CreateBookmarks: WdExportCreateBookmarks.wdExportCreateWordBookmarks,
                            DocStructureTags: true,
                            BitmapMissingFonts: true,
                            UseISO19005_1: false,
                            FixedFormatExtClassPtr: ref objMiss);
               
                return pdfFilePath;
            }
            catch (Exception ex)
            {
                throw new DocxToPdfConversionFailureException(ex);
            }
            finally
            {
                if (wordDocument != null)
                    wordDocument.Close();
                if(appWord != null)
                    appWord.Quit();
            }
        }
    }
}
