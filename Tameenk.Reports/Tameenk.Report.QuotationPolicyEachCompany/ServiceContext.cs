using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Data;
using Tameenk.Data.DAL;
using Tameenk.Loggin.DAL;
using Tameenk.Reports.Quotation;

namespace Tameenk.Report.QuotationPolicyEachCompany
{
   public class ServiceContext
    {
        #region Fields
        List<ServiceRequestLog> quotationList;
        List<ServiceRequestLog> policyList;
        List<Policy> policies;

        Hashtable RequiredFiles;

        #endregion

        ErrorLogging logging = new ErrorLogging();
        #region The Ctor
        public ServiceContext()
        {
            RequiredFiles = new Hashtable();
            RequiredFiles.Add("QuotationTrasaction","");
            RequiredFiles.Add("PolicyTrasaction", "");
            RequiredFiles.Add("SuccessPolicy", "");
        }

        #endregion

        #region Method

        public void ExportExcelThenSendMail()
        {
            List<InsuranceCompany> insuranceCompanies = InsuranceCompanyDataAccess.GetInsuranceCompanyList(3 * 60 * 60);
            foreach(InsuranceCompany insuranceCompany in insuranceCompanies)
            {
                ExportExcelThenSendMailForEachCompany(insuranceCompany.Key,insuranceCompany.InsuranceCompanyID);
            }
        }

        private void GetDataBasedOnCompanyName(string CompanyName, int companyId)
        {
            quotationList = ServiceRequestLogDataAccess.GetQuotationListForCompany(60000, CompanyName);
            policyList = ServiceRequestLogDataAccess.GetPolicyListForCompany(60000, CompanyName);
            policies = PolicyDataAccess.GetSuccessPolicyListForEachCompany(60000, companyId);
        }

        public void ExportExcelThenSendMailForEachCompany(string CompanyName,int companyId)
        {
            GetDataBasedOnCompanyName(CompanyName, companyId);

            try
            {
                ErrorLogger.LogDebug("start create Excel files and send email to " + CompanyName);

                string x = GenerateExcel(quotationList, "quotation_Transcation", CompanyName);
                RequiredFiles["QuotationTrasaction"] = GenerateExcel(quotationList, "quotation_Transcation", CompanyName);
                RequiredFiles["PolicyTrasaction"] = GenerateExcel(policyList, "policy_Transacation", CompanyName);
                RequiredFiles["SuccessPolicy"] = GenerateExcelToSuccessPolicies("Success_Policy", CompanyName);

                DateTime dt = DateTime.Now.AddDays(-1);

                List<AttachmentFile> attachmentFiles = new List<AttachmentFile>();

                List<FileStream> fileStreams = new List<FileStream>();

                foreach (DictionaryEntry item in RequiredFiles)
                {
                    if (File.Exists(item.Value.ToString()))
                    {
                        FileStream file = new FileStream(item.Value.ToString(), FileMode.Open);
                        AttachmentFile attachmentFile = new AttachmentFile();
                        attachmentFile.FileStream = file;
                        attachmentFile.FileName= CompanyName + " reports " + "_" + item.Key.ToString() + "_" + dt.ToString("dd-MM-yyyy") + ".xlsx";
                        attachmentFiles.Add(attachmentFile);
                        fileStreams.Add(file);
                    }
                }

                
                SendMailFromAdminIncludingMailTemplate(attachmentFiles, dt.ToString("dd-MM-yyyy"), CompanyName);

                foreach (FileStream file in fileStreams)
                {
                    file.Dispose();
                    file.Close();
                }

                logging.LogDebug("end create Excel files and send email to " + CompanyName);
            }catch(Exception ex)
            {
                logging.LogError("end create Excel files and send email to " + CompanyName + "with error",ex,false);
            }
        }

        public string GenerateExcel(List<ServiceRequestLog> serviceRequestLogs , string type , string CompanyName)
        {

            logging.LogDebug("start generate Excel sheets");

            try
            {
                
                    DateTime dt = DateTime.Now.AddDays(-1);
                    string SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CompanyName + "_" + type + dt.ToString("dd-MM-yyyy"));
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

                            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = type };
                            sheets.Append(sheet);

                            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                            List<string> columns = new List<string>();

                            var propertyNames = typeof(ServiceRequestLog).GetProperties().Select(x => x.Name).ToList();

                            foreach (var name in propertyNames)
                            {
                                columns.Add(name);
                                AddCellHeader(name, headerRow);
                            }

                            sheetData.AppendChild(headerRow);


                            workbook.WorkbookPart.Workbook.Save();


                        int success = 0;
                        int fail = 0;


                        if (serviceRequestLogs != null)
                        {
                            foreach (ServiceRequestLog Request in serviceRequestLogs)
                            {

                                if (Request.ErrorCode == 1)
                                    success++;
                                else
                                    fail++;


                                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();


                                foreach (String col in columns)
                                {
                                    if (col == "UserID")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserID.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "UserName")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserName); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "Method")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Method); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "CreatedDate")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "CompanyID")
                                    {
                                        // Loza
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyID.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "CompanyName")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyName); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ServiceURL")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceURL); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ErrorCode")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail"); //
                                        newRow.AppendChild(cell);
                                    }

                                    else if (col == "ErrorDescription")
                                    {
                                        /// Loza
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ServiceRequest")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceRequest); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ServiceResponse")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponse); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ServerIP")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServerIP); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "RequestId")
                                    {
                                        // Loza
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.RequestId.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ServiceResponseTimeInSeconds")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponseTimeInSeconds.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "Channel")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ServiceErrorCode")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceErrorCode); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ServiceErrorDescription")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceErrorDescription); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "VehicleId")
                                    {
                                        // Loza
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleId); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "DriverNin")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.DriverNin); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "InsuranceTypeCode")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.InsuranceTypeCode.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "ReferenceId")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ReferenceId); //
                                        newRow.AppendChild(cell);
                                    }



                                }

                                sheetData.AppendChild(newRow);
                                workbook.WorkbookPart.Workbook.Save();
                            }
                        }
                    


                        if (serviceRequestLogs != null)
                        {
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
                            cell32.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Total Number ");
                            newRow4.Append(cell32);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(serviceRequestLogs.Count.ToString());
                            newRow4.Append(cell33);

                            sheetData.AppendChild(newRow5);
                            sheetData.AppendChild(newRow6);

                            sheetData.AppendChild(newRow4);


                            // Add Cell Number of Success Request 
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
                            cell25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number Of Success ");
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
                            cell85.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number Of Fail ");
                            newRow2.Append(cell85);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell86 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell86.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell86.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(fail.ToString());
                            newRow2.Append(cell86);

                            sheetData.AppendChild(newRow2);




                            workbook.WorkbookPart.Workbook.Save();
                        }
                    }

                    workbook.Close();
                }
                return SPREADSHEET_NAME;

            }
            catch (Exception exp)
            {
                logging.LogError(exp.Message, exp, false);
                return "";
            }
         
        }

        private string GenerateExcelToSuccessPolicies(string type , string CompanyName)
        {
            logging.LogDebug("start generate Excel sheets");

            try
            {

                DateTime dt = DateTime.Now.AddDays(-1);
                string SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CompanyName + "_" + type + dt.ToString("dd-MM-yyyy"));
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

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = type };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<string> columns = new List<string>();
                        { 

                        columns.Add("PolicyNo");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo");
                        headerRow.AppendChild(cell);


                        columns.Add("PolicyIssueDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyIssueDate");
                        headerRow.AppendChild(cell2);

                        columns.Add("PolicyEffectiveDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyEffectiveDate");
                        headerRow.AppendChild(cell3);


                        columns.Add("CreatedDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cell4);


                        columns.Add("PolicyExpiryDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyExpiryDate");
                        headerRow.AppendChild(cell5);


                        columns.Add("NajmStatus");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NajmStatus");
                        headerRow.AppendChild(cell6);


                        columns.Add("IBAN");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IBAN");
                        headerRow.AppendChild(cell7);


                        columns.Add("PolicyStatus");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyStatus");
                        headerRow.AppendChild(cell8);

                        columns.Add("InsuredName");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredName");
                        headerRow.AppendChild(cell9);

                        columns.Add("TotalPremium");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TotalPremium");
                        headerRow.AppendChild(cell10);

                        sheetData.AppendChild(headerRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }

                        if (policies != null)
                        {
                            foreach (Policy Request in policies)
                            {
                                
                                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();


                                foreach (String col in columns)
                                {
                                    if (col == "PolicyNo")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyNo); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "PolicyIssueDate")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyIssueDate?.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "PolicyEffectiveDate")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyEffectiveDate?.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "PolicyExpiryDate")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyExpiryDate?.ToString()); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "NajmStatus")
                                    {
                                        // Loza
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.NajmStatus); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "IBAN")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CheckoutDetail.IBAN); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "PolicyStatus")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CheckoutDetail.PolicyStatu.NameAr); //
                                        newRow.AppendChild(cell);
                                    }
                                    else if (col == "InsuredName")
                                    {
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyDetail.InsuredNameAr); //
                                        newRow.AppendChild(cell);
                                    }

                                    else if (col == "TotalPremium")
                                    {
                                        /// Loza
                                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyDetail.TotalPremium); //
                                        newRow.AppendChild(cell);
                                    }



                                }

                                sheetData.AppendChild(newRow);
                                workbook.WorkbookPart.Workbook.Save();
                            }
                        }

                    }

                    workbook.Close();
                }
                return SPREADSHEET_NAME;

            }
            catch (Exception exp)
            {
                logging.LogError(exp.Message, exp, false);
                return "";
            }
        }

        public bool SendMailFromAdminIncludingMailTemplate(List<AttachmentFile> attachmentFiles, string dt,string company)
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
                    ErrorLogger.LogDebug(ToMails[j]);
                    toCollection.Add(ToMails[j]);
                }
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.IsBodyHtml = true;


                DateTime startDate = DateTime.Now.Date.AddDays(-1).AddHours(0).AddMinutes(0).AddSeconds(0);// new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 0, 0, 0);
                DateTime endDate = DateTime.Now.Date.Date.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); //new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 23, 59, 59);

                string valueDatetime = " from " + startDate.ToString() + " to " + endDate.ToString() + " ";

                 var body = ResourceFile.MailBody.Replace("[%DateTime%]", valueDatetime);
                body = body.Replace("[%SiteUrl%]", "https://www.bcare.com.sa/");
                body = body.Replace("[%Company%]", company);
                
                 mail.Body = body;
                mail.Subject = ResourceFile.MailSubject.Replace("[%Company%][%DateTime%]", company + " " + dt);
            

                for (int i = 0; i < toCollection.Count; i++)
                {
                    mail.To.Add(toCollection[i]);
                }

                mail.From = adminMail;

                foreach(AttachmentFile item in attachmentFiles)
                {
                    if (item != null && item.FileStream !=null && !string.IsNullOrEmpty(item.FileName))
                    {
                        System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(item.FileStream, item.FileName);
                        mail.Attachments.Add(att);
                    }
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


        private static void AddCellHeader(string Name, DocumentFormat.OpenXml.Spreadsheet.Row headerRow)
        {
            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Name);
            headerRow.AppendChild(cell);
        }

        #endregion

    }
}
