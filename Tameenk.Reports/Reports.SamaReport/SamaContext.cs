using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Data.DAL;

namespace Reports.SamaReport
{
    public class SamaContext
    {
        ErrorLogging logging = new ErrorLogging();
        public void ExportSamaExcelThenSendMail()
        {
            string SPREADSHEET_NAME = SaveSamaReportInExcel();
            DateTime dt = DateTime.Now.AddDays(-1);

            if (File.Exists(SPREADSHEET_NAME))
            {
                FileStream file = new FileStream(SPREADSHEET_NAME, FileMode.Open);
           
                SendMailFromAdminIncludingMailTemplate(file, "Sama reports " + dt.ToString("dd-MM-yyyy") + ".xlsx", dt.ToString("dd-MM-yyyy"));
                file.Dispose();
                file.Close();
            }
            return;
        }



        public string SaveSamaReportInExcel()
        {

            var sama = SamaDataAccess.GetSamaReport(60 * 60 * 60);

            if (sama != null)
            {

                DateTime dt = DateTime.Now;
                string fileName = "Tameenak_Sama_Report" + dt.ToString("dd-MM-yyyy");
                string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
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

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Sama Report" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        {
                            columns.Add("Invoice Date");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Invoice Date");
                            headerRow.AppendChild(cell);


                            columns.Add("Bcare Invoice Number");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Bcare Invoice Number");
                            headerRow.AppendChild(cell2);



                            columns.Add("SCHEME / PROJECT");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SCHEME / PROJECT");
                            headerRow.AppendChild(cell3);


                            columns.Add("Policy Holder");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Holder");
                            headerRow.AppendChild(cell4);


                            columns.Add("Mob#");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Mob#");
                            headerRow.AppendChild(cell5);


                            columns.Add("Email");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Email");
                            headerRow.AppendChild(cell6);


                            columns.Add("Insurer");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurer");
                            headerRow.AppendChild(cell9);

                            columns.Add("Policy No#");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy No#");
                            headerRow.AppendChild(cell10);

                            columns.Add("Payment  Method");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Payment  Method");
                            headerRow.AppendChild(cell11);


                            columns.Add("CARD NUMBER");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CARD NUMBER");
                            headerRow.AppendChild(cell13);

                            columns.Add("Insurance  Product");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurance  Product");
                            headerRow.AppendChild(cell14);


                            columns.Add("ExtraPremiumPrice");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ExtraPremiumPrice");
                            headerRow.AppendChild(cell15);

                            columns.Add("VAT INSURANCE COMPANY");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VAT INSURANCE COMPANY");
                            headerRow.AppendChild(cell16);

                        }

                        sheetData.AppendChild(headerRow);



                        foreach (var item in sama)
                        {

                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                            foreach (String col in columns)
                            {
                                if (col == "Invoice Date")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InvoiceDate?.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Bcare Invoice Number")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.BcareInvoiceNumber?.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "SCHEME / PROJECT")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Scheme); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Policy Holder")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyHolder); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Mob#")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Mob); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Email")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Email);
                                    newRow.AppendChild(cell);
                                }


                                else if (col == "Insurer")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Insurer); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Policy No#")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyNo); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Payment  Method")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PaymentMethod); //
                                    newRow.AppendChild(cell);
                                }


                                else if (col == "CARD NUMBER")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CardNumber); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Insurance  Product")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuranceProduct); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "ExtraPremiumPrice")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ExtraPremiumPrice?.ToString()); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "VAT INSURANCE COMPANY")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Vat?.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                            }

                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();

                        }
                        workbook.Close();
                    }

                }

                return SPREADSHEET_NAME;

            }
            return "";
        }

        public bool SendMailFromAdminIncludingMailTemplate(FileStream attachment, string attachmentName, string dt)
        {

            logging.LogDebug("SendMailFromAdminIncludingMailTemplate");
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

                var body = SamaResource.MailBody.Replace("[%DateTime%]", dt);
                body = body.Replace("[%SiteUrl%]", "https://www.bcare.com.sa/");

                mail.Body = body;
                mail.Subject = SamaResource.MailSubject.Replace("[%DateTime%]", dt);


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
                logging.LogError(ex.Message, ex, false);
            }

            return true;
        }


    }
}
