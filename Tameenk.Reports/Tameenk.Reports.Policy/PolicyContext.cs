using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Notifications.Models;
using Tameenk.Services.Implementation.Notifications;

namespace Tameenk.Reports.Policy
{
    
    public class PolicyContext
    {


        public  void ExportPolicyExcelThenSendMail()
        {
            string SPREADSHEET_NAME = ExportPolicyReportToExcel();
            DateTime dt = DateTime.Now.AddDays(-1);

            if (File.Exists(SPREADSHEET_NAME))
            {
                FileStream file = new FileStream(SPREADSHEET_NAME, FileMode.Open);
              //  SendFileViaEmail(file);
               SendMailFromAdminIncludingMailTemplate(file, "policy reports " + dt.ToString("dd-MM-yyyy") + ".xlsx", dt.ToString("dd-MM-yyyy"));
                file.Dispose();
                file.Close();
            }
            return;
        }




        /// <summary>
        /// Exports the report to excel.
        /// </summary>
        public  string ExportPolicyReportToExcel()
        {
            var PolicyList = PolicyRequestLogDataAccess.GetPolicyList(3 * 60 * 60);
            //ErrorLogging logging = new ErrorLogging();
            //logging.LogDebug("ExportReportToExcelRechargeCreditCardXLSX");
            try
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PolicyReport" + dt.ToString("dd-MM-yyyy"));
                SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
                using (var workbook = SpreadsheetDocument.Create(SPREADSHEET_NAME, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = workbook.AddWorkbookPart();
                    {

                        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                        var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                        sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                        workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                        workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                        DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                        uint sheetId = 1;
                        if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                        {
                            sheetId =
                                sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PolicysReport" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<string> columns = new List<string>();

                        var propertyNames = typeof(PolicyRequestLog).GetProperties().Select(x => x.Name).ToList();

                        foreach (var name in propertyNames)
                        {
                            columns.Add(name);
                            AddCellHeader(name, headerRow);
                        }

                        sheetData.AppendChild(headerRow);

                        int success = 0;
                        int fail = 0;

                        foreach (var Policy in PolicyList)
                        {

                            if (Policy.ErrorCode == 1)
                                success++;
                            else
                                fail++;

                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                            foreach (string col in columns)
                            {
                                if (col == "ID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.ID.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.CreatedDate.Value.ToString("dd-MM-yyyy")); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "UserId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.UserId.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "UserName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.UserName); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "UserIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.UserIP); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "UserAgent")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.UserAgent); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "RequestID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.RequestID.HasValue ? Policy.RequestID.Value.ToString() : ""); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "ServerIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.ServerIP); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.CompanyName); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "ErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.ErrorCode.HasValue ? Policy.ErrorCode.Value.ToString() : ""); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "ErrorDescription")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.ErrorDescription); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "QuotationNo")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.QuotationNo); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "ProductID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.ProductID.HasValue ? Policy.ProductID.Value.ToString() : ""); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredID); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredMobileNumber")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredMobileNumber); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredEmail")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredEmail); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredCity")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredCity); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredCity")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredCity); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredAddress")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredAddress); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "PaymentMethod")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.PaymentMethod); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "PaymentAmount")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.PaymentAmount.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "PaymentBillNumber")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.PaymentBillNumber); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredBankCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredBankCode); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredBankName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredBankName); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InsuredIBAN")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredIBAN); //
                                    newRow.AppendChild(cell);
                                }
                            }

                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Row newRow4 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow5 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow6 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                        DocumentFormat.OpenXml.Spreadsheet.Cell cell27 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell28 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell29 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell30 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell31 = new DocumentFormat.OpenXml.Spreadsheet.Cell();


                        newRow4.Append(cell27);
                        newRow4.Append(cell28);
                        newRow4.Append(cell29);
                        newRow4.Append(cell30);
                        newRow4.Append(cell31);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell32 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell32.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell32.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number of Transactions");
                        newRow4.Append(cell32);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(PolicyList.Count.ToString());
                        newRow4.Append(cell33);

                        sheetData.AppendChild(newRow5);
                        sheetData.AppendChild(newRow6);

                        sheetData.AppendChild(newRow4);


                        DocumentFormat.OpenXml.Spreadsheet.Row newRow1 = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell23 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell24 = new DocumentFormat.OpenXml.Spreadsheet.Cell();


                        newRow1.Append(cell20);
                        newRow1.Append(cell21);
                        newRow1.Append(cell22);
                        newRow1.Append(cell23);
                        newRow1.Append(cell24);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell25 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell25.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number Of Success");
                        newRow1.Append(cell25);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell26 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell26.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell26.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(success.ToString());
                        newRow1.Append(cell26);

                        sheetData.AppendChild(newRow1);


                        // Number of Fail Request ... 

                        DocumentFormat.OpenXml.Spreadsheet.Row newRow2 = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell80 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell81 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell82 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell83 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell84 = new DocumentFormat.OpenXml.Spreadsheet.Cell();


                        newRow2.Append(cell80);
                        newRow2.Append(cell81);
                        newRow2.Append(cell82);
                        newRow2.Append(cell83);
                        newRow2.Append(cell84);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell85 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell85.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell85.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number Of Fail");
                        newRow2.Append(cell85);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell86 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell86.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell86.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(fail.ToString());
                        newRow2.Append(cell86);

                        sheetData.AppendChild(newRow2);


                        workbook.WorkbookPart.Workbook.Save();
                    }

                    workbook.Close();
                }

                return SPREADSHEET_NAME;

            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }


        private static void AddCellHeader(string Name, DocumentFormat.OpenXml.Spreadsheet.Row headerRow)
        {
            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Name);
            headerRow.AppendChild(cell);
        }

        static void DeleteOldFiles()
        {
            string rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            string filesToDelete = @"*.xlsx";   // Only delete DOC files containing "DeleteMe" in their filenames
            string[] fileList = Directory.GetFiles(rootFolderPath, filesToDelete);
            foreach (string file in fileList)
            {
                System.IO.File.Delete(file);
            }
        }

    
       


        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }



        public  bool SendMailFromAdminIncludingMailTemplate(FileStream attachment, string attachmentName, string dt)
        {
          
            ErrorLogger.LogDebug("SendMailFromAdminIncludingMailTemplate");
            try
            {
                string mailFrom = Utilities.GetAppSetting("MailFrom");
                string mailTo = Utilities.GetAppSetting("MailTo");
                string[] ToMails = mailTo.Split(';');

                System.Net.Mail.MailAddress adminMail = new System.Net.Mail.MailAddress(mailFrom);
                System.Net.Mail.MailAddressCollection toCollection = new System.Net.Mail.MailAddressCollection();
                for (int j = 0; j < ToMails.Length; j++)
                {
                    //logging.LogDebug(ToMails[j]);
                    toCollection.Add(ToMails[j]);
                }
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;

                var body = PolicyServiceReportResource.MailBody.Replace("[%DateTime%]", dt);
                body = body.Replace("[%SiteUrl%]", "https://www.bcare.com.sa/");

                mail.Body = body;
                mail.Subject = PolicyServiceReportResource.MailSubject.Replace("[%DateTime%]", dt);


                for (int i = 0; i < toCollection.Count; i++)
                {
                    mail.To.Add(toCollection[i]);
                }

                mail.From = adminMail;
                if (attachment != null)
                {
                    System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(attachment, attachmentName);
                    mail.Attachments.Add(att);
                }

               

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

                //start: if we are in testing environment and not CC has value then send mail also to CC tester
                if (!string.IsNullOrEmpty(Utilities.GetAppSetting("Testing_Environment")))
                {
                    if (Utilities.GetAppSetting("Testing_Environment").ToString().ToLower() == "true")
                    {
                        //we are at testing environment
                        if (!string.IsNullOrEmpty(Utilities.GetAppSetting("Testing_CCMail")))
                        {
                            string CCEmail = Utilities.GetAppSetting("Testing_CCMail");
                            string[] EmailCC = CCEmail.Split(';');
                            for (int i = 0; i < EmailCC.Length; i++)
                            {
                                mail.CC.Add(EmailCC[i]);
                            }
                        }
                    }
                }
                //end:
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex.Message, ex, false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
            }

            return true;
        }

    
    }
}
