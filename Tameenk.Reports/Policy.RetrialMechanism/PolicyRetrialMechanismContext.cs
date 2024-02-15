
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL;

namespace Policy.RetrialMechanism
{
    public class PolicyRetrialMechanismContext
    {
        public static void GetAndSubmitFailedTransactions()
        {
            //try
            //{
            //    var commandTimeout = 60000; // default of 1 minute
            //    int commandTimeoutTemp;
            //    if (int.TryParse(Utilities.GetAppSetting("CommandTimeout"), out commandTimeoutTemp))
            //        commandTimeout = commandTimeoutTemp;

            //    List<PolicyFailedTransaction> logs = PolicyFailedTransactionDataAccess.GetFailedTransactionsSinceAday(commandTimeout);
            //    if (logs != null)
            //    {
            //        var client = Utilities.GetServiceClient<PaymentServicesClient, IPaymentServices>();
            //        if (client == null)
            //            client = new PaymentServicesClient();

            //        foreach (BillPaymentFailedTransaction log in logs)
            //        {
            //            int numberOfEAIHits = 0;
            //            numberOfEAIHits = log.NumberOfEAIHits.HasValue ? log.NumberOfEAIHits.Value + 1 : 1;
            //            try
            //            {

            //                addBillPaymentOutput billPaymentOutput = null;
            //                try
            //                {
            //                    billPaymentOutput = client.AddBillPayment(Utilities.RemoveZeroFromDial(log.Dial), Channel.BillPaymentRetrialMechanism, ModulesNames.PayBillViaCreditCard, log.RequestID.Value, log.AmountInPounds, log.MIGSVpcTransactionNo, log.EAIPaymentSerialNumber);
            //                }
            //                catch (Exception exp)
            //                {
            //                    ErrorLogger.LogError(exp.Message, exp, false);
            //                    PolicyFailedTransactionDataAccess.UpdateBillPaymentFailedTransactions(log.ID, numberOfEAIHits, "", "", "AddBillPayment failed - exception");
            //                    continue;
            //                }
            //                if (billPaymentOutput == null)
            //                {
            //                    PolicyFailedTransactionDataAccess.UpdateBillPaymentFailedTransactions(log.ID, numberOfEAIHits, "", "", "AddBillPayment failed");
            //                    continue;
            //                }
            //                PolicyFailedTransactionDataAccess.UpdateBillPaymentFailedTransactions(log.ID, numberOfEAIHits, billPaymentOutput.EAIErrorCode, billPaymentOutput.EAIStatus, billPaymentOutput.EAIErrorDescription);
            //                if (billPaymentOutput.ErrorCode != addBillPaymentOutput.ErrorCodes.Success)
            //                {
            //                    continue;
            //                }
            //                bool? isDeleted = PolicyFailedTransactionDataAccess.DeleteFromFailedTransaction(log.ID, log.Dial);
            //                if (!isDeleted.HasValue || !isDeleted.Value)
            //                {
            //                    //stop and send email to support team to avoid submitting to EAI twice
            //                    System.Net.Mail.MailAddressCollection supportEmails = new System.Net.Mail.MailAddressCollection();
            //                    string[] mailTo = Utilities.GetAppSetting("SupportEmails").Split(';');
            //                    foreach (string mail in mailTo)
            //                    {
            //                        supportEmails.Add(mail);
            //                    }
            //                    string mailBody = "please note that Retrial Mechanism had paid successfully the bill for this dial " + log.Dial + " but it can't delete the record from the table BillPaymentFailedTransactions RecordID=" + log.ID;
            //                    string mailSubject = "can not delete from table BillPaymentFailedTransactions";
            //                    MailUtilities.SendMailToMultiple(mailBody, mailSubject, Utilities.GetAppSetting("MailFrom"), supportEmails);
            //                    break;
            //                }
            //                PayBillViaCreditCardLog logInfo = PayBillViaCreditCardLogsDataAccess.GetEndRecordBillViaCreditCardLogByRequestID(log.RequestID.Value);

            //                logInfo.ErrorCode = (int)PayBillViaCreditCardOutput.ErrorCodes.Success;
            //                logInfo.ErrorDescription = "Success";
            //                logInfo.Channel = Channel.BillPaymentRetrialMechanism.ToString();

            //                logInfo.EAIStatus = billPaymentOutput.EAIStatus;
            //                logInfo.EAIErrorCode = billPaymentOutput.EAIErrorCode;
            //                logInfo.EAIErrorDescription = billPaymentOutput.EAIErrorDescription;

            //                PayBillViaCreditCardLogsDataAccess.UpdateBillViaCreditCardLogs(logInfo);
            //                CreditCardLocksDataAccess.DeleteFromCreditCardLocks(log.CreditCardHashed, ModulesNames.PayBillViaCreditCard.ToString());
            //            }
            //            catch (Exception perTransactionException)
            //            {
            //                ErrorLogger.LogError(perTransactionException.Message, perTransactionException, false);
            //                continue;
            //            }
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    ErrorLogger.LogError(exp.Message, exp, false);
            //}
        }


        /// <summary>
        /// Exports the report to excel.
        /// </summary>
        public static void SendMailWithReportOfFailedTransactionsSinceAday(string mailBody, string mailSubject)
        {
        //    try
        //    {
        //        var commandTimeout = 60000; // default of 1 minute
        //        int commandTimeoutTemp;
        //        if (int.TryParse(Utilities.GetAppSetting("CommandTimeout"), out commandTimeoutTemp))
        //            commandTimeout = commandTimeoutTemp;

        //        List<PolicyFailedTransaction> failedTransactions = BillPaymentFailedTransactionsDataAccess.GetFailedTransactionsSinceAday(commandTimeout);
        //        if (failedTransactions == null)
        //            return;
        //        DateTime dt = DateTime.Now.AddDays(-2);
        //        string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dt.ToString("dd-MM-yyyy-HHmmss"));
        //        SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";

        //        Utilities.DeleteExcelSheetFiles();
        //        using (var workbook = SpreadsheetDocument.Create(SPREADSHEET_NAME, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
        //        {
        //            var workbookPart = workbook.AddWorkbookPart();


        //            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        //            var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
        //            sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

        //            workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

        //            workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

        //            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
        //            string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        //            uint sheetId = 1;
        //            if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
        //            {
        //                sheetId =
        //                    sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        //            }

        //            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailedTransactions" };
        //            sheets.Append(sheet);

        //            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

        //            List<String> columns = new List<string>();
        //            columns.Add("Amount");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Amount");
        //            headerRow.AppendChild(cell);


        //            columns.Add("Dial");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Dial");
        //            headerRow.AppendChild(cell2);

        //            columns.Add("InvoiceNumber");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InvoiceNumber");
        //            headerRow.AppendChild(cell3);


        //            columns.Add("PaymentSerialNumber");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaymentSerialNumber");
        //            headerRow.AppendChild(cell4);


        //            columns.Add("VPCReceiptNo");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VPCReceiptNo");
        //            headerRow.AppendChild(cell6);


        //            //columns.Add("VPCMessage");
        //            //DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            //cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            //cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VPCMessage");
        //            //headerRow.AppendChild(cell7);


        //            columns.Add("EAIErrorCode");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EAIErrorCode");
        //            headerRow.AppendChild(cell9);

        //            columns.Add("EAIStatus");

        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EAIStatus");
        //            headerRow.AppendChild(cell10);

        //            columns.Add("CreatedDate");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
        //            headerRow.AppendChild(cell11);

        //            sheetData.AppendChild(headerRow);


        //            foreach (BillPaymentFailedTransaction log in failedTransactions)
        //            {
        //                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

        //                foreach (String col in columns)
        //                {
        //                    if (col == "Amount")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellAmount = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellAmount.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellAmount.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.AmountInPounds); //
        //                        newRow.AppendChild(cellAmount);
        //                    }
        //                    else if (col == "Dial")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellDial = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellDial.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellDial.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.Dial); //
        //                        newRow.AppendChild(cellDial);
        //                    }
        //                    else if (col == "InvoiceNumber")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellInvoiceNumber = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellInvoiceNumber.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellInvoiceNumber.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.MIGSVpcTransactionNo); //
        //                        newRow.AppendChild(cellInvoiceNumber);
        //                    }
        //                    else if (col == "PaymentSerialNumber")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellPaymentSerialNumber = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellPaymentSerialNumber.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellPaymentSerialNumber.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.EAIPaymentSerialNumber); //
        //                        newRow.AppendChild(cellPaymentSerialNumber);
        //                    }
        //                    else if (col == "VPCReceiptNo")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellVPCReceiptNo = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellVPCReceiptNo.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellVPCReceiptNo.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.MIGSVpcReceiptNo); //
        //                        newRow.AppendChild(cellVPCReceiptNo);
        //                    }
        //                    //else if (col == "VPCMessage")
        //                    //{
        //                    //    DocumentFormat.OpenXml.Spreadsheet.Cell cellVPCMessage = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                    //    cellVPCMessage.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                    //    cellVPCMessage.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.VPCMessage); //
        //                    //    newRow.AppendChild(cellVPCMessage);
        //                    //}
        //                    else if (col == "EAIErrorCode")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellEAIErrorCode = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellEAIErrorCode.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellEAIErrorCode.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.EAIErrorCode); //
        //                        newRow.AppendChild(cellEAIErrorCode);
        //                    }
        //                    else if (col == "EAIStatus")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellEAIStatus = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellEAIStatus.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellEAIStatus.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.EAIStatus); //
        //                        newRow.AppendChild(cellEAIStatus);
        //                    }
        //                    else if (col == "CreatedDate")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cellCreatedDate = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cellCreatedDate.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cellCreatedDate.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(log.CreatedDate.HasValue ? log.CreatedDate.Value.ToString("F") : ""); //
        //                        newRow.AppendChild(cellCreatedDate);
        //                    }
        //                }

        //                sheetData.AppendChild(newRow);
        //                workbook.WorkbookPart.Workbook.Save();
        //            }
        //            workbook.Close();
        //        }
        //        if (File.Exists(SPREADSHEET_NAME))
        //        {
        //            FileStream file = new FileStream(SPREADSHEET_NAME, FileMode.Open);

        //            mailBody = mailBody.Replace("[%DateTime%]", dt.ToString("F"));
        //            mailBody = mailBody.Replace("[%SiteUrl%]", Utilities.PublicSiteURL);

        //            MailUtilities.SendMailWithAttachment(mailBody, mailSubject.Replace("[%DateTime%]", dt.ToString("F")), Utilities.GetAppSetting("MailFrom"), Utilities.GetAppSetting("MailTo"), Utilities.GetAppSetting("CCMail"), file, "Bill-Payment-Failed-Transactions-To (" + dt.ToString("F").Replace(":", ".") + ").xlsx");
        //            file.Dispose();
        //            file.Close();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        ErrorLogger.LogError(exp.Message, exp, false);
        //    }
        }
    }
}
