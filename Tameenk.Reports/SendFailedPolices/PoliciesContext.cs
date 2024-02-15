using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Data.DAL;

namespace SendFailedPolices
{
    public class PoliciesContext
    {
        ErrorLogging logging = new ErrorLogging();
        public void ExportPolicyExcelThenSendMail()
        {
            string SPREADSHEET_NAME = SaveFailedPoliciesInExcel();
            DateTime dt = DateTime.Now.AddDays(-1);

            if (File.Exists(SPREADSHEET_NAME))
            {
                FileStream file = new FileStream(SPREADSHEET_NAME, FileMode.Open);
             
                SendMailFromAdminIncludingMailTemplate(file, "Fail policies reports " + dt.ToString("dd-MM-yyyy") + ".xlsx", dt.ToString("dd-MM-yyyy"));
                file.Dispose();
                file.Close();
            }
            return;
        }



        public string SaveFailedPoliciesInExcel()
        {

            var failedPolicies = PolicyProcessingQueueDataAccess.GetFailPolicyList(60*60*60);

            if (failedPolicies != null)
            {
               
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = "Tameenak_Failed_Policies_" + dt.ToString("dd-MM-yyyy");
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

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        {
                            columns.Add("DriverNationalId");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverNationalId");
                            headerRow.AppendChild(cell);


                            columns.Add("DriverFullName");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverFullName");
                            headerRow.AppendChild(cell2);

                           

                            columns.Add("VehicleId");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleId");
                            headerRow.AppendChild(cell3);


                            columns.Add("CreatedDate");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                            headerRow.AppendChild(cell4);


                            columns.Add("CompanyID");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompanyID");
                            headerRow.AppendChild(cell5);


                            columns.Add("CompanyName");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompanyName");
                            headerRow.AppendChild(cell6);


                            columns.Add("ErrorDescription");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorDescription");
                            headerRow.AppendChild(cell9);

                            columns.Add("ServiceRequest");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceRequest");
                            headerRow.AppendChild(cell10);

                            columns.Add("ServiceResponse");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceResponse");
                            headerRow.AppendChild(cell11);


                            columns.Add("ReferenceId");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                            headerRow.AppendChild(cell13);

                            columns.Add("InvoiceNo");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InvoiceNo");
                            headerRow.AppendChild(cell14);


                            columns.Add("DriverEmail");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverEmail");
                            headerRow.AppendChild(cell15);

                            columns.Add("DriverPhone");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverPhone");
                            headerRow.AppendChild(cell16);

                        }

                        sheetData.AppendChild(headerRow);



                        foreach (var policyItem in failedPolicies)
                        {

                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                            foreach (String col in columns)
                            {
                                if (col == "DriverNationalId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.Driver?.NIN); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "DriverFullName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.Driver?.FirstName+ " " + policyItem.Driver?.SubtribeName + " " + policyItem.Driver?.ThirdName); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "VehicleId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(!string.IsNullOrEmpty(policyItem.Vehicle?.CustomCardNumber) ? policyItem.Vehicle?.CustomCardNumber : policyItem.Vehicle?.SequenceNumber); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.PolicyProcessingQueue?.CreatedOn.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CompanyID")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.InsuranceCompany?.InsuranceCompanyID.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.InsuranceCompany?.NameAR);
                                    newRow.AppendChild(cell);
                                }
                               

                                else if (col == "ErrorDescription")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.PolicyProcessingQueue?.ErrorDescription); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "ServiceRequest")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.PolicyProcessingQueue?.ServiceRequest); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "ServiceResponse")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.PolicyProcessingQueue?.ServiceResponse); //
                                    newRow.AppendChild(cell);
                                }


                                else if (col == "ReferenceId")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.PolicyProcessingQueue?.ReferenceId); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InvoiceNo")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.Invoice?.InvoiceNo.ToString()); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "DriverEmail")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.CheckoutDetail?.Email); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "DriverPhone")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.CheckoutDetail?.Phone); //
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

                var body = PolicyServiceReport.MailBody.Replace("[%DateTime%]", dt);
                body = body.Replace("[%SiteUrl%]", "https://www.bcare.com.sa/");

                mail.Body = body;
                mail.Subject = PolicyServiceReport.MailSubject.Replace("[%DateTime%]", dt);


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
                    logging.LogError(ex.Message, ex, false);
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
