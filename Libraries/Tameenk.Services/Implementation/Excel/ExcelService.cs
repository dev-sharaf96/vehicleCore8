using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Excel;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Dtos;
using System.Globalization;
using Tameenk.Services.Implementation.Invoices;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Core.BlockNins;
using System.Web.Script.Serialization;

namespace Tameenk.Services.Implementation.Excel
{
    public class ExcelService : IExcelService
    {

        #region Methods


        public byte[] GenerateExcelFailPolicies(List<FailPolicy> failedPolicies)
        {

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
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.CheckoutDetail.Driver?.NIN); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "DriverFullName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.CheckoutDetail.Driver?.FirstName + " " + policyItem.CheckoutDetail.Driver?.SubtribeName + " " + policyItem.CheckoutDetail.Driver?.ThirdName); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "VehicleId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(!string.IsNullOrEmpty(policyItem.CheckoutDetail.Vehicle?.CustomCardNumber) ? policyItem.CheckoutDetail.Vehicle?.CustomCardNumber : policyItem.CheckoutDetail.Vehicle?.SequenceNumber); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.PolicyProcessingQueue?.CreatedDate.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CompanyID")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.Invoice.InsuranceCompany?.InsuranceCompanyID.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.Invoice.InsuranceCompany?.NameAR);
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


                return GetFileAsByte(SPREADSHEET_NAME);
            }

            return null;
        }

        public byte[] GenerateFailPoliciesExcel(List<PolicyListingModel> failedPolicies)
        {

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

                            
                            columns.Add("QuotationPrice");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("QuotationPrice");
                            headerRow.AppendChild(cell12);

                            columns.Add("PolicyPrice");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyPrice");
                            headerRow.AppendChild(cell17);

                            columns.Add("PaidAmount");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaidAmount");
                            headerRow.AppendChild(cell18);

                            columns.Add("BenfitsPrice");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("BenfitsPrice");
                            headerRow.AppendChild(cell19);  
                            
                            columns.Add("Merchant Id");

                            DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Merchant Id");
                            headerRow.AppendChild(cell20);

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
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.InsuredNIN); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "DriverFullName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.InsuredFullNameAr); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "VehicleId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(!string.IsNullOrEmpty(policyItem.CustomCardNumber) ? policyItem.CustomCardNumber : policyItem.SequenceNumber); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.CreateDate.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CompanyID")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.InsuranceCompanyID.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.InsuranceCompanyNameAr);
                                    newRow.AppendChild(cell);
                                }


                                else if (col == "ErrorDescription")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem?.ErrorDescription); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "ServiceRequest")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem?.ServiceRequest); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "ServiceResponse")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem?.ServiceResponse); //
                                    newRow.AppendChild(cell);
                                }


                                else if (col == "ReferenceId")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem?.CheckOutDetailsId); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "InvoiceNo")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem?.InvoiceNo.ToString()); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "DriverEmail")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem?.Email); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "DriverPhone")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem?.Phone); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "QuotationPrice")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.ProductPrice.ToString()); //
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "PolicyPrice")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.TotalPrice.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "PaidAmount")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.TotalPrice.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "BenfitsPrice")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.BenfitsPrice.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "Merchant Id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.MerchantId.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                            }

                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();

                        }
                        workbook.Close();
                    }

                }


                return GetFileAsByte(SPREADSHEET_NAME);
            }

            return null;
        }
        private byte[] GetFileAsByte(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);

                file.Dispose();
                file.Close();

                DeleteFile(fileName);

                return memoryStream.ToArray();
            }



            return null;
        }


        private void DeleteFile(string fileName)
        {
            if (File.Exists(@fileName))
            {
                File.Delete(@fileName);
            }
        }

        public byte[] GenerateExcelSuccessPolicies(List<PolicyListingModel> policies)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SuccessPolicy" + dt.ToString("dd-MM-yyyy"));
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();

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

                        columns.Add("ReferenceId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                        headerRow.AppendChild(cell11);

                        columns.Add("QuotationPrice");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("QuotationPrice");
                        headerRow.AppendChild(cell12);

                        columns.Add("PolicyPrice");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyPrice");
                        headerRow.AppendChild(cell13);

                        columns.Add("PaidAmount");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaidAmount");
                        headerRow.AppendChild(cell14);

                        columns.Add("BenfitsPrice");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("BenfitsPrice");
                        headerRow.AppendChild(cell15);

                        columns.Add("Phone");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Phone");
                        headerRow.AppendChild(cell16);

                        columns.Add("Email");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Email");
                        headerRow.AppendChild(cell17);

                        columns.Add("Insured ID");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured ID");
                        headerRow.AppendChild(cell18);

                        columns.Add("Channel");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell19);

                        columns.Add("VehicleOwnerId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleOwnerId");
                        headerRow.AppendChild(cell20);
                        columns.Add("Merchant ID");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Merchant ID");
                        headerRow.AppendChild(cell21);
                    }

                    sheetData.AppendChild(headerRow);

                    workbook.WorkbookPart.Workbook.Save();

                    foreach (PolicyListingModel policy in policies)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();


                        foreach (string col in columns)
                        {
                            if (col == "PolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyNo); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PolicyIssueDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyIssueDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PolicyEffectiveDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyEffectiveDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PolicyExpiryDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyExpiryDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NajmStatus")
                            {
                                // Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.NajmStatus); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "IBAN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.IBAN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PolicyStatus")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyStatusNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy?.InsuredFullNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "TotalPremium")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.TotalPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ReferenceId")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CheckOutDetailsId.ToString()); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "QuotationPrice")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ProductPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "PolicyPrice")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.TotalPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PaidAmount")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.TotalPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "BenfitsPrice")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.BenfitsPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Phone")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(!string.IsNullOrEmpty(policy.Phone) ? policy.Phone.ToString() : string.Empty); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Email")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Email.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Insured ID")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredNIN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Channel")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Channel); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleOwnerId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleOwnerId);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Merchant ID")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.MerchantId.ToString());
                                newRow.AppendChild(cell);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }

                }

                workbook.Close();
            }


            return GetFileAsByte(SPREADSHEET_NAME);

        }

        public byte[] GenerateExcelSamaReport(List<SamaReport> sama)
        {
            DateTime dt = DateTime.Now;
            string fileName = "Tameenak_Sama_Report" + dt.ToString("dd-MM-yyyy");
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
            Utilities.DeleteExcelSheetFiles();
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

                        columns.Add("Invoice Time");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellInvoiceTime = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellInvoiceTime.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellInvoiceTime.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Invoice Time");
                        headerRow.AppendChild(cellInvoiceTime);



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


                        //columns.Add("Mob#");

                        //DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        //cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        //cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Mob#");
                        //headerRow.AppendChild(cell5);

                        //columns.Add("Account Mob#");
                        //DocumentFormat.OpenXml.Spreadsheet.Cell cellAccountMob = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        //cellAccountMob.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        //cellAccountMob.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Account Mob#");
                        //headerRow.AppendChild(cellAccountMob);


                        //columns.Add("Email");

                        //DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        //cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        //cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Email");
                        //headerRow.AppendChild(cell6);

                        //columns.Add("Account Email");
                        //DocumentFormat.OpenXml.Spreadsheet.Cell cellAccountEmail = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        //cellAccountEmail.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        //cellAccountEmail.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Account Email");
                        //headerRow.AppendChild(cellAccountEmail);


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

                        columns.Add("Policy Issue Date");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell101 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell101.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell101.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Issue Date");
                        headerRow.AppendChild(cell101);

                        columns.Add("Policy Effective Date");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell102 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell102.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell102.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Effective Date");
                        headerRow.AppendChild(cell102);

                        columns.Add("Policy Expiry Date");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell103 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell103.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell103.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Expiry Date");
                        headerRow.AppendChild(cell103);

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

                        columns.Add("Reference Number");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reference Number");
                        headerRow.AppendChild(cell17);

                        columns.Add("Merchant Transaction Id");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell177 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell177.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell177.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Merchant Transaction Id");
                        headerRow.AppendChild(cell177);

                        columns.Add("Driver BirthDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Driver BirthDate");
                        headerRow.AppendChild(cell18);

                        columns.Add("National Id");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19x = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19x.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19x.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("National Id");
                        headerRow.AppendChild(cell19x);

                        columns.Add("IBAN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20x = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20x.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20x.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IBAN");
                        headerRow.AppendChild(cell20x);


                        columns.Add("Product Price");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18x = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18x.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18x.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Product Price");
                        headerRow.AppendChild(cell18x);

                        columns.Add("Basic Price");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18x1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18x1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18x1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Basic Price");
                        headerRow.AppendChild(cell18x1);

                        columns.Add("Total Price");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellx181 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellx181.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellx181.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Total Price");
                        headerRow.AppendChild(cellx181);

                        columns.Add("Paid Amount");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellx18111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellx18111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellx18111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Paid Amount");
                        headerRow.AppendChild(cellx18111);

                        columns.Add("Discount");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellx182 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellx182.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellx182.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Discount");
                        headerRow.AppendChild(cellx182);

                        columns.Add("Fees");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellx183 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellx183.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellx183.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Fees");
                        headerRow.AppendChild(cellx183);

                        columns.Add("Channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell19);

                        columns.Add("TotalBCareFees");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell199 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell199.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell199.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TotalBCareFees");
                        headerRow.AppendChild(cell199);

                        columns.Add("FeesCalculationDetails");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell1999991 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell1999991.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell1999991.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("FeesCalculationDetails");
                        headerRow.AppendChild(cell1999991);

                        columns.Add("ActualBankFees");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell1991 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell1991.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell1991.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ActualBankFees");
                        headerRow.AppendChild(cell1991);

                        columns.Add("TotalBCareCommission");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell1999 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell1999.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell1999.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TotalBCareCommission");
                        headerRow.AppendChild(cell1999);

                        columns.Add("TotalBCareAmount");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19999 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19999.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19999.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TotalBCareAmount");
                        headerRow.AppendChild(cell19999);

                        columns.Add("TotalCompanyAmount");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell199999 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell199999.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell199999.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TotalCompanyAmount");
                        headerRow.AppendChild(cell199999);

                       

                        columns.Add("NajmNcdFreeYears");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell24 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell24.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell24.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NajmNcdFreeYears");
                        headerRow.AppendChild(cell24);

                        columns.Add("NoOfAccident");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell25 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell25.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NoOfAccident");
                        headerRow.AppendChild(cell25);

                        columns.Add("ChassisNumber");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell26 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell26.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell26.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ChassisNumber");
                        headerRow.AppendChild(cell26);

                        columns.Add("VehicleOwnerId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell30 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell30.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell30.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleOwnerId");
                        headerRow.AppendChild(cell30);

                        columns.Add("VehicleBodyCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell231 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell231.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell231.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleBodyCode");
                        headerRow.AppendChild(cell231);

                        columns.Add("ArabicVehicleBody");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell232 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell232.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell232.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ArabicVehicleBody");
                        headerRow.AppendChild(cell232);

                        columns.Add("EnglishVehicleBody");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell233 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell233.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell233.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EnglishVehicleBody");
                        headerRow.AppendChild(cell233);


                        columns.Add("InsuredNationalId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell31 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell31.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell31.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredNationalId");
                        headerRow.AppendChild(cell31);
                        

                        columns.Add("EnglishBankName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell27 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell27.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell27.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EnglishBankName");
                        headerRow.AppendChild(cell27);

                        columns.Add("ArabicBankName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell28 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell28.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell28.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ArabicBankName");
                        headerRow.AppendChild(cell28);

                        columns.Add("TotalBCareDiscount");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2888 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2888.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2888.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TotalBCareDiscount");
                        headerRow.AppendChild(cell2888);

                        columns.Add("DiscountCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell28888 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell28888.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell28888.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DiscountCode");
                        headerRow.AppendChild(cell28888);

                        columns.Add("DiscountValue");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell288888 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell288888.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell288888.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DiscountValue");
                        headerRow.AppendChild(cell288888);

                        columns.Add("DiscountPercentage");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2888888 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2888888.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2888888.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DiscountPercentage");
                        headerRow.AppendChild(cell2888888);

                        columns.Add("Additional Driver 1");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Additional Driver 1");
                        headerRow.AppendChild(cell20);

                        columns.Add("Additional Driver 2");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Additional Driver 2");
                        headerRow.AppendChild(cell21);

                        columns.Add("Additional Driver 3");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Additional Driver 3");
                        headerRow.AppendChild(cell22);

                        columns.Add("Additional Driver 4");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell23 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell23.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell23.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Additional Driver 4");
                        headerRow.AppendChild(cell23);

                      
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
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InvoiceDate?.Date.ToString("dd-MM-yyyy")); //
                                newRow.AppendChild(cell);
                            }
                            if (col == "Invoice Time")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InvoiceDate?.TimeOfDay.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Reference Number")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ReferenceNo.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Merchant Transaction Id")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.MerchantTransactionId.HasValue?item.MerchantTransactionId?.ToString(): item.ReferenceNo); //
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
                            //else if (col == "Mob#")
                            //{
                            //    // Loza
                            //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Mob); //
                            //    newRow.AppendChild(cell);
                            //}
                            //else if (col == "Account Mob#")
                            //{
                            //    // Loza
                            //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.AccountMob); //
                            //    newRow.AppendChild(cell);
                            //}
                            //else if (col == "Email")
                            //{
                            //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Email);
                            //    newRow.AppendChild(cell);
                            //}

                            //else if (col == "Account Email")
                            //{
                            //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.AccountEmail);
                            //    newRow.AppendChild(cell);
                            //}

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
                            else if (col == "Policy Issue Date")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyIssueDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy Effective Date")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyEffectiveDate.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy Expiry Date")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyExpiryDate.ToString()); //
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

                            else if (col == "Driver BirthDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DriverBirthDate); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "National Id")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NationalId); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "IBAN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.IBAN); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "Product Price")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ProductPrice?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Basic Price")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.BasicPrice?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Total Price")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.TotalPrice?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Paid Amount")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PaidAmount?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Discount")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Discount?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Fees")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Fees?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Channel")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Channel?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "TotalBCareFees")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.TotalBCareFees?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "FeesCalculationDetails")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.FeesCalculationDetails); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "ActualBankFees")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ActualBankFees?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "TotalBCareCommission")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.TotalBCareCommission?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "TotalBCareAmount")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                if(item.TotalBCareFees.HasValue &&item.TotalBCareCommission.HasValue)
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue((item.TotalBCareFees.Value+item.TotalBCareCommission.Value).ToString()); //
                                else
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(string.Empty); //

                                newRow.AppendChild(cell);                            }
                            else if (col == "TotalCompanyAmount")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.TotalCompanyAmount?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "NajmNcdFreeYears")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NajmNcdFreeYears?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "NoOfAccident")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NoOfAccident?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "ChassisNumber")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ChassisNumber?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "VehicleOwnerId")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CarOwnerNIN?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "VehicleBodyCode")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleBodyCode.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "ArabicVehicleBody")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ArabicVehicleBody?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "EnglishVehicleBody")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.EnglishVehicleBody?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "InsuredNationalId")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuredNationalId?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "EnglishBankName")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.EnglishBankName?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "ArabicBankName")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ArabicBankName?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "TotalBCareDiscount")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.TotalBCareDiscount?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "DiscountCode")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DiscountCode); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "DiscountValue")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DiscountValue?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "DiscountPercentage")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DiscountPercentage?.ToString()); //
                                newRow.AppendChild(cell);                            }
                            else if (col == "Additional Driver 1")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.AdditionalDriverOneName ?? string.Empty); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "Additional Driver 2")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.AdditionalDriverTwoName ?? string.Empty); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "Additional Driver 3")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.AdditionalDriverThreeName ?? string.Empty); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "Additional Driver 4")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.AdditionalDriverFourName ?? string.Empty); //
                                newRow.AppendChild(cell);
                            }
                        }
                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }
                    workbook.Close();
                }

            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }


        /// <summary>
        /// Generate Excel Sheet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="nameOfExcelSheet"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public byte[] GenerateExcelSheet<T>(List<T> data, string nameOfExcelSheet, DateTime? dateTime = null)
        {

            SpreadsheetDocument spreadsheetDocument = CreateExcelFile(ref nameOfExcelSheet, dateTime);

            if (spreadsheetDocument != null)
            {
                List<string> cols = new List<string>();
                SheetData sheetData = null;
                spreadsheetDocument = CreateExcelSheet<T>(spreadsheetDocument, cols, out sheetData);
                spreadsheetDocument = AddDataToExcelSheet<T>(data, cols, spreadsheetDocument, ref sheetData);
                spreadsheetDocument.Dispose();
            }


            GetFileAsByte(nameOfExcelSheet);

            return null;
        }


        public byte[] GenericGenerationExcelSheet<T>(List<T> data, string nameOfExcelSheet, DateTime? dateTime = null)
        {
            try
            {
                SpreadsheetDocument spreadsheetDocument = CreateExcelFile(ref nameOfExcelSheet, dateTime);


                List<string> elements = HandleObjectsData<T>(data);
                List<string> cols = HandleHeader(elements[0]);

                if (spreadsheetDocument != null)
                {

                    SheetData sheetData = null;
                    spreadsheetDocument = CreateExcelSheetGeneric(spreadsheetDocument, cols, out sheetData);
                    spreadsheetDocument = AddDataToExcelSheetGeneric(elements, spreadsheetDocument, ref sheetData);
                    spreadsheetDocument.Close();//.Dispose();
                }
                return GetFileAsByte(nameOfExcelSheet);

            }
            catch (Exception ex)
            {
                DeleteFile(nameOfExcelSheet);
                return null;
            }


        }

        private List<string> HandleObjectsData<T>(List<T> data)
        {
            List<string> elements = new List<string>();
            foreach (var item in data)
            {
                string element = new JavaScriptSerializer().Serialize(item);
                elements.Add(element);
            }
            return elements;
        }

        private List<string> HandleHeader(string item)
        {
            List<string> cols = new List<string>();
            item = item.Replace("}", "");
            item = item.Replace("{", "");
            string[] afterSplitComa = SplitData(item);
            for (int i = 0; i < afterSplitComa.Length; i++)
            {
                string element = afterSplitComa[i].Split(':')[0].Replace("\"", "");
                if (!string.IsNullOrEmpty(element))
                    cols.Add(element);
            }

            return cols;
        }

        #endregion


        #region private Methods


        private SpreadsheetDocument AddDataToExcelSheet<T>(List<T> data, List<string> cols, SpreadsheetDocument workbook, ref SheetData sheetData)
        {
            foreach (var item in data)
            {
                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                foreach (string col in cols)
                {
                    string cellValue = item.GetType().GetProperty(col).GetValue(item, null)?.ToString();
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = GenerateCell(cellValue);
                    newRow.AppendChild(cell);
                }
                sheetData.AppendChild(newRow);
                workbook.WorkbookPart.Workbook.Save();
            }

            return workbook;
        }


        private string[] SplitData(string data)
        {
            List<int> start = new List<int>();
            List<int> end = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case '{':
                        start.Add(i);
                        break;
                    case '}':
                        end.Add(i);
                        break;

                }
            }

            for (int i = 0; i < start.Count(); i++)
            {
                string sub = data.Substring(start[i], end[i]);
                sub = sub.Replace(",", "#");
                sub = sub.Replace(":", "$");
                data = data.Replace(data.Substring(start[i], end[i]), sub);
            }
            return data.Split(',');
        }


        private SpreadsheetDocument AddDataToExcelSheetGeneric(List<string> data, SpreadsheetDocument workbook, ref SheetData sheetData)
        {

            foreach (var item in data)
            {
                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                string element = item.Substring(1, item.Length - 2);
                //element= element.Replace("{", "");
                string[] cols = SplitData(element);
                foreach (string col in cols)
                {
                    string cellValue = col.Split(':')[1].Replace("\"", "");
                    cellValue = cellValue.Replace("#", ",");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = GenerateCell(cellValue);
                    newRow.AppendChild(cell);
                }
                sheetData.AppendChild(newRow);
                workbook.WorkbookPart.Workbook.Save();
            }

            return workbook;
        }



        private SpreadsheetDocument CreateExcelFile(ref string nameOfExcelSheet, DateTime? dateTime)
        {

            nameOfExcelSheet = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameOfExcelSheet + "_" + dateTime?.ToString("dd-MM-yyyy"));
            nameOfExcelSheet = nameOfExcelSheet + ".xlsx";
            DeleteFile(nameOfExcelSheet);
            return SpreadsheetDocument.Create(nameOfExcelSheet, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);

        }


        private SpreadsheetDocument CreateExcelSheet<T>(SpreadsheetDocument workbook, List<string> cols, out SheetData sheetData)
        {
            var workbookPart = workbook.AddWorkbookPart();
            {

                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
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

                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "QuotationRequest" };
                sheets.Append(sheet);

                workbook.WorkbookPart.Workbook.Save();
                workbook = AddHeaderToSheet<T>(workbook, ref sheetData, cols);
            }

            return workbook;
        }


        private SpreadsheetDocument CreateExcelSheetGeneric(SpreadsheetDocument workbook, List<string> cols, out SheetData sheetData)
        {
            var workbookPart = workbook.AddWorkbookPart();
            {

                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
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

                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "QuotationRequest" };
                sheets.Append(sheet);

                workbook.WorkbookPart.Workbook.Save();
                workbook = AddHeaderToSheetGeneric(workbook, ref sheetData, cols);
            }

            return workbook;
        }

        private SpreadsheetDocument AddHeaderToSheetGeneric(SpreadsheetDocument workbook, ref SheetData sheetData, List<string> cols)
        {
            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();


            foreach (var name in cols)
            {
                if (!string.IsNullOrEmpty(name))
                    AddCellHeader(name, headerRow);
            }

            sheetData.AppendChild(headerRow);

            workbook.WorkbookPart.Workbook.Save();

            return workbook;
        }


        private SpreadsheetDocument AddHeaderToSheet<T>(SpreadsheetDocument workbook, ref SheetData sheetData, List<string> cols)
        {
            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();



            var propertyNames = typeof(T).GetProperties().Select(x => x.Name).ToList();

            foreach (var name in propertyNames)
            {
                cols.Add(name);
                AddCellHeader(name, headerRow);
            }

            sheetData.AppendChild(headerRow);

            workbook.WorkbookPart.Workbook.Save();

            return workbook;
        }

        private void AddCellHeader(string Name, Row headerRow)
        {
            DocumentFormat.OpenXml.Spreadsheet.Cell cell = GenerateCell(Name);
            headerRow.AppendChild(cell);
        }

        private Cell GenerateCell(string cellValue)
        {
            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(cellValue);
            return cell;
        }

        #endregion


        #region Service Request  

        public byte[] GenerateServiceRequest(List<ServiceRequestLog> request, string name)
        {

            if (request != null)
            {

                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = "Tameenak_Request_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("ReferenceID");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellReferenceID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellReferenceID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellReferenceID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceID");
                        headerRow.AppendChild(cellReferenceID);

                        columns.Add("UserID");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserID");
                        headerRow.AppendChild(cell);


                        columns.Add("UserName");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserName");
                        headerRow.AppendChild(cell2);

                        columns.Add("Method");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Method");
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


                        columns.Add("ServiceURL");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceURL");
                        headerRow.AppendChild(cell7);


                        columns.Add("ErrorCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorCode");
                        headerRow.AppendChild(cell8);

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

                        columns.Add("ServerIP");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServerIP");
                        headerRow.AppendChild(cell12);

                        columns.Add("RequestId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RequestId");
                        headerRow.AppendChild(cell13);

                        columns.Add("ServiceResponseTimeInSeconds");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceResponseTimeInSeconds");
                        headerRow.AppendChild(cell14);

                        columns.Add("Channel");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell15);

                        columns.Add("ServiceErrorCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceErrorCode");
                        headerRow.AppendChild(cell16);

                        columns.Add("ServiceErrorDescription");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceErrorDescription");
                        headerRow.AppendChild(cell17);

                        columns.Add("PolicyNo");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo");
                        headerRow.AppendChild(cell18);

                        columns.Add("VehicleMaker");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMaker");
                        headerRow.AppendChild(cell19);

                        columns.Add("VehicleMakerCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMakerCode");
                        headerRow.AppendChild(cell20);

                        columns.Add("VehicleModel");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModel");
                        headerRow.AppendChild(cell21);

                        columns.Add("VehicleModelCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22x = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell22x.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell22x.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModelCode");
                        headerRow.AppendChild(cell22x);

                        columns.Add("VehicleModelYear");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22xx = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell22xx.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell22xx.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModelYear");
                        headerRow.AppendChild(cell22xx);



                        sheetData.AppendChild(headerRow);



                        foreach (var Request in request)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "ReferenceID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ReferenceId.ToString()); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "UserID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserID.ToString()); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "UserName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserName); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "Method")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Method); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate.ToString()); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "CompanyID")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyID.ToString()); //
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyName); //
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "ServiceURL")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceURL); //
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "ErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail"); //
                                    newRow.AppendChild(cell88);
                                }

                                else if (col == "ErrorDescription")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                    newRow.AppendChild(cell99);
                                }
                                else if (col == "ServiceRequest")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceRequest); //
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "ServiceResponse")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponse); //
                                    newRow.AppendChild(cell111);
                                }
                                else if (col == "ServerIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServerIP); //
                                    newRow.AppendChild(cell122);
                                }
                                else if (col == "RequestId")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell133 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell133.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell133.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.RequestId.ToString()); //
                                    newRow.AppendChild(cell133);
                                }
                                else if (col == "ServiceResponseTimeInSeconds")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell144 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell144.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell144.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponseTimeInSeconds.ToString()); //
                                    newRow.AppendChild(cell144);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell155 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell155.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell155.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                    newRow.AppendChild(cell155);
                                }
                                else if (col == "ServiceErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell666 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell666.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell666.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceErrorCode); //
                                    newRow.AppendChild(cell666);
                                }
                                else if (col == "ServiceErrorDescription")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1777 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1777.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1777.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceErrorDescription); //
                                    newRow.AppendChild(cell1777);
                                }

                                else if (col == "PolicyNo")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1888 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1888.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1888.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyNo); //
                                    newRow.AppendChild(cell1888);
                                }

                                else if (col == "VehicleMaker")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1999 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1999.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1999.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleMaker); //
                                    newRow.AppendChild(cell1999);
                                }

                                else if (col == "VehicleMakerCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1888x = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1888x.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1888x.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleMakerCode); //
                                    newRow.AppendChild(cell1888x);
                                }

                                else if (col == "VehicleModel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cellxs1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cellxs1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cellxs1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleModel); //
                                    newRow.AppendChild(cellxs1);
                                }

                                else if (col == "VehicleModelCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cellxs2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cellxs2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cellxs2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleModelCode); //
                                    newRow.AppendChild(cellxs2);
                                }

                                else if (col == "VehicleModelYear")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cellxs3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cellxs3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cellxs3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleModelYear.HasValue ? Request.VehicleModelYear.Value.ToString() : ""); //
                                    newRow.AppendChild(cellxs3);
                                }

                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }
        public byte[] GenerateServiceRequest(List<IServiceRequestLog> request, string name)
        {

            if (request != null)
            {

                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = "Tameenak_Request_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("ReferenceID");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellReferenceID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellReferenceID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellReferenceID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceID");
                        headerRow.AppendChild(cellReferenceID);

                        columns.Add("ChassisNumber");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellChassisNumber = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellChassisNumber.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellChassisNumber.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ChassisNumber");
                        headerRow.AppendChild(cellChassisNumber);

                        columns.Add("UserID");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserID");
                        headerRow.AppendChild(cell);


                        columns.Add("UserName");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserName");
                        headerRow.AppendChild(cell2);

                        columns.Add("Method");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Method");
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


                        columns.Add("ServiceURL");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceURL");
                        headerRow.AppendChild(cell7);


                        columns.Add("ErrorCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorCode");
                        headerRow.AppendChild(cell8);

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

                        columns.Add("ServerIP");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServerIP");
                        headerRow.AppendChild(cell12);

                        columns.Add("RequestId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RequestId");
                        headerRow.AppendChild(cell13);

                        columns.Add("ServiceResponseTimeInSeconds");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceResponseTimeInSeconds");
                        headerRow.AppendChild(cell14);

                        columns.Add("Channel");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell15);

                        columns.Add("ServiceErrorCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceErrorCode");
                        headerRow.AppendChild(cell16);

                        columns.Add("ServiceErrorDescription");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceErrorDescription");
                        headerRow.AppendChild(cell17);

                        columns.Add("PolicyNo");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo");
                        headerRow.AppendChild(cell18);

                        columns.Add("VehicleMaker");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMaker");
                        headerRow.AppendChild(cell19);

                        columns.Add("VehicleMakerCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMakerCode");
                        headerRow.AppendChild(cell20);

                        columns.Add("VehicleModel");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModel");
                        headerRow.AppendChild(cell21);

                        columns.Add("VehicleModelCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22x = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell22x.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell22x.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModelCode");
                        headerRow.AppendChild(cell22x);

                        columns.Add("VehicleModelYear");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22xx = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell22xx.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell22xx.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModelYear");
                        headerRow.AppendChild(cell22xx);



                        sheetData.AppendChild(headerRow);



                        foreach (var Request in request)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "ReferenceID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ReferenceId); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "ChassisNumber")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell ChassisNumbercell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    ChassisNumbercell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    ChassisNumbercell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ChassisNumber); //
                                    newRow.AppendChild(ChassisNumbercell);
                                }
                                else if (col == "UserID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserID.ToString()); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "UserName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserName); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "Method")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Method); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate.ToString()); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "CompanyID")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyID.ToString()); //
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyName); //
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "ServiceURL")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceURL); //
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "ErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail"); //
                                    newRow.AppendChild(cell88);
                                }

                                else if (col == "ErrorDescription")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                    newRow.AppendChild(cell99);
                                }
                                else if (col == "ServiceRequest")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceRequest); //
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "ServiceResponse")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponse); //
                                    newRow.AppendChild(cell111);
                                }
                                else if (col == "ServerIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServerIP); //
                                    newRow.AppendChild(cell122);
                                }
                                else if (col == "RequestId")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell133 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell133.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell133.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.RequestId.ToString()); //
                                    newRow.AppendChild(cell133);
                                }
                                else if (col == "ServiceResponseTimeInSeconds")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell144 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell144.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell144.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponseTimeInSeconds.ToString()); //
                                    newRow.AppendChild(cell144);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell155 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell155.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell155.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                    newRow.AppendChild(cell155);
                                }
                                else if (col == "ServiceErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell666 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell666.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell666.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceErrorCode); //
                                    newRow.AppendChild(cell666);
                                }
                                else if (col == "ServiceErrorDescription")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1777 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1777.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1777.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceErrorDescription); //
                                    newRow.AppendChild(cell1777);
                                }

                                else if (col == "PolicyNo")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1888 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1888.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1888.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PolicyNo); //
                                    newRow.AppendChild(cell1888);
                                }

                                else if (col == "VehicleMaker")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1999 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1999.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1999.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleMaker); //
                                    newRow.AppendChild(cell1999);
                                }

                                else if (col == "VehicleMakerCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1888x = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1888x.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1888x.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleMakerCode); //
                                    newRow.AppendChild(cell1888x);
                                }

                                else if (col == "VehicleModel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cellxs1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cellxs1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cellxs1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleModel); //
                                    newRow.AppendChild(cellxs1);
                                }

                                else if (col == "VehicleModelCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cellxs2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cellxs2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cellxs2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleModelCode); //
                                    newRow.AppendChild(cellxs2);
                                }

                                else if (col == "VehicleModelYear")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cellxs3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cellxs3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cellxs3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleModelYear.HasValue ? Request.VehicleModelYear.Value.ToString() : ""); //
                                    newRow.AppendChild(cellxs3);
                                }

                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }
        #endregion

        public byte[] ExportInquiryRequestLog(List<InquiryRequestLog> request, string name)
        {

            if (request != null)
            {

                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = "Tameenak_Request_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("Id");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Id");
                        headerRow.AppendChild(cellID);

                        columns.Add("CreatedDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cell);


                        columns.Add("UserId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserId");
                        headerRow.AppendChild(cell2);

                        columns.Add("UserName");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserName");
                        headerRow.AppendChild(cell3);


                        columns.Add("CreatedDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cell4);


                        columns.Add("UserIP");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserIP");
                        headerRow.AppendChild(cell5);


                        columns.Add("UserAgent");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserAgent");
                        headerRow.AppendChild(cell6);


                        columns.Add("ServerIP");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServerIP");
                        headerRow.AppendChild(cell7);


                        columns.Add("Channel");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell8);

                        columns.Add("ErrorCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorCode");
                        headerRow.AppendChild(cell9);

                        columns.Add("ErrorDescription");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorDescription");
                        headerRow.AppendChild(cell10);

                        columns.Add("MethodName");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MethodName");
                        headerRow.AppendChild(cell11);

                        columns.Add("RequestId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RequestId");
                        headerRow.AppendChild(cell12);

                        columns.Add("NIN");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NIN");
                        headerRow.AppendChild(cell13);

                        columns.Add("VehicleId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleId");
                        headerRow.AppendChild(cell14);

                        columns.Add("CityCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CityCode");
                        headerRow.AppendChild(cell15);

                        columns.Add("ExternalId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ExternalId");
                        headerRow.AppendChild(cell16);

                        columns.Add("NajmNcdRefrence");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NajmNcdRefrence");
                        headerRow.AppendChild(cell17);

                        columns.Add("NajmNcdFreeYears");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NajmNcdFreeYears");
                        headerRow.AppendChild(cell18);

                        columns.Add("ServiceRequest");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceRequest");
                        headerRow.AppendChild(cell19);

                        columns.Add("MobileVersion");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MobileVersion");
                        headerRow.AppendChild(cell20);

                        sheetData.AppendChild(headerRow);

                        foreach (var Request in request)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "Id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Id.ToString()); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate.ToString()); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "UserId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserId); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "UserName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserName); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "UserIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserIP); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "UserAgent")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserAgent); //
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "ServerIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServerIP); //
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "ErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail"); //
                                    newRow.AppendChild(cell88);
                                }

                                else if (col == "ErrorDescription")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                    newRow.AppendChild(cell99);
                                }
                                else if (col == "MethodName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.MethodName); //
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "RequestId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.RequestId.ToString()); //
                                    newRow.AppendChild(cell111);
                                }
                                else if (col == "NIN")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.NIN); //
                                    newRow.AppendChild(cell122);
                                }
                                else if (col == "VehicleId")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell133 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell133.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell133.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleId); //
                                    newRow.AppendChild(cell133);
                                }
                                else if (col == "CityCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell144 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell144.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell144.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CityCode.ToString()); //
                                    newRow.AppendChild(cell144);
                                }
                                else if (col == "ExternalId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell155 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell155.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell155.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ExternalId); //
                                    newRow.AppendChild(cell155);
                                }
                                else if (col == "NajmNcdRefrence")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell666 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell666.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell666.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.NajmNcdRefrence); //
                                    newRow.AppendChild(cell666);
                                }
                                else if (col == "NajmNcdRefrence")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1777 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1777.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1777.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.NajmNcdRefrence); //
                                    newRow.AppendChild(cell1777);
                                }

                                else if (col == "NajmNcdFreeYears")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1888 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1888.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1888.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.NajmNcdFreeYears.ToString()); //
                                    newRow.AppendChild(cell1888);
                                }

                                else if (col == "ServiceRequest")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1999 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1999.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1999.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceRequest); //
                                    newRow.AppendChild(cell1999);
                                }
                                else if (col == "MobileVersion")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2000 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell2000.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell2000.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.MobileVersion); //
                                    newRow.AppendChild(cell2000);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        public byte[] ExportCheckoutRequestLog(List<CheckoutRequestLog> request, string name)
        {
            if (request != null)
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = "Tameenak_Request_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("Reference Number");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reference Number");
                        headerRow.AppendChild(cellID);

                        columns.Add("VehicleId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleId");
                        headerRow.AppendChild(cell14);

                        columns.Add("NIN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NIN");
                        headerRow.AppendChild(cell13);

                        columns.Add("CreatedDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cell);

                        columns.Add("MethodName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MethodName");
                        headerRow.AppendChild(cell11);

                        columns.Add("PaymentName");
                        cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaymentName");
                        headerRow.AppendChild(cell11);

                        columns.Add("Amount");
                        cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Amount");
                        headerRow.AppendChild(cell11);

                        columns.Add("Channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell8);

                        columns.Add("CompanyID");
                        cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompanyID");
                        headerRow.AppendChild(cell8);

                        columns.Add("CompanyName");
                        cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompanyName");
                        headerRow.AppendChild(cell8);

                        columns.Add("ErrorCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorCode");
                        headerRow.AppendChild(cell9);

                        columns.Add("ErrorDescription");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorDescription");
                        headerRow.AppendChild(cell10);

                        columns.Add("UserId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserId");
                        headerRow.AppendChild(cell2);

                        columns.Add("UserName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserName");
                        headerRow.AppendChild(cell3);

                        columns.Add("UserIP");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserIP");
                        headerRow.AppendChild(cell5);

                        columns.Add("UserAgent");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserAgent");
                        headerRow.AppendChild(cell6);

                        columns.Add("ServerIP");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServerIP");
                        headerRow.AppendChild(cell7);

                        columns.Add("ServiceRequest");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceRequest");
                        headerRow.AppendChild(cell19);

                        sheetData.AppendChild(headerRow);

                        foreach (var Request in request)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "Reference Number")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ReferenceId); 
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "VehicleId")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell133 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell133.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell133.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.VehicleId);
                                    newRow.AppendChild(cell133);
                                }
                                else if (col == "NIN")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.DriverNin);
                                    newRow.AppendChild(cell122);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate.ToString()); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "MethodName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.MethodName);
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "PaymentName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.PaymentMethod);
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "Amount")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Amount?.ToString());
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "CompanyID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyId?.ToString()); 
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyName);
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "ErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail");
                                    newRow.AppendChild(cell88);
                                }

                                else if (col == "ErrorDescription")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription);
                                    newRow.AppendChild(cell99);
                                }
                                else if (col == "UserId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserId); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "UserName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserName); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "UserIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserIP); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "UserAgent")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserAgent); //
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "ServerIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServerIP); //
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "ServiceRequest")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1999 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1999.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1999.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceRequest); //
                                    newRow.AppendChild(cell1999);
                                }    
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #region Ticket

        public byte[] ExportTicketLog(List<TicketLogModel> logs, string name)
        {
            if (logs != null)
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = name + "_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("Id");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Id");
                        headerRow.AppendChild(cellID);

                        columns.Add("CreatedDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cell);


                        columns.Add("MethodName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MethodName");
                        headerRow.AppendChild(cell2);

                        columns.Add("UserAgent");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UsUserAgenterName");
                        headerRow.AppendChild(cell3);

                        columns.Add("Channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell4);

                        columns.Add("ErrorCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorCode");
                        headerRow.AppendChild(cell5);

                        columns.Add("ErrorDescription");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorDescription");
                        headerRow.AppendChild(cell6);

                        columns.Add("DriverNin");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverNin");
                        headerRow.AppendChild(cell7);

                        columns.Add("ReferenceId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                        headerRow.AppendChild(cell8);

                        columns.Add("UserId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserId");
                        headerRow.AppendChild(cell9);

                        columns.Add("UserIP");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserIP");
                        headerRow.AppendChild(cell10);

                        columns.Add("ServerIP");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServerIP");
                        headerRow.AppendChild(cell11);

                        sheetData.AppendChild(headerRow);

                        foreach (var Request in logs)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "Id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Id.ToString()); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate.ToString()); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "MethodName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.MethodName); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "UserAgent")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserAgent); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "ErrorCode")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail"); //
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "ErrorDescription")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "DriverNin")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.DriverNin); //
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "ReferenceId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ReferenceId); //
                                    newRow.AppendChild(cell88);
                                }

                                else if (col == "UserId")
                                {
                                    /// Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserId); //
                                    newRow.AppendChild(cell99);
                                }
                                else if (col == "UserIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserIP); //
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "ServerIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServerIP); //
                                    newRow.AppendChild(cell111);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #endregion

        #region Vehicel Makers

        public byte[] ExportVehicleMakers(List<VehicleMakerModel> makers, string name)
        {
            if (makers != null)
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = name + "_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("Code");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Code");
                        headerRow.AppendChild(cellID);

                        columns.Add("English Description");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("English Description");
                        headerRow.AppendChild(cell);

                        columns.Add("Arabic Description");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Arabic Description");
                        headerRow.AppendChild(cell2);

                        sheetData.AppendChild(headerRow);

                        foreach (var maker in makers)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "Code")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(maker.Code.ToString()); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "English Description")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(maker.EnglishDescription); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "Arabic Description")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(maker.ArabicDescription); //
                                    newRow.AppendChild(cell22);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        public byte[] ExportVehicleMakerModels(List<VehicleMakerModelsModel> makers, string name)
        {
            if (makers != null)
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = name + "_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("Code");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Code");
                        headerRow.AppendChild(cellID);

                        columns.Add("Maker Code");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Maker Code");
                        headerRow.AppendChild(cell);

                        columns.Add("English Description");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("English Description");
                        headerRow.AppendChild(cell2);

                        columns.Add("Arabic Description");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Arabic Description");
                        headerRow.AppendChild(cell3);

                        sheetData.AppendChild(headerRow);

                        foreach (var maker in makers)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "Code")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(maker.Code.ToString()); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "Maker Code")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(maker.MakerCode.ToString()); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "English Description")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(maker.EnglishDescription); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "Arabic Description")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(maker.ArabicDescription); //
                                    newRow.AppendChild(cell33);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #endregion

        #region Yakeen City Center

        public byte[] ExportYakeenCityCenters(List<YakeenCityCenterModel> cities, string name)
        {
            if (cities != null)
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = name + "_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("Id");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Id");
                        headerRow.AppendChild(cellID);

                        columns.Add("CityId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CityId");
                        headerRow.AppendChild(cell);

                        columns.Add("CityName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CityName");
                        headerRow.AppendChild(cell2);

                        columns.Add("EnglishName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EnglishName");
                        headerRow.AppendChild(cell3);

                        columns.Add("ZipCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ZipCode");
                        headerRow.AppendChild(cell4);

                        columns.Add("RegionId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RegionId");
                        headerRow.AppendChild(cell5);

                        columns.Add("RegionArabicName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RegionArabicName");
                        headerRow.AppendChild(cell6);

                        columns.Add("RegionEnglishName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RegionEnglishName");
                        headerRow.AppendChild(cell7);

                        columns.Add("ElmCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ElmCode");
                        headerRow.AppendChild(cell8);

                        sheetData.AppendChild(headerRow);

                        foreach (var item in cities)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "Id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Id.ToString()); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "CityId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CityId.ToString()); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "CityName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CityName); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "EnglishName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.EnglishName); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "ZipCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ZipCode.ToString()); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "RegionId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.RegionId.ToString()); //
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "RegionArabicName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.RegionArabicName); //
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "RegionEnglishName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.RegionEnglishName); //
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "ElmCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ElmCode.ToString()); //
                                    newRow.AppendChild(cell88);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #endregion

        #region promotion offers discounts

        public byte[] GenerateOffersExcel(List<DeservingDiscount> offers)
        {
            if (offers != null)
            {
                DateTime dt = DateTime.Now;
                string fileName = "Offers_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();

                        columns.Add("Id");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Id");
                        headerRow.AppendChild(cellID);

                        columns.Add("NationalId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CityId");
                        headerRow.AppendChild(cell);

                        columns.Add("Name");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CityName");
                        headerRow.AppendChild(cell2);

                        columns.Add("Mobile");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EnglishName");
                        headerRow.AppendChild(cell3);

                        columns.Add("ExpiryDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ZipCode");
                        headerRow.AppendChild(cell4);

                        sheetData.AppendChild(headerRow);

                        foreach (var item in offers)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "Id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Id.ToString()); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "NationalId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NationalId); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "Name")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Name); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "Mobile")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Mobile); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "ExpiryDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ExpiryDate.ToString()); //
                                    newRow.AppendChild(cell44);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #endregion

        #region New Success Policies Info

        public byte[] GenerateExcelSuccessPoliciesInfo(List<SuccessPoliciesInfoListingModel> policies, string lang)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SuccessPoliciesInfo" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();

                    columns.Add("PolicyNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo");
                    headerRow.AppendChild(cell1);

                    columns.Add("InsuranceCompanyName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuranceCompanyName");
                    headerRow.AppendChild(cell2);

                    columns.Add("ReferenceNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceNo");
                    headerRow.AppendChild(cell3);

                    columns.Add("NajmStatusName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NajmStatusName");
                    headerRow.AppendChild(cell4);


                    columns.Add("InvoiceNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InvoiceNo");
                    headerRow.AppendChild(cell5);


                    columns.Add("PaymentMethodName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaymentMethodName");
                    headerRow.AppendChild(cell6);

                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (SuccessPoliciesInfoListingModel policy in policies)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "PolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyNo);
                                newRow.AppendChild(cell11);
                            }
                            else if (col == "InsuranceCompanyName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(
                                    (lang == "en") ? policy.InsuranceCompanyNameEn : policy.InsuranceCompanyNameAr);
                                newRow.AppendChild(cell22);
                            }
                            else if (col == "ReferenceNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ReferenceNo);
                                newRow.AppendChild(cell33);
                            }
                            else if (col == "NajmStatusName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(
                                    (lang == "en") ? policy.NajmStatusNameEN : policy.NajmStatusNameAr);
                                newRow.AppendChild(cell44);
                            }
                            else if (col == "InvoiceNo")
                            {
                                // Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InvoiceNo.ToString());
                                newRow.AppendChild(cell55);
                            }
                            else if (col == "PaymentMethodName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(
                                    (lang == "en") ? policy.PaymentMethodNameEN : policy.PaymentMethodNameAr);
                                newRow.AppendChild(cell66);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion

        #region Export sms logs

        public byte[] GenerateSmsServiceLogExcel(List<SMSLog> smss)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SMS-Logs" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();

                    columns.Add("MobileNumber");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MobileNumber");
                    headerRow.AppendChild(cell1);

                    columns.Add("SMSMessage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SMSMessage");
                    headerRow.AppendChild(cell2);

                    columns.Add("Status");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Status");
                    headerRow.AppendChild(cell3);

                    columns.Add("ErrorDescription");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorDescription");
                    headerRow.AppendChild(cell4);

                    columns.Add("Method");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Method");
                    headerRow.AppendChild(cell5);

                    columns.Add("CreatedDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                    headerRow.AppendChild(cell6);

                    columns.Add("UserIP");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserIP");
                    headerRow.AppendChild(cell7);

                    columns.Add("ServerIP");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServerIP");
                    headerRow.AppendChild(cell8);

                    columns.Add("UserAgent");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserAgent");
                    headerRow.AppendChild(cell9);

                    columns.Add("SMSProvider");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SMSProvider");
                    headerRow.AppendChild(cell10);

                    columns.Add("ServiceURL");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceURL");
                    headerRow.AppendChild(cell11);

                    columns.Add("ServiceRequest");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceRequest");
                    headerRow.AppendChild(cell12);

                    columns.Add("ServiceResponse");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceResponse");
                    headerRow.AppendChild(cell13);

                    columns.Add("ServiceResponseTimeInSeconds");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceResponseTimeInSeconds");
                    headerRow.AppendChild(cell14);

                    columns.Add("ReferenceId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                    headerRow.AppendChild(cell15);

                    columns.Add("Module");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Module");
                    headerRow.AppendChild(cell16);

                    columns.Add("Channel");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                    headerRow.AppendChild(cell17);

                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (SMSLog sms in smss)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "MobileNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell50 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell50.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell50.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.MobileNumber);
                                newRow.AppendChild(cell50);
                            }
                            else if (col == "SMSMessage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.SMSMessage);
                                newRow.AppendChild(cell22);
                            }
                            else if (col == "Status")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue((sms.ErrorCode == 100 || sms.ErrorCode == 0) ? "Success" : "Fail");
                                newRow.AppendChild(cell33);
                            }

                            else if (col == "ErrorDescription")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.ErrorDescription);
                                newRow.AppendChild(cell44);
                            }
                            else if (col == "Method")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.Method);
                                newRow.AppendChild(cell55);
                            }
                            else if (col == "CreatedDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.CreatedDate.ToString());
                                newRow.AppendChild(cell66);
                            }
                            else if (col == "UserIP")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell67= new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell67.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell67.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.UserIP);
                                newRow.AppendChild(cell67);
                            }
                            else if (col == "ServerIP")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell68 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell68.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell68.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.ServerIP);
                                newRow.AppendChild(cell68);
                            }
                            else if (col == "UserAgent")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell80 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell80.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell80.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.UserAgent);
                                newRow.AppendChild(cell80);
                            }
                            else if (col == "SMSProvider")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell69 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell69.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell69.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.SMSProvider);
                                newRow.AppendChild(cell69);
                            }
                            else if (col == "ServiceURL")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell70 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell70.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell70.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.ServiceURL);
                                newRow.AppendChild(cell70);
                            }
                            else if (col == "ServiceRequest")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell71 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell71.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell71.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.ServiceRequest);
                                newRow.AppendChild(cell71);
                            }
                            else if (col == "ServiceResponse")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell72 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell72.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell72.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.ServiceResponse);
                                newRow.AppendChild(cell72);
                            }
                            else if (col == "ServiceResponseTimeInSeconds")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell73 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell73.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell73.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.ServiceResponseTimeInSeconds.ToString());
                                newRow.AppendChild(cell73);
                            }
                            else if (col == "ReferenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell74 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell74.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell74.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.ReferenceId);
                                newRow.AppendChild(cell74);
                            }
                            else if (col == "Module")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell75 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell75.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell75.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.Module);
                                newRow.AppendChild(cell75);
                            }
                            else if (col == "Channel")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell76 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell76.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell76.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(sms.Channel);
                                newRow.AppendChild(cell76);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion

        #region Export sms logs

        public byte[] GeneratePolicyInformationForRoadAssistance(List<PolicyInformation> policies)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Policies-Information" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();

                    columns.Add("PolicyNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo");
                    headerRow.AppendChild(cell1);

                    columns.Add("Policy start date");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy start date");
                    headerRow.AppendChild(cell2);

                    columns.Add("Policy end date");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy end date");
                    headerRow.AppendChild(cell3);

                    columns.Add("vin");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("vin");
                    headerRow.AppendChild(cell5);

                    columns.Add("Vehicle Registration serial number");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Vehicle Registration serial number");
                    headerRow.AppendChild(cell6);

                    columns.Add("owner first name");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("owner first name");
                    headerRow.AppendChild(cell7);

                    columns.Add("owner last name");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("owner last name");
                    headerRow.AppendChild(cell8);

                    columns.Add("owner mobile phone");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("owner mobile phone");
                    headerRow.AppendChild(cell9);

                    columns.Add("Plate Info");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Plate Info");
                    headerRow.AppendChild(cell10);

                    columns.Add("Make");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Make");
                    headerRow.AppendChild(cell11);

                    columns.Add("Model");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Model");
                    headerRow.AppendChild(cell12);

                    columns.Add("Model Year");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Model Year");
                    headerRow.AppendChild(cell13);


                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (PolicyInformation policy in policies)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "PolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyNo);
                                newRow.AppendChild(cell14);
                            }
                            else if (col == "Policy start date")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyEffectiveDate.ToString());
                                newRow.AppendChild(cell15);
                            }
                            else if (col == "Policy end date")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyExpiryDate.ToString());
                                newRow.AppendChild(cell16);
                            }

                            else if (col == "vin")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ChassisNumber);
                                newRow.AppendChild(cell17);
                            }
                            else if (col == "Vehicle Registration serial number")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleId);
                                newRow.AppendChild(cell18);
                            }
                            else if (col == "owner first name")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ownerFirstName);
                                newRow.AppendChild(cell19);
                            }
                            else if (col == "owner last name")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.OwnerLastName);
                                newRow.AppendChild(cell20);
                            }
                            else if (col == "owner mobile phone")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.OwnerMobilePhone);
                                newRow.AppendChild(cell21);
                            }
                            else if (col == "Plate Info")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PlateInfo);
                                newRow.AppendChild(cell22);
                            }
                            else if (col == "Make")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell23 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell23.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell23.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Make);
                                newRow.AppendChild(cell23);
                            }
                            else if (col == "Model")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell24 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell24.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell24.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Model);
                                newRow.AppendChild(cell24);
                            }
                            else if (col == "Model Year")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell25 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell25.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ModelYear.ToString());
                                newRow.AppendChild(cell25);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion

        #region Export Sama Report Excel

        public byte[] GenerateSamaReportPoliciesExcel(List<SamaReportPoliciesListingModel> policies, string lang)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SuccessPoliciesInfo" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();

                    columns.Add("PolicyNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo");
                    headerRow.AppendChild(cell1);

                    columns.Add("ReferenceId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                    headerRow.AppendChild(cell2);

                    columns.Add("PolicyIssueDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyIssueDate");
                    headerRow.AppendChild(cell3);

                    columns.Add("PolicyEffectiveDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyEffectiveDate");
                    headerRow.AppendChild(cell4);

                    columns.Add("PolicyExpiryDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell104 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell104.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell104.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyExpiryDate");
                    headerRow.AppendChild(cell104);

                    columns.Add("Channel");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1044 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1044.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1044.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                    headerRow.AppendChild(cell1044);

                    columns.Add("NajmStatus");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NajmStatus");
                    headerRow.AppendChild(cell5);


                    columns.Add("InsuranceCompanyName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuranceCompanyName");
                    headerRow.AppendChild(cell6);

                    columns.Add("CheckoutEmail");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CheckoutEmail");
                    headerRow.AppendChild(cell7);

                    columns.Add("CheckoutPhone");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CheckoutPhone");
                    headerRow.AppendChild(cell8);

                    columns.Add("IBAN");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IBAN");
                    headerRow.AppendChild(cell9);

                    columns.Add("InvoiceNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InvoiceNo");
                    headerRow.AppendChild(cell10);

                    columns.Add("InvoiceDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InvoiceDate");
                    headerRow.AppendChild(cell11);

                    columns.Add("InvoiceDueDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InvoiceDueDate");
                    headerRow.AppendChild(cell12);

                    columns.Add("ProductPrice");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ProductPrice");
                    headerRow.AppendChild(cell13);

                    columns.Add("fees");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("fees");
                    headerRow.AppendChild(cell14);

                    columns.Add("vat");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("vat");
                    headerRow.AppendChild(cell15);

                    columns.Add("SubTotalPrice");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SubTotalPrice");
                    headerRow.AppendChild(cell16);

                    columns.Add("TotalPrice");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TotalPrice");
                    headerRow.AppendChild(cell17);

                    columns.Add("ExtraPremiumPrice");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ExtraPremiumPrice");
                    headerRow.AppendChild(cell18);

                    columns.Add("DiscountPercentageValue");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DiscountPercentageValue");
                    headerRow.AppendChild(cell19);

                    columns.Add("SchemeDiscount");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SchemeDiscount");
                    headerRow.AppendChild(cell20);

                    columns.Add("SchemeDiscountPercentage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SchemeDiscountPercentage");
                    headerRow.AppendChild(cell21);

                    columns.Add("LoyaltyDiscountValue");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("LoyaltyDiscountValue");
                    headerRow.AppendChild(cell22);

                    columns.Add("LoyaltyDiscountPercentage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell23 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell23.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell23.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("LoyaltyDiscountPercentage");
                    headerRow.AppendChild(cell23);

                    columns.Add("InsuranceType");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell117 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell117.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell117.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuranceType");
                    headerRow.AppendChild(cell117);

                    columns.Add("DeductibleValue");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell118 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell118.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell118.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DeductibleValue");
                    headerRow.AppendChild(cell118);

                    columns.Add("RepairType");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell46 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell46.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell46.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RepairType");
                    headerRow.AppendChild(cell46);

                    columns.Add("MileageExpectedPerYearId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell47 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell47.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell47.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MileageExpectedPerYearId");
                    headerRow.AppendChild(cell47);

                    columns.Add("MileageExpectedAnnual");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell119 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell119.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell119.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MileageExpectedAnnual");
                    headerRow.AppendChild(cell119);

                    columns.Add("Covarage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell120 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell120.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell120.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Covarage");
                    headerRow.AppendChild(cell120);

                    columns.Add("MainDriverNin");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell24 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell24.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell24.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MainDriverNin");
                    headerRow.AppendChild(cell24);

                    columns.Add("MainDriverName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell25 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell25.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MainDriverName");
                    headerRow.AppendChild(cell25);

                    columns.Add("InsuredBirthDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell32 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell32.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell32.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredBirthDate");
                    headerRow.AppendChild(cell32);

                    columns.Add("InsuredBirthDateH");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredBirthDateH");
                    headerRow.AppendChild(cell33);

                    columns.Add("InsuredGender");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell36 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell36.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell36.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredGender");
                    headerRow.AppendChild(cell36);

                    columns.Add("InsuredEducationId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell37 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell37.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell37.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredEducationId");
                    headerRow.AppendChild(cell37);

                    columns.Add("InsuredSocialStatus");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell38 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell38.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell38.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredSocialStatus");
                    headerRow.AppendChild(cell38);

                    columns.Add("InsuredChildrenBelow16Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell39 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell39.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell39.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredChildrenBelow16Years");
                    headerRow.AppendChild(cell39);

                    columns.Add("MainDriverOccupation");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell27 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell27.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell27.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MainDriverOccupation");
                    headerRow.AppendChild(cell27);

                    columns.Add("MainDriverOccupationCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell28 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell28.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell28.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MainDriverOccupationCode");
                    headerRow.AppendChild(cell28);


                    columns.Add("InsuredNationalAddress");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell40 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell40.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell40.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredNationalAddress");
                    headerRow.AppendChild(cell40);

                    columns.Add("InsuredWorkCity");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell41 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell41.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell41.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredWorkCity");
                    headerRow.AppendChild(cell41);

                    columns.Add("DriverLicenseType");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell121 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell121.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell121.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverLicenseType");
                    headerRow.AppendChild(cell121);

                    columns.Add("SaudiLicenseHeldYears");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SaudiLicenseHeldYears");
                    headerRow.AppendChild(cell122);

                    columns.Add("DriverExtraLicenses");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell123 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell123.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell123.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverExtraLicenses");
                    headerRow.AppendChild(cell123);

                    columns.Add("NajmNcdFreeYears");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell42 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell42.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell42.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NajmNcdFreeYears");
                    headerRow.AppendChild(cell42);

                    columns.Add("NOALast5Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell43 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell43.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell43.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NOALast5Years");
                    headerRow.AppendChild(cell43);

                    columns.Add("NOCLast5Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NOCLast5Years");
                    headerRow.AppendChild(cell44);


                    columns.Add("DriverViolations");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell126 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell126.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell126.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverViolations");
                    headerRow.AppendChild(cell126);

                    columns.Add("DriverMedicalCondition");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell127 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell127.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell127.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverMedicalCondition");
                    headerRow.AppendChild(cell127);

                    columns.Add("City");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell26 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell26.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell26.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("City");
                    headerRow.AppendChild(cell26);


                    columns.Add("PromotionProgram");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell29 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell29.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell29.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PromotionProgram");
                    headerRow.AppendChild(cell29);

                    columns.Add("InsuredID");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell30 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell30.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell30.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredID");
                    headerRow.AppendChild(cell30);

                    columns.Add("InsuredNationality");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell31 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell31.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell31.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredNationality");
                    headerRow.AppendChild(cell31);

                    columns.Add("EXcess");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell45 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell45.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell45.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EXcess");
                    headerRow.AppendChild(cell45);

                    columns.Add("AddditionalDriverOneNin");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell59 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell59.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell59.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneNin");
                    headerRow.AppendChild(cell59);

                    columns.Add("AddditionalDriverOneName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell58 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell58.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell58.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneName");
                    headerRow.AppendChild(cell58);

                    columns.Add("AddditionalDriverOneDateOfBirthG");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell60 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell60.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell60.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneDateOfBirthG");
                    headerRow.AppendChild(cell60);

                    columns.Add("AddditionalDriverOneDateOfBirthH");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell61 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell61.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell61.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneDateOfBirthH");
                    headerRow.AppendChild(cell61);

                    columns.Add("AddditionalDriverOneGenderId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell62 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell62.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell62.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneGenderId");
                    headerRow.AppendChild(cell62);

                    columns.Add("AddditionalDriverOneEducationId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell63 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell63.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell63.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneEducationId");
                    headerRow.AppendChild(cell63);

                    columns.Add("AddditionalDriverOneSocialStatusId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell64 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell64.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell64.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneSocialStatusId");
                    headerRow.AppendChild(cell64);

                    columns.Add("AddditionalDriverOneChildrenBelow16Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell65 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell65.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell65.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneChildrenBelow16Years");
                    headerRow.AppendChild(cell65);

                    columns.Add("AddditionalDriverOneOccupation");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell67 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell67.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell67.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneOccupation");
                    headerRow.AppendChild(cell67);

                    columns.Add("AddditionalDriverOneOccupationCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell68 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell68.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell68.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneOccupationCode");
                    headerRow.AppendChild(cell68);

                    columns.Add("AddditionalDriverOneAddress");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell74 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell74.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell74.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneAddress");
                    headerRow.AppendChild(cell74);

                    columns.Add("AddditionalDriverOneWorkCity");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell699 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell699.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell699.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneWorkCity");
                    headerRow.AppendChild(cell699);

                    columns.Add("AddditionalDriverOneDriverLicense");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell75 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell75.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell75.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneDriverLicense");
                    headerRow.AppendChild(cell75);

                    columns.Add("AddditionalDriverOneSaudiLicenseHeldYears");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell73 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell73.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell73.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneSaudiLicenseHeldYears");
                    headerRow.AppendChild(cell73);

                    columns.Add("AddditionalDriverOneDriverExtraLicense");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell76 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell76.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell76.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneDriverExtraLicense");
                    headerRow.AppendChild(cell76);

                    columns.Add("AddditionalDriverOneNCDFreeYears");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell70 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell70.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell70.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneNCDFreeYears");
                    headerRow.AppendChild(cell70);


                    columns.Add("AdditionalDriverOneNOALast5Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell69 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell69.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell69.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AdditionalDriverOneNOALast5Years");
                    headerRow.AppendChild(cell69);

                    columns.Add("AddditionalDriverOneNOCLast5Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell669 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell669.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell669.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneNOCLast5Years");
                    headerRow.AppendChild(cell669);

                    columns.Add("AddditionalDriverOneViolation");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell72 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell72.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell72.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneViolation");
                    headerRow.AppendChild(cell72);

                    columns.Add("AddditionalDriverOneMedicalCondition");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell71 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell71.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell71.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneMedicalCondition");
                    headerRow.AppendChild(cell71);

                    columns.Add("AddditionalDriverTwoNin");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell78 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell78.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell78.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoNin");
                    headerRow.AppendChild(cell78);

                    columns.Add("AddditionalDriverTwoName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoName");
                    headerRow.AppendChild(cell77);

                    columns.Add("AddditionalDriverTwoDateOfBirthG");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell79 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell79.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell79.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoDateOfBirthG");
                    headerRow.AppendChild(cell79);

                    columns.Add("AddditionalDriverTwoDateOfBirthH");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell80 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell80.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell80.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoDateOfBirthH");
                    headerRow.AppendChild(cell80);

                    columns.Add("AddditionalDriverTwoGenderId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell81 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell81.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell81.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoGenderId");
                    headerRow.AppendChild(cell81);

                    columns.Add("AddditionalDriverTwoEducationId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell82 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell82.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell82.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoEducationId");
                    headerRow.AppendChild(cell82);

                    columns.Add("AddditionalDriverTwoSocialStatusId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell83 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell83.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell83.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoSocialStatusId");
                    headerRow.AppendChild(cell83);

                    columns.Add("AddditionalDriverTwoChildrenBelow16Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell84 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell84.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell84.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoChildrenBelow16Years");
                    headerRow.AppendChild(cell84);

                    columns.Add("AddditionalDriverTwoOccupation");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell85 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell85.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell85.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoOccupation");
                    headerRow.AppendChild(cell85);

                    columns.Add("AddditionalDriverTwoOccupationCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell86 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell86.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell86.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoOccupationCode");
                    headerRow.AppendChild(cell86);

                    columns.Add("AddditionalDriverTwoAddress");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell94 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell94.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell94.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoAddress");
                    headerRow.AppendChild(cell94);

                    columns.Add("AddditionalDriverTwoWorkCity");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell89 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell89.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell89.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoWorkCity");
                    headerRow.AppendChild(cell89);

                    columns.Add("AddditionalDriverTwoDriverLicense");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell95 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell95.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell95.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoDriverLicense");
                    headerRow.AppendChild(cell95);

                    columns.Add("AddditionalDriverTwoSaudiLicenseHeldYears");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell93 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell93.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell93.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoSaudiLicenseHeldYears");
                    headerRow.AppendChild(cell93);

                    columns.Add("AddditionalDriverTwoDriverExtraLicense");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell96 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell96.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell96.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoDriverExtraLicense");
                    headerRow.AppendChild(cell96);


                    columns.Add("AddditionalDriverTwoNCDFreeYears");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoNCDFreeYears");
                    headerRow.AppendChild(cell88);

                    columns.Add("AdditionalDriverTwoNOALast5Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell87 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell87.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell87.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AdditionalDriverTwoNOALast5Years");
                    headerRow.AppendChild(cell87);

                    columns.Add("AddditionalDriverTwoNOCLast5Years");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell817 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell817.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell817.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoNOCLast5Years");
                    headerRow.AppendChild(cell817);

                    columns.Add("AddditionalDriverTwoViolation");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell92 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell92.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell92.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoViolation");
                    headerRow.AppendChild(cell92);

                    columns.Add("AddditionalDriverTwoMedicalCondition");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell91 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell91.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell91.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoMedicalCondition");
                    headerRow.AppendChild(cell91);

                    columns.Add("VehicleMakeModel");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell51 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell51.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell51.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMakeModel");
                    headerRow.AppendChild(cell51);


                    columns.Add("ManufactureYear");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell52 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell52.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell52.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ManufactureYear");
                    headerRow.AppendChild(cell52);

                    columns.Add("SumInsured");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell53 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell53.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell53.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SumInsured");
                    headerRow.AppendChild(cell53);

                    columns.Add("VehicleUseId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell54 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell54.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell54.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleUseId");
                    headerRow.AppendChild(cell54);

                    columns.Add("TransmissionTypeId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("TransmissionTypeId");
                    headerRow.AppendChild(cell55);

                    columns.Add("ParkingLocationId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell56 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell56.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell56.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ParkingLocationId");
                    headerRow.AppendChild(cell56);

                    columns.Add("VehicleModificationDetails");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell57 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell57.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell57.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModificationDetails");
                    headerRow.AppendChild(cell57);

                    columns.Add("CustomCardNumber");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell49 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell49.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell49.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CustomCardNumber");
                    headerRow.AppendChild(cell49);

                    columns.Add("SequenceNumber");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell50 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell50.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell50.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SequenceNumber");
                    headerRow.AppendChild(cell50);

                    columns.Add("VehicleBodyCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell231 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell231.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell231.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleBodyCode");
                    headerRow.AppendChild(cell231);

                    columns.Add("ArabicVehicleBody");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell232 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell232.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell232.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ArabicVehicleBody");
                    headerRow.AppendChild(cell232);

                    columns.Add("EnglishVehicleBody");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell233 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell233.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell233.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EnglishVehicleBody");
                    headerRow.AppendChild(cell233);

                    columns.Add("AddditionalDriverOneNationalityCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell700 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell700.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell700.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverOneNationalityCode");
                    headerRow.AppendChild(cell700);

                    columns.Add("AddditionalDriverTwoNationalityCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell90 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell90.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell90.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AddditionalDriverTwoNationalityCode");
                    headerRow.AppendChild(cell90);

                    columns.Add("InsuredOccupation");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell34 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell34.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell34.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredOccupation");
                    headerRow.AppendChild(cell34);

                    columns.Add("InsuredOccupationCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell35 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell35.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell35.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredOccupationCode");
                    headerRow.AppendChild(cell35);

                    columns.Add("CarPlateNumber");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell48 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell48.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell48.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CarPlateNumber");
                    headerRow.AppendChild(cell48);

                    columns.Add("VehicleLoad");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell128 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell128.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell128.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleLoad");
                    headerRow.AppendChild(cell128);

                    columns.Add("EngineSize");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell129 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell129.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell129.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EngineSize");
                    headerRow.AppendChild(cell129);

                    columns.Add("ChassisNumber");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell130 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell130.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell130.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ChassisNumber");
                    headerRow.AppendChild(cell130);

                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (var policy in policies)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "PolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell01 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell01.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell01.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyNo);
                                newRow.AppendChild(cell01);
                            }
                            else if (col == "ReferenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell02 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell02.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell02.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ReferenceId);
                                newRow.AppendChild(cell02);
                            }
                            else if (col == "PolicyIssueDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell03 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell03.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell03.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyIssueDate?.ToString());
                                newRow.AppendChild(cell03);
                            }
                            else if (col == "PolicyEffectiveDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell04 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell04.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell04.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyEffectiveDate?.ToString());
                                newRow.AppendChild(cell04);
                            }
                            else if (col == "PolicyExpiryDate")
                            {
                                // Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell05 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell05.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell05.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyExpiryDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell05);
                            }
                            else if (col == "Channel")
                            {
                                // Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell055 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell055.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell055.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Channel);
                                newRow.AppendChild(cell055);
                            }
                            else if (col == "NajmStatus")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell06 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell06.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell06.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.NajmStatus);
                                newRow.AppendChild(cell06);
                            }
                            else if (col == "InsuranceCompanyName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell07 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell07.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell07.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuranceCompanyName);
                                newRow.AppendChild(cell07);
                            }
                            else if (col == "CheckoutEmail")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell08 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell08.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell08.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CheckoutEmail);
                                newRow.AppendChild(cell08);
                            }
                            else if (col == "CheckoutPhone")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell09 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell09.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell09.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CheckoutPhone);
                                newRow.AppendChild(cell09);
                            }
                            else if (col == "IBAN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell010 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell010.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell010.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.IBAN);
                                newRow.AppendChild(cell010);
                            }
                            else if (col == "InvoiceNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell011 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell011.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell011.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InvoiceNo.ToString());
                                newRow.AppendChild(cell011);
                            }
                            else if (col == "InvoiceDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell012 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell012.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell012.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InvoiceDate.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell012);
                            }
                            else if (col == "InvoiceDueDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell013 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell013.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell013.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InvoiceDueDate.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell013);
                            }
                            else if (col == "ProductPrice")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell014 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell014.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell014.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ProductPrice?.ToString());
                                newRow.AppendChild(cell014);
                            }
                            else if (col == "fees")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell015 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell015.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell015.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.fees?.ToString());
                                newRow.AppendChild(cell015);
                            }
                            else if (col == "vat")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell016 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell016.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell016.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.vat?.ToString());
                                newRow.AppendChild(cell016);
                            }
                            else if (col == "SubTotalPrice")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell017 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell017.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell017.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.SubTotalPrice?.ToString());
                                newRow.AppendChild(cell017);
                            }
                            else if (col == "TotalPrice")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell018 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell018.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell018.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.TotalPrice?.ToString());
                                newRow.AppendChild(cell018);
                            }
                            else if (col == "ExtraPremiumPrice")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell019 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell019.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell019.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ExtraPremiumPrice?.ToString());
                                newRow.AppendChild(cell019);
                            }
                            else if (col == "DiscountPercentageValue")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell020 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell020.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell020.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.DiscountPercentageValue?.ToString());
                                newRow.AppendChild(cell020);
                            }
                            else if (col == "SchemeDiscount")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell021 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell021.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell021.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.SchemeDiscount?.ToString());
                                newRow.AppendChild(cell021);
                            }
                            else if (col == "SchemeDiscountPercentage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell022 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell022.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell022.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.SchemeDiscountPercentage?.ToString());
                                newRow.AppendChild(cell022);
                            }
                            else if (col == "LoyaltyDiscountValue")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell023 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell023.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell023.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.LoyaltyDiscountValue?.ToString());
                                newRow.AppendChild(cell023);
                            }
                            else if (col == "LoyaltyDiscountPercentage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell024 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell024.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell024.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.LoyaltyDiscountPercentage?.ToString());
                                newRow.AppendChild(cell024);
                            }
                            else if (col == "MainDriverNin")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell025 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell025.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell025.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.MainDriverNin);
                                newRow.AppendChild(cell025);
                            }
                            else if (col == "MainDriverName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell026 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell026.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell026.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.MainDriverName);
                                newRow.AppendChild(cell026);
                            }
                            else if (col == "City")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell027 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell027.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell027.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.City);
                                newRow.AppendChild(cell027);
                            }
                            else if (col == "MainDriverOccupation")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell028 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell028.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell028.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.MainDriverOccupation);
                                newRow.AppendChild(cell028);
                            }
                            else if (col == "MainDriverOccupationCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell029 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell029.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell029.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.MainDriverOccupationCode);
                                newRow.AppendChild(cell029);
                            }
                            else if (col == "PromotionProgram")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell030 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell030.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell030.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PromotionProgram);
                                newRow.AppendChild(cell030);
                            }
                            else if (col == "InsuredID")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell031 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell031.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell031.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredID);
                                newRow.AppendChild(cell031);
                            }
                            else if (col == "InsuredNationality")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell032 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell032.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell032.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredNationality);
                                newRow.AppendChild(cell032);
                            }
                            else if (col == "InsuredBirthDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell033 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell033.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell033.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredBirthDate.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell033);
                            }
                            else if (col == "InsuredBirthDateH")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell034 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell034.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell034.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredBirthDateH);
                                newRow.AppendChild(cell034);
                            }
                            else if (col == "InsuredOccupation")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell035 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell035.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell035.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredOccupation);
                                newRow.AppendChild(cell035);
                            }
                            else if (col == "InsuredOccupationCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell036 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell036.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell036.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredOccupationCode);
                                newRow.AppendChild(cell036);
                            }
                            else if (col == "InsuredGender")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell037 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell037.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell037.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredGender?.ToString());
                                newRow.AppendChild(cell037);
                            }
                            else if (col == "InsuredEducationId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell038 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell038.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell038.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredEducationId.ToString());
                                newRow.AppendChild(cell038);
                            }
                            else if (col == "InsuredSocialStatus")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell039 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell039.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell039.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredSocialStatus?.ToString());
                                newRow.AppendChild(cell039);
                            }
                            else if (col == "InsuredChildrenBelow16Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell040 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell040.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell040.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredChildrenBelow16Years?.ToString());
                                newRow.AppendChild(cell040);
                            }
                            else if (col == "InsuredNationalAddress")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell041 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell041.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell041.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredNationalAddress);
                                newRow.AppendChild(cell041);
                            }
                            else if (col == "InsuredWorkCity")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell042 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell042.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell042.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredWorkCity);
                                newRow.AppendChild(cell042);
                            }
                            else if (col == "NajmNcdFreeYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell043 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell043.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell043.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.NajmNcdFreeYears?.ToString());
                                newRow.AppendChild(cell043);
                            }
                            else if (col == "EXcess")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell044 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell044.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell044.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.EXcess?.ToString());
                                newRow.AppendChild(cell044);
                            }
                            else if (col == "RepairType")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell045 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell045.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                if (policy.InsuranceTypeCode == 2&& policy.IsVehicleAgencyRepair.HasValue&& policy.IsVehicleAgencyRepair.Value)
                                {
                                    cell045.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Agency");
                                }
                               else if (policy.InsuranceTypeCode == 2 && policy.IsVehicleAgencyRepair.HasValue && !policy.IsVehicleAgencyRepair.Value)
                                {
                                    cell045.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("WorkShop");
                                }
                                else if (policy.InsuranceTypeCode == 2 && !policy.IsVehicleAgencyRepair.HasValue)
                                {
                                    cell045.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("WorkShop");
                                }
                                else
                                {
                                    cell045.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("WorkShop");
                                }
                                newRow.AppendChild(cell045);
                            }
                            else if (col == "MileageExpectedPerYearId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell046 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell046.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell046.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.MileageExpectedPerYearId?.ToString());
                                newRow.AppendChild(cell046);
                            }
                            else if (col == "CarPlateNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell047 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell047.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell047.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CarPlateNumber?.ToString());
                                newRow.AppendChild(cell047);
                            }
                            else if (col == "CustomCardNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell048 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell048.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                if (string.IsNullOrEmpty(policy.SequenceNumber))
                                {
                                    cell048.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CustomCardNumber);
                                }
                                else
                                {
                                    cell048.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(string.Empty);
                                }
                                newRow.AppendChild(cell048);
                            }
                            else if (col == "SequenceNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell049 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell049.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell049.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.SequenceNumber);
                                newRow.AppendChild(cell049);
                            }
                            else if (col == "VehicleMakeModel")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell050 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell050.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell050.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleMakeModel);
                                newRow.AppendChild(cell050);
                            }
                            else if (col == "ManufactureYear")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell051 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell051.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell051.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ManufactureYear?.ToString());
                                newRow.AppendChild(cell051);
                            }
                            else if (col == "SumInsured")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell052 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell052.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell052.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.SumInsured?.ToString());
                                newRow.AppendChild(cell052);
                            }
                            else if (col == "VehicleUseId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell053 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell053.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell053.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleUseId.ToString());
                                newRow.AppendChild(cell053);
                            }
                            else if (col == "TransmissionTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell054 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell054.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell054.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.TransmissionTypeId?.ToString());
                                newRow.AppendChild(cell054);
                            }
                            else if (col == "ParkingLocationId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell055 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell055.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell055.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ParkingLocationId?.ToString());
                                newRow.AppendChild(cell055);
                            }
                            else if (col == "VehicleModificationDetails")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell056 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell056.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell056.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleModificationDetails);
                                newRow.AppendChild(cell056);
                            }
                            else if (col == "VehicleBodyCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleBodyCode.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ArabicVehicleBody")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ArabicVehicleBody?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "EnglishVehicleBody")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.EnglishVehicleBody?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "AddditionalDriverOneName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell057 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell057.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell057.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneName);
                                newRow.AppendChild(cell057);
                            }
                            else if (col == "AddditionalDriverOneNin")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell058 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell058.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell058.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneNin);
                                newRow.AppendChild(cell058);
                            }
                            else if (col == "AddditionalDriverOneDateOfBirthG")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell059 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell059.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell059.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneDateOfBirthG?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell059);
                            }
                            else if (col == "AddditionalDriverOneDateOfBirthH")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell060 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell060.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell060.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneDateOfBirthH);
                                newRow.AppendChild(cell060);
                            }
                            else if (col == "AddditionalDriverOneGenderId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell061 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell061.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell061.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneGenderId?.ToString());
                                newRow.AppendChild(cell061);
                            }
                            else if (col == "AddditionalDriverOneEducationId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell062 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell062.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell062.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneEducationId?.ToString());
                                newRow.AppendChild(cell062);
                            }
                            else if (col == "AddditionalDriverOneSocialStatusId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell063 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell063.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell063.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneSocialStatusId?.ToString());
                                newRow.AppendChild(cell063);
                            }
                            else if (col == "AddditionalDriverOneChildrenBelow16Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell064 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell064.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell064.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneChildrenBelow16Years?.ToString());
                                newRow.AppendChild(cell064);
                            }
                            else if (col == "AddditionalDriverOneOccupation")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell066 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell066.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell066.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneOccupation);
                                newRow.AppendChild(cell066);
                            }
                            else if (col == "AddditionalDriverOneOccupationCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell067 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell067.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell067.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneOccupationCode);
                                newRow.AppendChild(cell067);
                            }
                            else if (col == "AdditionalDriverOneNOALast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell068 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell068.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell068.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneNumOfFaultAccidentInLast5Years?.ToString());
                                newRow.AppendChild(cell068);
                            }
                            else if (col == "AddditionalDriverOneNOCLast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell088 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell088.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell088.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneNOCLast5Years?.ToString());
                                newRow.AppendChild(cell088);
                            }
                            else if (col == "AddditionalDriverOneNCDFreeYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell069 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell069.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell069.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneNCDFreeYears?.ToString());
                                newRow.AppendChild(cell069);
                            }
                            else if (col == "AddditionalDriverOneWorkCity")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell070 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                if (string.IsNullOrEmpty(policy.AddditionalDriverOneWorkCity) && !string.IsNullOrEmpty(policy.AddditionalDriverOneNin))
                                {
                                    cell070.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell070.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredWorkCity);
                                }
                                else
                                {
                                    cell070.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell070.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneWorkCity);
                                }
                                newRow.AppendChild(cell070);
                            }
                            else if (col == "AddditionalDriverOneNationalityCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell071 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell071.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell071.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneNationalityCode?.ToString());
                                newRow.AppendChild(cell071);
                            }
                            else if (col == "AddditionalDriverOneMedicalCondition")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell072 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell072.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell072.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneMedicalCondition);
                                newRow.AppendChild(cell072);
                            }
                            else if (col == "AddditionalDriverOneViolation")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell073 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell073.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell073.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneRoadConvictions);
                                newRow.AppendChild(cell073);
                            }
                            else if (col == "AddditionalDriverOneSaudiLicenseHeldYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell074 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell074.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell074.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneSaudiLicenseHeldYears?.ToString());
                                newRow.AppendChild(cell074);
                            }
                            else if (col == "AddditionalDriverOneAddress")
                            {
                                // As per rawabi in mail (RE: SAMA Report Issues  3-2-2022)
                                //DocumentFormat.OpenXml.Spreadsheet.Cell cell075 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //cell075.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //cell075.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredNationalAddress);
                                //newRow.AppendChild(cell075);
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell075 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                if (string.IsNullOrEmpty(policy.AddditionalDriverOneAddress) && !string.IsNullOrEmpty(policy.AddditionalDriverOneNin))
                                {
                                    cell075.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell075.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredNationalAddress);
                                    newRow.AppendChild(cell075);
                                }
                                else
                                {
                                    cell075.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell075.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneAddress);
                                    newRow.AppendChild(cell075);
                                }
                            }
                            else if (col == "AddditionalDriverOneDriverLicense")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell076 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell076.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell076.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneDriverLicense);
                                newRow.AppendChild(cell076);
                            }
                            else if (col == "AddditionalDriverOneDriverExtraLicense")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell077 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell077.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell077.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverOneDriverExtraLicense);
                                newRow.AppendChild(cell077);
                            }
                            else if (col == "AddditionalDriverTwoName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell078 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell078.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell078.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoName);
                                newRow.AppendChild(cell078);
                            }
                            else if (col == "AddditionalDriverTwoNin")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell079 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell079.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell079.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoNin);
                                newRow.AppendChild(cell079);
                            }
                            else if (col == "AddditionalDriverTwoDateOfBirthG")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell080 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell080.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell080.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoDateOfBirthG?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell080);
                            }
                            else if (col == "AddditionalDriverTwoDateOfBirthH")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell081 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell081.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell081.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoDateOfBirthH);
                                newRow.AppendChild(cell081);
                            }
                            else if (col == "AddditionalDriverTwoGenderId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell082 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell082.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell082.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoGenderId?.ToString());
                                newRow.AppendChild(cell082);
                            }
                            else if (col == "AddditionalDriverTwoEducationId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell083 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell083.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell083.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoEducationId?.ToString());
                                newRow.AppendChild(cell083);
                            }
                            else if (col == "AddditionalDriverTwoSocialStatusId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell084 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell084.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell084.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoSocialStatusId?.ToString());
                                newRow.AppendChild(cell084);
                            }
                            else if (col == "AddditionalDriverTwoChildrenBelow16Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell085 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell085.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell085.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoChildrenBelow16Years?.ToString());
                                newRow.AppendChild(cell085);
                            }
                            else if (col == "AddditionalDriverTwoOccupation")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell086 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell086.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell086.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoOccupation);
                                newRow.AppendChild(cell086);
                            }
                            else if (col == "AddditionalDriverTwoOccupationCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell087 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell087.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell087.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoOccupationCode);
                                newRow.AppendChild(cell087);
                            }
                            else if (col == "AdditionalDriverTwoNOALast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell088 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell088.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell088.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoNumOfFaultAccidentInLast5Years?.ToString());
                                newRow.AppendChild(cell088);
                            }
                            else if (col == "AddditionalDriverTwoNOCLast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell488 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell488.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell488.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoNOCLast5Years?.ToString());
                                newRow.AppendChild(cell488);
                            }
                            else if (col == "AddditionalDriverTwoNCDFreeYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell089 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell089.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell089.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoNCDFreeYears?.ToString());
                                newRow.AppendChild(cell089);
                            }
                            else if (col == "AddditionalDriverTwoWorkCity")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell090 = new DocumentFormat.OpenXml.Spreadsheet.Cell();

                                if (string.IsNullOrEmpty(policy.AddditionalDriverTwoWorkCity) && !string.IsNullOrEmpty(policy.AddditionalDriverTwoNin))
                                {
                                    cell090.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell090.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredWorkCity);
                                }
                                else
                                {
                                    cell090.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell090.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoWorkCity);
                                }
                                newRow.AppendChild(cell090);
                            }
                            else if (col == "AddditionalDriverTwoNationalityCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell091 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell091.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell091.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoNationalityCode?.ToString());
                                newRow.AppendChild(cell091);
                            }
                            else if (col == "AddditionalDriverTwoMedicalCondition")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell092 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell092.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell092.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoMedicalCondition);
                                newRow.AppendChild(cell092);
                            }
                            else if (col == "AddditionalDriverTwoViolation")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell093 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell093.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell093.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoRoadConvictions);
                                newRow.AppendChild(cell093);
                            }
                            else if (col == "AddditionalDriverTwoSaudiLicenseHeldYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell094 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell094.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell094.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoSaudiLicenseHeldYears?.ToString());
                                newRow.AppendChild(cell094);
                            }
                            else if (col == "AddditionalDriverTwoAddress")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell095 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                if (string.IsNullOrEmpty(policy.AddditionalDriverTwoAddress) && !string.IsNullOrEmpty(policy.AddditionalDriverTwoNin))
                                {
                                    cell095.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell095.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredNationalAddress);
                                    newRow.AppendChild(cell095);
                                }
                                else
                                {
                                    cell095.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell095.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoAddress);
                                    newRow.AppendChild(cell095);
                                }
                            }
                            else if (col == "AddditionalDriverTwoDriverLicense")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell096 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell096.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell096.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoDriverLicense);
                                newRow.AppendChild(cell096);
                            }
                            else if (col == "AddditionalDriverTwoDriverExtraLicense")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell097 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell097.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell097.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.AddditionalDriverTwoDriverExtraLicense);
                                newRow.AppendChild(cell097);
                            }
                            else if (col == "InsuranceType")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0118 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0118.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0118.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuranceType);
                                newRow.AppendChild(cell0118);
                            }
                            else if (col == "DeductibleValue")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0119 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0119.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0119.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.DeductibleValue?.ToString());
                                newRow.AppendChild(cell0119);
                            }
                            else if (col == "MileageExpectedAnnual")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0120 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0120.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0120.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.MileageExpectedAnnual);
                                newRow.AppendChild(cell0120);
                            }
                            else if (col == "Covarage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0121 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0121.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0121.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Covarage);
                                newRow.AppendChild(cell0121);
                            }
                            else if (col == "DriverLicenseType")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.DriverLicenseType);
                                newRow.AppendChild(cell0122);
                            }
                            else if (col == "SaudiLicenseHeldYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0123 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0123.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0123.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.SaudiLicenseHeldYears?.ToString());
                                newRow.AppendChild(cell0123);
                            }
                            else if (col == "DriverExtraLicenses")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0124 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0124.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0124.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.DriverExtraLicenses);
                                newRow.AppendChild(cell0124);
                            }
                            else if (col == "NOALast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0125 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0125.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0125.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.NOALast5Years?.ToString());
                                newRow.AppendChild(cell0125);
                            }
                            else if (col == "NOCLast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0126 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0126.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0126.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.NOCLast5Years?.ToString());
                                newRow.AppendChild(cell0126);
                            }
                            else if (col == "DriverViolations")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0127 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0127.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0127.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.DriverViolations);
                                newRow.AppendChild(cell0127);
                            }
                            else if (col == "DriverMedicalCondition")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0128 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0128.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0128.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.DriverMedicalCondition);
                                newRow.AppendChild(cell0128);
                            }
                            else if (col == "VehicleLoad")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0129 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0129.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0129.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleLoad.ToString());
                                newRow.AppendChild(cell0129);
                            }
                            else if (col == "EngineSize")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0130 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0130.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0130.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.EngineSize);
                                newRow.AppendChild(cell0130);
                            }
                            else if (col == "ChassisNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell0131 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell0131.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell0131.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ChassisNumber);
                                newRow.AppendChild(cell0131);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }


        #endregion

        #region Export Occupations Excel

        public byte[] ExportOccupations(List<Occupation> occupations, string name)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SMS-Logs" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();

                    columns.Add("Code");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Code");
                    headerRow.AppendChild(cell1);

                    columns.Add("Name Ar");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Name Ar");
                    headerRow.AppendChild(cell2);

                    columns.Add("Name En");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Name En");
                    headerRow.AppendChild(cell3);

                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (var occupation in occupations)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "Code")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(occupation.Code);
                                newRow.AppendChild(cell11);
                            }
                            else if (col == "Name Ar")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(occupation.NameAr);
                                newRow.AppendChild(cell22);
                            }
                            else if (col == "Name En")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(occupation.NameEn);
                                newRow.AppendChild(cell33);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion

        #region Export Tickets

        //public byte[] ExportAllTicket(List<TicketModel> tickets, string name)
        //{
        //    DateTime dt = DateTime.Now.AddDays(-1);
        //    string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SMS-Logs" + dt.ToString("dd-MM-yyyy"));
        //    SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
        //    using (var workbook = SpreadsheetDocument.Create(SPREADSHEET_NAME, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
        //    {
        //        var workbookPart = workbook.AddWorkbookPart();
        //        {
        //            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        //            var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
        //            sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);
        //            workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
        //            workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();
        //            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
        //            string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        //            uint sheetId = 1;
        //            if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
        //                sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

        //            DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
        //            sheets.Append(sheet);

        //            DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        //            List<String> columns = new List<string>();

        //            columns.Add("Id");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Ticket Number");
        //            headerRow.AppendChild(cell1);

        //            columns.Add("RequestedDate");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Requested Date");
        //            headerRow.AppendChild(cell2);

        //            columns.Add("TicketType");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RequestType");
        //            headerRow.AppendChild(cell3);

        //            columns.Add("Statue");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Statue");
        //            headerRow.AppendChild(cell4);

        //            columns.Add("ClosedDate");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Closed Date");
        //            headerRow.AppendChild(cell5);

        //            columns.Add("TheUser");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("The User");
        //            headerRow.AppendChild(cell6);

        //            columns.Add("AdminReply");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AdminReply");
        //            headerRow.AppendChild(cell7);

        //            columns.Add("RepliedBy");
        //            DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //            cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //            cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RepliedBy");
        //            headerRow.AppendChild(cell8);

        //            sheetData.AppendChild(headerRow);
        //            workbook.WorkbookPart.Workbook.Save();

        //            foreach (var ticket in tickets)
        //            {
        //                DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
        //                foreach (string col in columns)
        //                {
        //                    if (col == "Id")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.Id.ToString());
        //                        newRow.AppendChild(cell11);
        //                    }
        //                    else if (col == "RequestedDate")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.OpenedDate.ToString());
        //                        newRow.AppendChild(cell22);
        //                    }
        //                    else if (col == "TicketType")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.TicketTypeNameEn);
        //                        newRow.AppendChild(cell33);
        //                    }
        //                    else if (col == "Statue")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.StatusNameEn);
        //                        newRow.AppendChild(cell44);
        //                    }
        //                    else if (col == "ClosedDate")
        //                    {
        //                        if (ticket.StatusId == 3)
        //                        {
        //                            DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                            cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                            cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.ClosedDate.ToString());// if statues closed
        //                            newRow.AppendChild(cell55);
        //                        }
        //                        else
        //                        {
        //                            DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                            cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                            cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("");// if statues closed
        //                            newRow.AppendChild(cell55);
        //                        }
        //                    }
        //                    else if (col == "TheUser")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.UserEmail);
        //                        newRow.AppendChild(cell66);
        //                    }
        //                    else if (col == "AdminReply")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell67 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cell67.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cell67.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.AdminReply);
        //                        newRow.AppendChild(cell67);
        //                    } 
        //                    else if (col == "RepliedBy")
        //                    {
        //                        DocumentFormat.OpenXml.Spreadsheet.Cell cell68 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
        //                        cell68.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
        //                        cell68.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(ticket.ClosedBy);
        //                        newRow.AppendChild(cell68);
        //                    }
        //                }

        //                sheetData.AppendChild(newRow);
        //                workbook.WorkbookPart.Workbook.Save();
        //            }

        //        }

        //        workbook.Close();
        //    }

        //    return GetFileAsByte(SPREADSHEET_NAME);
        //}

        public byte[] ExportAllTicket(List<ExcelTiketModel> tickets, string name)        {            DateTime dt = DateTime.Now;
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PoliciesDetails" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PoliciesDetails" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();


                    columns.Add("TicketNumber");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell0 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell0.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell0.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Ticket Number");
                    headerRow.AppendChild(cell0);


                    columns.Add("CreationDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Creation Date");
                    headerRow.AppendChild(cell1);

                    columns.Add("TicketType");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Ticket Type");
                    headerRow.AppendChild(cell2);



                    columns.Add("Channel");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                    headerRow.AppendChild(cell3);



                    columns.Add("UserEmail");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("User Email");
                    headerRow.AppendChild(cell4);


                    columns.Add("Reporter");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reporter");
                    headerRow.AppendChild(cell5);


                    columns.Add("Statue");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Statue");
                    headerRow.AppendChild(cell6);


                    //first history
                    columns.Add("UpdateDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Update Date");
                    headerRow.AppendChild(cell7);


                    columns.Add("UpdateBy");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Update By");
                    headerRow.AppendChild(cell8);


                    columns.Add("FirstAdminReply");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Admin Reply");
                    headerRow.AppendChild(cell9);

                    //closed history
                    columns.Add("ClosedDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Closed Date");
                    headerRow.AppendChild(cell10);


                    columns.Add("ClosedBy");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Closed By");
                    headerRow.AppendChild(cell11);


                    columns.Add("ClosedAdminReply");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Admin Reply");
                    headerRow.AppendChild(cell12);


                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (var item in tickets)
                    {
                        int lastItemIndex = item.UserTicketHistory.Count();
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "TicketNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell00 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell00.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell00.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Id?.ToString());
                                newRow.AppendChild(cell00);
                            }
                            else if (col == "UserEmail")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.UserEmail);
                                newRow.AppendChild(cell44);
                            }
                            else if (col == "TicketType")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.TicketTypeNameEn);
                                newRow.AppendChild(cell22);
                            }
                            else if (col == "Channel")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Channel);
                                newRow.AppendChild(cell33);
                            }
                            else if (col == "Reporter")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Reporter);
                                newRow.AppendChild(cell55);
                            }
                            else if (col == "CreationDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.OpenedDate?.ToString());
                                newRow.AppendChild(cell111);
                            }
                            else if (col == "UpdateDate" && item.UserTicketHistory != null && item.UserTicketHistory.Count > 0)
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.UserTicketHistory[0]?.CreatedDate?.ToString());
                                newRow.AppendChild(cell77);
                            }
                            else if (col == "UpdateBy" && item.UserTicketHistory != null && item.UserTicketHistory.Count > 0)
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.UserTicketHistory[0]?.RepliedBy);
                                newRow.AppendChild(cell88);
                            }
                            else if (col == "FirstAdminReply" && item.UserTicketHistory != null && item.UserTicketHistory.Count > 0)
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.UserTicketHistory[0]?.AdminReply);
                                newRow.AppendChild(cell99);
                            }
                            else if (col == "Statue" && item.UserTicketHistory != null && item.UserTicketHistory.Count > 0)
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.UserTicketHistory[0]?.StatusNameEn);
                                newRow.AppendChild(cell66);
                            }

                            // closed history
                            if (item.UserTicketHistory.Any(a => a.TicketStatusId == 3))
                            {
                                var closedhitory = item.UserTicketHistory.Where(a => a.TicketStatusId == 3).FirstOrDefault();

                                if (col == "ClosedDate" && item.UserTicketHistory != null && item.UserTicketHistory.Count > 0)
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1010 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1010.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1010.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(closedhitory?.CreatedDate?.ToString());
                                    newRow.AppendChild(cell1010);
                                }
                                else if (col == "ClosedBy" && item.UserTicketHistory != null && item.UserTicketHistory.Count > 0)
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(closedhitory?.RepliedBy);
                                    newRow.AppendChild(cell1111);
                                }
                                else if (col == "ClosedAdminReply" && item.UserTicketHistory != null && item.UserTicketHistory.Count > 0)
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1212 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1212.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1212.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(closedhitory?.AdminReply);
                                    newRow.AppendChild(cell1212);
                                }
                            }

                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);        }
        #endregion

        #region Export Autoleasing Quotation Report
        public byte[] GenerateExcelAutoleasingQuotationReport(List<AutoleasingQuotationReportInfoModel> quotationReport)
        {
            DateTime dt = DateTime.Now;
            string fileName = "Autoleasing Quotation Report" + dt.ToString("dd-MM-yyyy");
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
            Utilities.DeleteExcelSheetFiles();
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Autoleasing Quotation Report" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell;
                    {
                        columns.Add("Quotation Number(رقم عرض السعر)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Quotation Number");
                        headerRow.AppendChild(cell);

                        columns.Add("Insurance Company");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurance company");
                        headerRow.AppendChild(cell);

                        columns.Add("Status(الحالة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Status(الحالة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Quotation Issue date(تاريخ اصدار عرض السعر)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Quotation Issue date(تاريخ اصدار عرض السعر)");
                        headerRow.AppendChild(cell);

                        columns.Add("Quotation expiry date(تاريخ انتهاء عرض السعر)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Quotation expiry date(تاريخ انتهاء عرض السعر)");
                        headerRow.AppendChild(cell);

                        columns.Add("Issued Quotation User(مصدر عرض السعر )");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Issued Quotation User(مصدر عرض السعر )");
                        headerRow.AppendChild(cell);

                        columns.Add("Product Price");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Product Price");
                        headerRow.AppendChild(cell);

                        columns.Add("NCD Level(مستوى خصم عدم وجود مطالبات )");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NCD Level(مستوى خصم عدم وجود مطالبات )");
                        headerRow.AppendChild(cell);

                        columns.Add("NCD Percentage(خصم عدم وجود مطالبات)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NCD Percentage(خصم عدم وجود مطالبات)");
                        headerRow.AppendChild(cell);

                        columns.Add("No Claims Discount (NCD)(خصم عدم وجود مطالبات)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("No Claims Discount (NCD)(خصم عدم وجود مطالبات)");
                        headerRow.AppendChild(cell);

                        columns.Add("Basic Premium");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Basic Premium");
                        headerRow.AppendChild(cell);

                        columns.Add("Basic Premium + VAT");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Basic Premium + VAT");
                        headerRow.AppendChild(cell);

                        columns.Add("Value Added Tax (VAT)(قيمة الضريبة المضافة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Value Added Tax (VAT)(قيمة الضريبة المضافة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Lessee insurance amount((حساب تأمين المستأجر ( المبلغ )");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Lessee insurance amount((حساب تأمين المستأجر ( المبلغ )");
                        headerRow.AppendChild(cell);

                        columns.Add("Customer Name(اسم العميل)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Customer Name(اسم العميل)");
                        headerRow.AppendChild(cell);

                        columns.Add("National ID(رقم الهوية)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("National ID(رقم الهوية)");
                        headerRow.AppendChild(cell);

                        columns.Add("DOB(تاريخ الميلاد)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DOB(تاريخ الميلاد)");
                        headerRow.AppendChild(cell);

                        columns.Add("Phone Number(رقم الهاتف)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Phone Number(رقم الهاتف)");
                        headerRow.AppendChild(cell);

                        columns.Add("Make Name(النوع)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Make Name(النوع)");
                        headerRow.AppendChild(cell);

                        columns.Add("Model Name(الطراز)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Model Name(الطراز)");
                        headerRow.AppendChild(cell);

                        columns.Add("Body type(نوع الهيكل)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Body type(نوع الهيكل)");
                        headerRow.AppendChild(cell);

                        columns.Add("Vehicle Color(لون المركبة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Vehicle Color(لون المركبة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Sum insured(القيمة التأمينية)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Sum insured(القيمة التأمينية)");
                        headerRow.AppendChild(cell);

                        columns.Add("USAGE(المستخدم)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("USAGE(المستخدم)");
                        headerRow.AppendChild(cell);

                        columns.Add("Custom No(رقم البطاقة الجمركية)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Custom No(رقم البطاقة الجمركية)");
                        headerRow.AppendChild(cell);

                        columns.Add("Chassis No(رقم الهيكل)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Chassis No(رقم الهيكل)");
                        headerRow.AppendChild(cell);

                        columns.Add("Sequence No(الرقم التسلسلي)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Sequence No(الرقم التسلسلي)");
                        headerRow.AppendChild(cell);

                        columns.Add("Plate No(رقم اللوحة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Plate No(رقم اللوحة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Manufacturing year(سنة الصنع)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Manufacturing year(سنة الصنع)");
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (var item in quotationReport)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        foreach (String col in columns)
                        {
                            
                            if (col == "Quotation Number(رقم عرض السعر)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.QuotationNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Insurance Company")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuranceCompanyName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Status(الحالة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Status);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Quotation Issue date(تاريخ اصدار عرض السعر)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.QuotationIssueDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Quotation expiry date(تاريخ انتهاء عرض السعر)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.QuotationExpiryDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Issued Quotation User(مصدر عرض السعر )")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.IssuedQuotationUser);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Product Price")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ProductPrice?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NCD Level(مستوى خصم عدم وجود مطالبات )")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NCDLevel?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NCD Percentage(خصم عدم وجود مطالبات)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NCDPercentage?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "No Claims Discount (NCD)(خصم عدم وجود مطالبات)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NoClaimsDiscountNCD?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Basic Premium")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.BasicPrimium?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Basic Premium + VAT")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.BasicPrimiumWithVAT?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Value Added Tax (VAT)(قيمة الضريبة المضافة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VAT?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Lessee insurance amount((حساب تأمين المستأجر ( المبلغ )")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.LesseeInsuranceAmount?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Customer Name(اسم العميل)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CustomerName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "National ID(رقم الهوية)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NationalID);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DOB(تاريخ الميلاد)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DateOfBirth?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Phone Number(رقم الهاتف)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PhoneNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Make Name(النوع)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleMakerName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Model Name(الطراز)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleModelName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Body type(نوع الهيكل)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleBodyType);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Vehicle Color(لون المركبة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleColor);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Sum insured(القيمة التأمينية)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.SumInsured?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "USAGE(المستخدم)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Usage);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Custom No(رقم البطاقة الجمركية)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CustomCardNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Chassis No(رقم الهيكل)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ChassisNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Sequence No(الرقم التسلسلي)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.SequenceNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Plate No(رقم اللوحة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PlateNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Manufacturing year(سنة الصنع)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ModelYear.ToString());
                                newRow.AppendChild(cell);
                            }
                        }
                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }
                    workbook.Close();
                }
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        public byte[] GenerateExcelCorporatePolicies(List<CorporatePolicyModel> corporatePolicies)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CorporatePolicies" + dt.ToString("dd-MM-yyyy"));
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "CorporatePolicies" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();

                    {
                        columns.Add("PolicyNo");
                        headerRow.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Cell() {
                            DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String,
                            CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo")
                        });

                        columns.Add("InsuranceCompany");
                        headerRow.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Cell()
                        {
                            DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String,
                            CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuranceCompany")
                        });

                        columns.Add("InsuranceType");
                        headerRow.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Cell()
                        {
                            DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String,
                            CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuranceType")
                        });

                        columns.Add("PolicyIssueDate");
                        headerRow.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Cell()
                        {
                            DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String,
                            CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyIssueDate")
                        });

                        columns.Add("PolicyEffectiveDate");
                        headerRow.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Cell()
                        {
                            DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String,
                            CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyEffectiveDate")
                        });

                        columns.Add("PolicyExpiryDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyExpiryDate");
                        headerRow.AppendChild(cell5);

                        columns.Add("IBAN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IBAN");
                        headerRow.AppendChild(cell7);

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

                        columns.Add("ReferenceId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                        headerRow.AppendChild(cell11);

                        columns.Add("QuotationPrice");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("QuotationPrice");
                        headerRow.AppendChild(cell12);

                        columns.Add("PolicyPrice");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyPrice");
                        headerRow.AppendChild(cell13);

                        columns.Add("PaidAmount");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaidAmount");
                        headerRow.AppendChild(cell14);

                        columns.Add("BenfitsPrice");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("BenfitsPrice");
                        headerRow.AppendChild(cell15);

                        columns.Add("Phone");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Phone");
                        headerRow.AppendChild(cell16);

                        columns.Add("Email");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Email");
                        headerRow.AppendChild(cell17);

                        columns.Add("Insured ID");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured ID");
                        headerRow.AppendChild(cell18);

                        columns.Add("UserAccount");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserAccount");
                        headerRow.AppendChild(cell19);

                        columns.Add("PaymentMethod");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaymentMethod");
                        headerRow.AppendChild(cell20);

                    }

                    sheetData.AppendChild(headerRow);

                    workbook.WorkbookPart.Workbook.Save();

                    foreach (CorporatePolicyModel policy in corporatePolicies)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();


                        foreach (string col in columns)
                        {
                            if (col == "PolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyNo); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuranceCompany")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuranceCompanyName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuranceType")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuranceType);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PolicyIssueDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyIssueDate?.ToString(new CultureInfo("en-US"))); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PolicyEffectiveDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyEffectiveDate?.ToString(new CultureInfo("en-US"))); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PolicyExpiryDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyExpiryDate?.ToString(new CultureInfo("en-US"))); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "IBAN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.IBAN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy?.InsuredFullNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "TotalPremium")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.TotalPremium.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ReferenceId")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CheckOutDetailsId); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "QuotationPrice")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.QuotationPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "PolicyPrice")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PaidAmount")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PaidAmount.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "BenfitsPrice")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.BenfitsPrice.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Phone")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Phone); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Email")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CheckOutDetailsEmail); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Insured ID")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.DriverNIN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "UserAccount")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.UserAccountEmail); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PaymentMethod")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PaymentMethod); //
                                newRow.AppendChild(cell);
                            }

                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }

                }

                workbook.Close();
            }


            return GetFileAsByte(SPREADSHEET_NAME);

        }
        #endregion

        #region Export Autoleasing Policy Report
        public byte[] GenerateExcelAutoleasingPolicyReport(List<AutoleasingPolicyReportInfoModel> policyReport)
        {
            DateTime dt = DateTime.Now;
            string fileName = "Autoleasing Policy Report" + dt.ToString("dd-MM-yyyy");
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
            Utilities.DeleteExcelSheetFiles();
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Autoleasing Quotation Report" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell;
                    {
                        columns.Add("Policy Number(رقم البوليصه)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Number(رقم البوليصه)");
                        headerRow.AppendChild(cell);

                        columns.Add("Insurance Company");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurance company");
                        headerRow.AppendChild(cell);

                        columns.Add("Leasing Contract Number(رقم عقد التأجير)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Leasing Contract Number(رقم عقد التأجير)");
                        headerRow.AppendChild(cell);

                        columns.Add("Quotation Number(رقم عرض السعر)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Quotation Number(رقم عرض السعر)");
                        headerRow.AppendChild(cell);

                        columns.Add("Issued Policy User(مصدر البوليصه السعر)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Issued Policy User(مصدر البوليصه السعر)");
                        headerRow.AppendChild(cell);

                        columns.Add("Policy issued on(تم نشر السياسة في)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy issued on(تم نشر السياسة في)");
                        headerRow.AppendChild(cell);

                        columns.Add("Policy Start Date(تاريخ بداية الوثيقة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Start Date(تاريخ بداية الوثيقة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Policy End Date(تاريخ نهاية الوثيقة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy End Date(تاريخ نهاية الوثيقة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Product Price");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Product Price");
                        headerRow.AppendChild(cell);

                        columns.Add("NCD Level(مستوى خصم عدم وجود مطالبات )");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NCD Level(مستوى خصم عدم وجود مطالبات )");
                        headerRow.AppendChild(cell);

                        columns.Add("NCD Percentage(خصم عدم وجود مطالبات)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NCD Percentage(خصم عدم وجود مطالبات)");
                        headerRow.AppendChild(cell);

                        columns.Add("No Claims Discount (NCD)(خصم عدم وجود مطالبات)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("No Claims Discount (NCD)(خصم عدم وجود مطالبات)");
                        headerRow.AppendChild(cell);

                        columns.Add("Basic Premium");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Basic Premium");
                        headerRow.AppendChild(cell);

                        columns.Add("Basic Premium + VAT");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Basic Premium + VAT");
                        headerRow.AppendChild(cell);

                        columns.Add("Value Added Tax (VAT)(قيمة الضريبة المضافة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Value Added Tax (VAT)(قيمة الضريبة المضافة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Lessee insurance amount((حساب تأمين المستأجر ( المبلغ )");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Lessee insurance amount((حساب تأمين المستأجر ( المبلغ )");
                        headerRow.AppendChild(cell);

                        columns.Add("NAJMStatus(حالة الرفع إلى شركة نجم)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NAJMStatus(حالة الرفع إلى شركة نجم)");
                        headerRow.AppendChild(cell);

                        columns.Add("Customer Name(اسم العميل)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Customer Name(اسم العميل)");
                        headerRow.AppendChild(cell);

                        columns.Add("National ID(رقم الهوية)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("National ID(رقم الهوية)");
                        headerRow.AppendChild(cell);

                        columns.Add("DOB(تاريخ الميلاد)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DOB(تاريخ الميلاد)");
                        headerRow.AppendChild(cell);

                        columns.Add("Phone Number(رقم الهاتف)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Phone Number(رقم الهاتف)");
                        headerRow.AppendChild(cell);

                        columns.Add("Make Name(النوع)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Make Name(النوع)");
                        headerRow.AppendChild(cell);

                        columns.Add("Model Name(الطراز)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Model Name(الطراز)");
                        headerRow.AppendChild(cell);

                        columns.Add("Body type(نوع الهيكل)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Body type(نوع الهيكل)");
                        headerRow.AppendChild(cell);

                        columns.Add("Vehicle Color(لون المركبة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Vehicle Color(لون المركبة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Sum insured(القيمة التأمينية)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Sum insured(القيمة التأمينية)");
                        headerRow.AppendChild(cell);

                        columns.Add("USAGE(المستخدم)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("USAGE(المستخدم)");
                        headerRow.AppendChild(cell);

                        columns.Add("Custom No(رقم البطاقة الجمركية)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Custom No(رقم البطاقة الجمركية)");
                        headerRow.AppendChild(cell);

                        columns.Add("Chassis No(رقم الهيكل)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Chassis No(رقم الهيكل)");
                        headerRow.AppendChild(cell);

                        columns.Add("Sequence No(الرقم التسلسلي)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Sequence No(الرقم التسلسلي)");
                        headerRow.AppendChild(cell);

                        columns.Add("Plate No(رقم اللوحة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Plate No(رقم اللوحة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Manufacturing year(سنة الصنع)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Manufacturing year(سنة الصنع)");
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (var item in policyReport)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        foreach (String col in columns)
                        {

                            if (col == "Policy Number(رقم البوليصه)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Insurance Company")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuranceCompanyName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Leasing Contract Number(رقم عقد التأجير)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.LeasingContractNumber?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Quotation Number(رقم عرض السعر)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.QuotationNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Issued Policy User(مصدر البوليصه السعر)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.IssuedPolicyUser);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy issued on(تم نشر السياسة في)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyIssueDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy Start Date(تاريخ بداية الوثيقة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyStartDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy End Date(تاريخ نهاية الوثيقة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyEndDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Product Price")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ProductPrice?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NCD Level(مستوى خصم عدم وجود مطالبات )")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NCDLevel?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NCD Percentage(خصم عدم وجود مطالبات)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NCDPercentage?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "No Claims Discount (NCD)(خصم عدم وجود مطالبات)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NoClaimsDiscountNCD?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Basic Premium")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.BasicPrimium?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Basic Premium + VAT")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.BasicPrimiumWithVAT?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Value Added Tax (VAT)(قيمة الضريبة المضافة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VAT?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Lessee insurance amount((حساب تأمين المستأجر ( المبلغ )")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.LesseeInsuranceAmount?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NAJMStatus(حالة الرفع إلى شركة نجم)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NajmStatus);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Customer Name(اسم العميل)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CustomerName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "National ID(رقم الهوية)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NationalID);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DOB(تاريخ الميلاد)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DateOfBirth?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Phone Number(رقم الهاتف)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PhoneNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Make Name(النوع)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleMakerName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Model Name(الطراز)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleModelName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Body type(نوع الهيكل)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleBodyType);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Vehicle Color(لون المركبة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleColor);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Sum insured(القيمة التأمينية)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.SumInsured?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "USAGE(المستخدم)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Usage);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Custom No(رقم البطاقة الجمركية)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CustomCardNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Chassis No(رقم الهيكل)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ChassisNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Sequence No(الرقم التسلسلي)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.SequenceNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Plate No(رقم اللوحة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PlateNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Manufacturing year(سنة الصنع)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ModelYear.ToString());
                                newRow.AppendChild(cell);
                            }
                        }
                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }
                    workbook.Close();
                }
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }
        #endregion

        #region Export Autoleasing Wallet Report
        public byte[] GenerateExcelGetAutoleasingWalletReport(List<AutoleasingWalletReportModel> walletReport)
        {
            DateTime dt = DateTime.Now;
            string fileName = "Autoleasing Wallet Report" + dt.ToString("dd-MM-yyyy");
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
            Utilities.DeleteExcelSheetFiles();
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Autoleasing Quotation Report" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell;
                    {
                        columns.Add("ReferenceId");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                        headerRow.AppendChild(cell);

                        columns.Add("Policy Number(رقم البوليصه)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Number(رقم البوليصه)");
                        headerRow.AppendChild(cell);

                        columns.Add("Insurance Company");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurance company");
                        headerRow.AppendChild(cell);

                        columns.Add("Issued Policy User(مصدر البوليصه السعر)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Issued Policy User(مصدر البوليصه السعر)");
                        headerRow.AppendChild(cell);

                        columns.Add("CreatedDate");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cell);

                        columns.Add("Method");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Method");
                        headerRow.AppendChild(cell);

                        columns.Add("Policy issued on(تم نشر السياسة في)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy issued on(تم نشر السياسة في)");
                        headerRow.AppendChild(cell);

                        columns.Add("Policy Start Date(تاريخ بداية الوثيقة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Start Date(تاريخ بداية الوثيقة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Policy End Date(تاريخ نهاية الوثيقة)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy End Date(تاريخ نهاية الوثيقة)");
                        headerRow.AppendChild(cell);

                        columns.Add("Customer Name(اسم العميل)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Customer Name(اسم العميل)");
                        headerRow.AppendChild(cell);

                        columns.Add("National ID(رقم الهوية)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("National ID(رقم الهوية)");
                        headerRow.AppendChild(cell);

                        columns.Add("Phone Number(رقم الهاتف)");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Phone Number(رقم الهاتف)");
                        headerRow.AppendChild(cell);

                        columns.Add("Amount");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Amount");
                        headerRow.AppendChild(cell);

                        columns.Add("RemainingBalance");
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RemainingBalance");
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (var item in walletReport)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        foreach (String col in columns)
                        {
                            if (col == "ReferenceId")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ReferenceId);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy Number(رقم البوليصه)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Insurance Company")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuranceCompanyName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Issued Policy User(مصدر البوليصه السعر)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.UserAccount);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CreatedDate")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CreatedDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Method")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Method);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy issued on(تم نشر السياسة في)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyIssueDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy Start Date(تاريخ بداية الوثيقة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyEffectiveDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")));
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Policy End Date(تاريخ نهاية الوثيقة)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyExpiryDate?.ToString("dd-MM-yyyy", new CultureInfo("en-US")));
                                newRow.AppendChild(cell);
                            }

                            else if (col == "Customer Name(اسم العميل)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuredName);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "National ID(رقم الهوية)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuredID);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Phone Number(رقم الهاتف)")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PhoneNumber);
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Amount")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Amount?.ToString());
                                newRow.AppendChild(cell);
                            }
                            else if (col == "RemainingBalance")
                            {
                                cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.RemainingBalance.ToString());
                                newRow.AppendChild(cell);
                            }
                        }
                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }
                    workbook.Close();
                }
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion

        #region Sama Statistics Report
        public byte[] GenerateSamaStatisticsReportExcel(SamaStatisticsCountReport report)
        {
            string SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SamaStatistics" + DateTime.Now.AddDays(-1.0).ToString("dd-MM-yyyy")) + ".xlsx";
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(SPREADSHEET_NAME, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                spreadsheetDocument.AddWorkbookPart();
                WorksheetPart worksheetPart1 = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData1 = new SheetData();
                worksheetPart1.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData1);
                spreadsheetDocument.WorkbookPart.Workbook = new Workbook();
                spreadsheetDocument.WorkbookPart.Workbook.Sheets = new Sheets();
                Sheets firstChild1 = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart((OpenXmlPart)worksheetPart1);
                uint num1 = 1;
                if (firstChild1.Elements<Sheet>().Count<Sheet>() > 0)
                    num1 = firstChild1.Elements<Sheet>().Select<Sheet, uint>((Func<Sheet, uint>)(s => s.SheetId.Value)).Max<uint>() + 1U;
                Sheet sheet1 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = 1, Name = "Total Polices Per Gender" };
                firstChild1.Append(sheet1);

                DocumentFormat.OpenXml.Spreadsheet.Row newChild1 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<string> stringList1 = new List<string>();
                stringList1.Add("Index");
                Cell newChild2 = new Cell();
                newChild2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild2.CellValue = new CellValue("Index");
                newChild1.AppendChild<Cell>(newChild2);
                stringList1.Add("Item");
                Cell newChild3 = new Cell();
                newChild3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild3.CellValue = new CellValue("Item");
                newChild1.AppendChild<Cell>(newChild3);
                stringList1.Add("Male");
                Cell newChild4 = new Cell();
                newChild4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild4.CellValue = new CellValue("Male");
                newChild1.AppendChild<Cell>(newChild4);
                stringList1.Add("Female");
                Cell newChild5 = new Cell();
                newChild5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild5.CellValue = new CellValue("Female");
                newChild1.AppendChild<Cell>(newChild5);
                sheetData1.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild1);
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                foreach (TotalPolicesPerGenderModel policesPerGenderModel in report.TotalPolicesPerGenderModel)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newChild6 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (string str2 in stringList1)
                    {
                        if (str2 == "Index")
                        {
                            Cell newChild7 = new Cell();
                            newChild7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild7.CellValue = new CellValue(policesPerGenderModel.Index.ToString());
                            newChild6.AppendChild<Cell>(newChild7);
                        }
                        else if (str2 == "Item")
                        {
                            Cell newChild8 = new Cell();
                            newChild8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild8.CellValue = new CellValue(policesPerGenderModel.Item.ToString());
                            newChild6.AppendChild<Cell>(newChild8);
                        }
                        else if (str2 == "Male")
                        {
                            Cell newChild9 = new Cell();
                            newChild9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild9.CellValue = new CellValue(policesPerGenderModel.TotalPolicesPerGenderMale.ToString());
                            newChild6.AppendChild<Cell>(newChild9);
                        }
                        else if (str2 == "Female")
                        {
                            Cell newChild10 = new Cell();
                            newChild10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild10.CellValue = new CellValue(policesPerGenderModel.TotalPolicesPerGenderFemale.ToString());
                            newChild6.AppendChild<Cell>(newChild10);
                        }
                    }
                    sheetData1.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild6);
                    spreadsheetDocument.WorkbookPart.Workbook.Save();
                }
                WorksheetPart worksheetPart2 = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData2 = new SheetData();
                worksheetPart2.Worksheet = new Worksheet(sheetData2);
                Sheets firstChild2 = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string idOfPart2 = spreadsheetDocument.WorkbookPart.GetIdOfPart((OpenXmlPart)worksheetPart2);
                uint num2 = 1;
                if (firstChild2.Elements<Sheet>().Count<Sheet>() > 0)
                    num2 = firstChild2.Elements<Sheet>().Select<Sheet, uint>((Func<Sheet, uint>)(s => s.SheetId.Value)).Max<uint>() + 1U;
                
                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet2 = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = idOfPart2, SheetId = 2, Name = "Count Of Polices Per Channel" };

                firstChild1.Append(sheet2);
                DocumentFormat.OpenXml.Spreadsheet.Row newChild11 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<string> stringList2 = new List<string>();
                stringList2.Add("Index");
                Cell newChild12 = new Cell();
                newChild12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild12.CellValue = new CellValue("Index");
                newChild11.AppendChild<Cell>(newChild12);
                stringList2.Add("Mechanism");
                Cell newChild13 = new Cell();
                newChild13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild13.CellValue = new CellValue("Mechanism");
                newChild11.AppendChild<Cell>(newChild13);
                stringList2.Add("CountOfPolices");
                Cell newChild14 = new Cell();
                newChild14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild14.CellValue = new CellValue("CountOfPolices");
                newChild11.AppendChild<Cell>(newChild14);
                sheetData2.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild11);
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                foreach (AllChannelModel allChannelModel in report.AllChannelModel)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newChild15 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (string str3 in stringList2)
                    {
                        if (str3 == "Index")
                        {
                            Cell newChild16 = new Cell();
                            newChild16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild16.CellValue = new CellValue(allChannelModel.Index.ToString());
                            newChild15.AppendChild<Cell>(newChild16);
                        }
                        else if (str3 == "Mechanism")
                        {
                            Cell newChild17 = new Cell();
                            newChild17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild17.CellValue = new CellValue(allChannelModel.Channel.ToString());
                            newChild15.AppendChild<Cell>(newChild17);
                        }
                        else if (str3 == "CountOfPolices")
                        {
                            Cell newChild18 = new Cell();
                            newChild18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild18.CellValue = new CellValue(allChannelModel.TotalPurchasedPolicies.ToString());
                            newChild15.AppendChild<Cell>(newChild18);
                        }
                    }
                    sheetData2.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild15);
                    spreadsheetDocument.WorkbookPart.Workbook.Save();
                }
                WorksheetPart worksheetPart3 = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData3 = new SheetData();
                worksheetPart3.Worksheet = new Worksheet(sheetData3);
                Sheets firstChild3 = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string idOfPart3 = spreadsheetDocument.WorkbookPart.GetIdOfPart((OpenXmlPart)worksheetPart3);
                uint num3 = 1;
                if (firstChild3.Elements<Sheet>().Count<Sheet>() > 0)
                    num3 = firstChild3.Elements<Sheet>().Select<Sheet, uint>((Func<Sheet, uint>)(s => s.SheetId.Value)).Max<uint>() + 1U;
                Sheet sheet3 = new Sheet()
                {
                    Id = idOfPart3,
                    SheetId = 3,
                    Name = "Total Polices Per Age"
                };
                firstChild1.Append(sheet3);
                DocumentFormat.OpenXml.Spreadsheet.Row newChild19 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<string> stringList3 = new List<string>();
                stringList3.Add("Index");
                Cell newChild20 = new Cell();
                newChild20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild20.CellValue = new CellValue("Index");
                newChild19.AppendChild<Cell>(newChild20);
                stringList3.Add("AgeRange");
                Cell newChild21 = new Cell();
                newChild21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild21.CellValue = new CellValue("Age Range");
                newChild19.AppendChild<Cell>(newChild21);
                stringList3.Add("Male");
                Cell newChild22 = new Cell();
                newChild22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild22.CellValue = new CellValue("Male");
                newChild19.AppendChild<Cell>(newChild22);
                stringList3.Add("Female");
                Cell newChild23 = new Cell();
                newChild23.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild23.CellValue = new CellValue("Female");
                newChild19.AppendChild<Cell>(newChild23);
                sheetData3.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild19);
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                foreach (TotalcontPerAgeForAllRanges allAgeRange in report.AllAgeRanges)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newChild24 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (string str4 in stringList3)
                    {
                        if (str4 == "Index")
                        {
                            Cell newChild25 = new Cell();
                            newChild25.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild25.CellValue = new CellValue(allAgeRange.Index.ToString());
                            newChild24.AppendChild<Cell>(newChild25);
                        }
                        else if (str4 == "AgeRange")
                        {
                            Cell newChild26 = new Cell();
                            newChild26.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild26.CellValue = new CellValue(allAgeRange.range.ToString());
                            newChild24.AppendChild<Cell>(newChild26);
                        }
                        else if (str4 == "Male")
                        {
                            Cell newChild27 = new Cell();
                            newChild27.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild27.CellValue = new CellValue(allAgeRange.totalCountMale.ToString());
                            newChild24.AppendChild<Cell>(newChild27);
                        }
                        else if (str4 == "Female")
                        {
                            Cell newChild28 = new Cell();
                            newChild28.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild28.CellValue = new CellValue(allAgeRange.totalCountFemale.ToString());
                            newChild24.AppendChild<Cell>(newChild28);
                        }
                    }
                    sheetData3.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild24);
                    spreadsheetDocument.WorkbookPart.Workbook.Save();
                }
                WorksheetPart worksheetPart4 = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData4 = new SheetData();
                worksheetPart4.Worksheet = new Worksheet(sheetData4);
                Sheets firstChild4 = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string idOfPart4 = spreadsheetDocument.WorkbookPart.GetIdOfPart((OpenXmlPart)worksheetPart4);
                uint num4 = 1;
                if (firstChild4.Elements<Sheet>().Count<Sheet>() > 0)
                    num4 = firstChild4.Elements<Sheet>().Select<Sheet, uint>((Func<Sheet, uint>)(s => s.SheetId.Value)).Max<uint>() + 1U;
                Sheet sheet4 = new Sheet()
                {
                    Id = idOfPart4,
                    SheetId = 4U,
                    Name = "Accounts Statictis"
                };
                firstChild1.Append(sheet4);
                DocumentFormat.OpenXml.Spreadsheet.Row newChild29 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<string> stringList4 = new List<string>();
                stringList4.Add("Index");
                Cell newChild30 = new Cell();
                newChild30.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild30.CellValue = new CellValue("Index");
                newChild29.AppendChild<Cell>(newChild30);
                stringList4.Add("Item");
                Cell newChild31 = new Cell();
                newChild31.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild31.CellValue = new CellValue("Item");
                newChild29.AppendChild<Cell>(newChild31);
                stringList4.Add("Individual");
                Cell newChild32 = new Cell();
                newChild32.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild32.CellValue = new CellValue("Individual");
                newChild29.AppendChild<Cell>(newChild32);
                stringList4.Add("Corporate");
                Cell newChild33 = new Cell();
                newChild33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild33.CellValue = new CellValue("Corporate");
                newChild29.AppendChild<Cell>(newChild33);
                sheetData4.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild29);
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                foreach (TotalInSystemModel totalInSystemModel in report.totalInSystemModel)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newChild34 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (string str5 in stringList4)
                    {
                        if (str5 == "Index")
                        {
                            Cell newChild35 = new Cell();
                            newChild35.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild35.CellValue = new CellValue(totalInSystemModel.Index.ToString());
                            newChild34.AppendChild<Cell>(newChild35);
                        }
                        else if (str5 == "Item")
                        {
                            Cell newChild36 = new Cell();
                            newChild36.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild36.CellValue = new CellValue(totalInSystemModel.Item);
                            newChild34.AppendChild<Cell>(newChild36);
                        }
                        else if (str5 == "Individual")
                        {
                            Cell newChild37 = new Cell();
                            newChild37.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild37.CellValue = new CellValue(totalInSystemModel.TotalIndividualUsersInTheSystem.ToString());
                            newChild34.AppendChild<Cell>(newChild37);
                        }
                        else if (str5 == "Corporate")
                        {
                            Cell newChild38 = new Cell();
                            newChild38.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild38.CellValue = new CellValue(totalInSystemModel.TotalCorporateUsersInTheSystem.ToString());
                            newChild34.AppendChild<Cell>(newChild38);
                        }
                    }
                    sheetData4.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild34);
                    spreadsheetDocument.WorkbookPart.Workbook.Save();
                }
                WorksheetPart worksheetPart5 = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData5 = new SheetData();
                worksheetPart5.Worksheet = new Worksheet(sheetData5);
                Sheets firstChild5 = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string idOfPart5 = spreadsheetDocument.WorkbookPart.GetIdOfPart((OpenXmlPart)worksheetPart5);
                uint num5 = 1;
                if (firstChild5.Elements<Sheet>().Count<Sheet>() > 0)
                    num5 = firstChild5.Elements<Sheet>().Select<Sheet, uint>((Func<Sheet, uint>)(s => s.SheetId.Value)).Max<uint>() + 1U;
                Sheet sheet5 = new Sheet()
                {
                    Id = idOfPart5,
                    SheetId = 5U,
                    Name = "Total Individual per Region"
                };
                firstChild1.Append(sheet5);
                DocumentFormat.OpenXml.Spreadsheet.Row newChild39 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<string> stringList5 = new List<string>();
                stringList5.Add("Index");
                Cell newChild40 = new Cell();
                newChild40.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild40.CellValue = new CellValue("Index");
                newChild39.AppendChild<Cell>(newChild40);
                stringList5.Add("Region Name");
                Cell newChild41 = new Cell();
                newChild41.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild41.CellValue = new CellValue("Region Name");
                newChild39.AppendChild<Cell>(newChild41);
                stringList5.Add("Total Individual polices");
                Cell newChild42 = new Cell();
                newChild42.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild42.CellValue = new CellValue("Total Individual polices");
                newChild39.AppendChild<Cell>(newChild42);
                sheetData5.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild39);
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                foreach (TotalPolicesPerCity totalPolicesPerCity in report.TotalIndividualPolicesPerCity)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newChild43 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (string str6 in stringList5)
                    {
                        if (str6 == "Index")
                        {
                            Cell newChild44 = new Cell();
                            newChild44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild44.CellValue = new CellValue(totalPolicesPerCity.Index.ToString());
                            newChild43.AppendChild<Cell>(newChild44);
                        }
                        else if (str6 == "Region Name")
                        {
                            Cell newChild45 = new Cell();
                            newChild45.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild45.CellValue = new CellValue(totalPolicesPerCity.RegionName);
                            newChild43.AppendChild<Cell>(newChild45);
                        }
                        else if (str6 == "Total Individual polices")
                        {
                            Cell newChild46 = new Cell();
                            newChild46.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild46.CellValue = new CellValue(totalPolicesPerCity.TotalPolices.ToString());
                            newChild43.AppendChild<Cell>(newChild46);
                        }
                    }
                    sheetData5.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild43);
                    spreadsheetDocument.WorkbookPart.Workbook.Save();
                }
                WorksheetPart worksheetPart6 = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData6 = new SheetData();
                worksheetPart6.Worksheet = new Worksheet(sheetData6);
                Sheets firstChild6 = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string idOfPart6 = spreadsheetDocument.WorkbookPart.GetIdOfPart((OpenXmlPart)worksheetPart6);
                uint num6 = 1;
                if (firstChild6.Elements<Sheet>().Count<Sheet>() > 0)
                    num6 = firstChild6.Elements<Sheet>().Select<Sheet, uint>((Func<Sheet, uint>)(s => s.SheetId.Value)).Max<uint>() + 1U;
                Sheet sheet6 = new Sheet()
                {
                    Id = idOfPart6,
                    SheetId = 6U,
                    Name = "Total Corprate per Region"
                };
                firstChild1.Append(sheet6);
                DocumentFormat.OpenXml.Spreadsheet.Row newChild47 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<string> stringList6 = new List<string>();
                stringList6.Add("Index");
                Cell newChild48 = new Cell();
                newChild48.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild48.CellValue = new CellValue("Index");
                newChild47.AppendChild<Cell>(newChild48);
                stringList6.Add("Region Name");
                Cell newChild49 = new Cell();
                newChild49.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild49.CellValue = new CellValue("Region Name");
                newChild47.AppendChild<Cell>(newChild49);
                stringList6.Add("Total Corprate polices");
                Cell newChild50 = new Cell();
                newChild50.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild50.CellValue = new CellValue("Total Corprate polices");
                newChild47.AppendChild<Cell>(newChild50);
                sheetData6.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild47);
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                foreach (TotalPolicesPerCity totalPolicesPerCity in report.TotalCorpratePolicesPerCity)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newChild51 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (string str7 in stringList6)
                    {
                        if (str7 == "Index")
                        {
                            Cell newChild52 = new Cell();
                            newChild52.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild52.CellValue = new CellValue(totalPolicesPerCity.Index.ToString());
                            newChild51.AppendChild<Cell>(newChild52);
                        }
                        else if (str7 == "Region Name")
                        {
                            Cell newChild53 = new Cell();
                            newChild53.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild53.CellValue = new CellValue(totalPolicesPerCity.RegionName);
                            newChild51.AppendChild<Cell>(newChild53);
                        }
                        else if (str7 == "Total Corprate polices")
                        {
                            Cell newChild54 = new Cell();
                            newChild54.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild54.CellValue = new CellValue(totalPolicesPerCity.TotalPolices.ToString());
                            newChild51.AppendChild<Cell>(newChild54);
                        }
                    }
                    sheetData6.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild51);
                    spreadsheetDocument.WorkbookPart.Workbook.Save();
                }
                WorksheetPart worksheetPart7 = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData7 = new SheetData();
                worksheetPart7.Worksheet = new Worksheet(sheetData7);
                Sheets firstChild7 = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string idOfPart7 = spreadsheetDocument.WorkbookPart.GetIdOfPart((OpenXmlPart)worksheetPart7);
                uint num7 = 1;
                if (firstChild7.Elements<Sheet>().Count<Sheet>() > 0)
                    num7 = firstChild7.Elements<Sheet>().Select<Sheet, uint>((Func<Sheet, uint>)(s => s.SheetId.Value)).Max<uint>() + 1U;
                Sheet sheet7 = new Sheet()
                {
                    Id = idOfPart7,
                    SheetId = 7U,
                    Name = "Total  Payment method"
                };
                firstChild1.Append(sheet7);
                DocumentFormat.OpenXml.Spreadsheet.Row newChild55 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<string> stringList7 = new List<string>();
                stringList7.Add("Index");
                Cell newChild56 = new Cell();
                newChild56.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild56.CellValue = new CellValue("Index");
                newChild55.AppendChild<Cell>(newChild56);
                stringList7.Add("payment Method");
                Cell newChild57 = new Cell();
                newChild57.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild57.CellValue = new CellValue("payment Method");
                newChild55.AppendChild<Cell>(newChild57);
                stringList7.Add("Total polices");
                Cell newChild58 = new Cell();
                newChild58.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                newChild58.CellValue = new CellValue("Total polices");
                newChild55.AppendChild<Cell>(newChild58);
                sheetData7.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild55);
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                foreach (AllPaymentMethodModel paymentMethodModel in report.AllPaymentMethodModel)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row newChild59 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (string str8 in stringList7)
                    {
                        if (str8 == "Index")
                        {
                            Cell newChild60 = new Cell();
                            newChild60.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild60.CellValue = new CellValue(paymentMethodModel.Index.ToString());
                            newChild59.AppendChild<Cell>(newChild60);
                        }
                        else if (str8 == "payment Method")
                        {
                            Cell newChild61 = new Cell();
                            newChild61.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild61.CellValue = new CellValue(paymentMethodModel.paymentMethod);
                            newChild59.AppendChild<Cell>(newChild61);
                        }
                        else if (str8 == "Total polices")
                        {
                            Cell newChild62 = new Cell();
                            newChild62.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            newChild62.CellValue = new CellValue(paymentMethodModel.totalPolices.ToString());
                            newChild59.AppendChild<Cell>(newChild62);
                        }
                    }
                    sheetData7.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Row>(newChild59);
                    spreadsheetDocument.WorkbookPart.Workbook.Save();
                }
                spreadsheetDocument.Close();
            }
            return GetFileAsByte(SPREADSHEET_NAME);
        }
        #endregion

        #region
        public byte[] GenerateExcelPoliciesStatisticsInfo(List<PolicyStatisticsDataModel> policies)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PolicyStatistics" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Policy Statistics" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();

                    columns.Add("CreatedDateTime");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDateTime");
                    headerRow.AppendChild(cell1);

                    columns.Add("PolicyNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PolicyNo");
                    headerRow.AppendChild(cell2);

                    columns.Add("InsuredCity");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredCity");
                    headerRow.AppendChild(cell3);

                    columns.Add("SelectedInsuranceTypeCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SelectedInsuranceTypeCode");
                    headerRow.AppendChild(cell4);


                    columns.Add("VehicleBodyType");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleBodyType");
                    headerRow.AppendChild(cell5);


                    columns.Add("VehicleMaker");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMaker");
                    headerRow.AppendChild(cell6);


                    columns.Add("VehicleModel");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModel");
                    headerRow.AppendChild(cell7);


                    columns.Add("ModelYear");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ModelYear");
                    headerRow.AppendChild(cell8);


                    columns.Add("InsuredId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredId");
                    headerRow.AppendChild(cell9);


                    columns.Add("InsuredBirthDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredBirthDate");
                    headerRow.AppendChild(cell10);


                    columns.Add("InsuredBirthDateH");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredBirthDateH");
                    headerRow.AppendChild(cel11);


                    columns.Add("GenderId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("GenderId");
                    headerRow.AppendChild(cel12);


                    columns.Add("Driver1_DateOfBirthG");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Driver1_DateOfBirthG");
                    headerRow.AppendChild(cel13);


                    columns.Add("Driver1_GenderCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Driver1_GenderCode");
                    headerRow.AppendChild(cel14);


                    columns.Add("Driver2_DateOfBirthG");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Driver2_DateOfBirthG");
                    headerRow.AppendChild(cel15);


                    columns.Add("Driver2_GenderCode");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Driver2_GenderCode");
                    headerRow.AppendChild(cel16);


                    columns.Add("PriceTypeCode_7_Value");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_7_Value");
                    headerRow.AppendChild(cel17);


                    columns.Add("PriceTypeCode_1_Value");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_1_Value");
                    headerRow.AppendChild(cel18);


                    columns.Add("PriceTypeCode_1_Percentage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_1_Percentage");
                    headerRow.AppendChild(cel19);


                    columns.Add("PriceTypeCode_2_Value");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_2_Value");
                    headerRow.AppendChild(cel20);

                    
  
                    columns.Add("PriceTypeCode_2_Percentag");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_2_Percentag");
                    headerRow.AppendChild(cel21);

                    
                    columns.Add("PriceTypeCode_3_Value");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_3_Value");
                    headerRow.AppendChild(cel22);

                    
                    columns.Add("PriceTypeCode_3_Percentage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel23 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel23.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel23.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_3_Percentage");
                    headerRow.AppendChild(cel23);

                      
                    columns.Add("PriceTypeCode_8_Value");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel24 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel24.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel24.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_8_Value");
                    headerRow.AppendChild(cel24);

                      
                    columns.Add("PriceTypeCode_8_Percentage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel25 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel25.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_8_Percentage");
                    headerRow.AppendChild(cel25);

                    
                         
                    columns.Add("PriceTypeCode_9_Value");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel26 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel26.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel26.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_9_Value");
                    headerRow.AppendChild(cel26);

                      
                    columns.Add("PriceTypeCode_9_Percentage");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel27 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel27.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel27.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceTypeCode_9_Percentage");
                    headerRow.AppendChild(cel27);

                    
                    columns.Add("ProductPrice");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cel28 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cel28.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cel28.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ProductPrice");
                    headerRow.AppendChild(cel28);

                    

                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (PolicyStatisticsDataModel policy in policies)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "CreatedDateTime")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.CreatedDateTime.ToString());
                                newRow.AppendChild(cell11);
                            }
                            else if (col == "PolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PolicyNo);
                                newRow.AppendChild(cell22);
                            }
                            else if (col == "InsuredCity")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredCity);
                                newRow.AppendChild(cell33);
                            }
                            else if (col == "SelectedInsuranceTypeCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.SelectedInsuranceTypeCode.ToString());
                                newRow.AppendChild(cell44);
                            }
                            else if (col == "VehicleBodyType")
                            {
                                // Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleBodyType);
                                newRow.AppendChild(cell55);
                            }
                            else if (col == "VehicleMaker")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleMaker);
                                newRow.AppendChild(cell66);
                            }
                            else if (col == "VehicleModel")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.VehicleModel);
                                newRow.AppendChild(cell77);
                            }
                            else if (col == "InsuredId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredId);
                                newRow.AppendChild(cell88);
                            }
                            else if (col == "ModelYear")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ModelYear.ToString());
                                newRow.AppendChild(cell99);
                            }
                            else if (col == "InsuredBirthDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredBirthDate.ToString());
                                newRow.AppendChild(cell100);
                            }
                            else if (col == "InsuredBirthDateH")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.InsuredBirthDateH);
                                newRow.AppendChild(cell111);
                            }
                            else if (col == "GenderId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell112 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell112.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell112.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.GenderId.ToString());
                                newRow.AppendChild(cell112);
                            }
                            else if (col == "Driver1_DateOfBirthG")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell113 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell113.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell113.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Driver1_DateOfBirthG.ToString());
                                newRow.AppendChild(cell113);
                            } 
                            else if (col == "Driver1_GenderCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell114 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell114.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell114.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Driver1_GenderCode.ToString());
                                newRow.AppendChild(cell114);
                            }  
                            else if (col == "Driver2_DateOfBirthG")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell115 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell115.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell115.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Driver2_DateOfBirthG.ToString());
                                newRow.AppendChild(cell115);
                            }  
                            else if (col == "Driver2_GenderCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell116 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell116.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell116.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.Driver2_GenderCode.ToString());
                                newRow.AppendChild(cell116);
                            }  
                            else if (col == "PriceTypeCode_7_Value")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell117 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell117.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell117.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_7_Value.ToString());
                                newRow.AppendChild(cell117);
                            } 
                            else if (col == "PriceTypeCode_1_Value")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell118 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell118.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell118.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_1_Value.ToString());
                                newRow.AppendChild(cell118);
                            } 
                       
                            else if (col == "PriceTypeCode_1_Percentage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell119 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell119.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell119.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_1_Percentage.ToString());
                                newRow.AppendChild(cell119);
                            }
                            else if (col == "PriceTypeCode_2_Value")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell120 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell120.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell120.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_2_Value.ToString());
                                newRow.AppendChild(cell120);
                            }
                            else if (col == "PriceTypeCode_2_Percentag")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell121 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell121.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell121.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_2_Percentage.ToString());
                                newRow.AppendChild(cell121);
                            } 
                            else if (col == "PriceTypeCode_3_Value")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_3_Value.ToString());
                                newRow.AppendChild(cell122);
                            } 
                            else if (col == "PriceTypeCode_3_Percentage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell123 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell123.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell123.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_3_Percentage.ToString());
                                newRow.AppendChild(cell123);
                            } 
                            else if (col == "PriceTypeCode_8_Value")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell124 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell124.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell124.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_8_Value.ToString());
                                newRow.AppendChild(cell124);
                            }
                          else if (col == "PriceTypeCode_8_Percentage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell125 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell125.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell125.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_8_Percentage.ToString());
                                newRow.AppendChild(cell125);
                            } 
                            else if (col == "PriceTypeCode_9_Value")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell126 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell126.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell126.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_9_Value.ToString());
                                newRow.AppendChild(cell126);
                            } 
                            else if (col == "PriceTypeCode_9_Percentage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell127 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell127.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell127.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.PriceTypeCode_9_Percentage.ToString());
                                newRow.AppendChild(cell127);
                            }
                            else if (col == "ProductPrice")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell128 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell128.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell128.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policy.ProductPrice.ToString());
                                newRow.AppendChild(cell128);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }


        #endregion

        #region Renewal Export

        public byte[] ExportRenewalData(List<RenewalDataModel> data, string name)
        {
            DateTime dt = DateTime.Now;
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SuccessPoliciesInfo" + dt.ToString("dd-MM-yyyy"));
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
                        sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "SuccessPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();

                    columns.Add("OldReferenceId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceID");
                    headerRow.AppendChild(cell1);

                    columns.Add("OldPolicyNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy No.");
                    headerRow.AppendChild(cell2);


                    columns.Add("OldCompanyName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurance Company");
                    headerRow.AppendChild(cell3);

                    columns.Add("OldProductType");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Product Type");
                    headerRow.AppendChild(cell4);

                    columns.Add("ExpiryDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Expiry Date");
                    headerRow.AppendChild(cell5);

                    columns.Add("RenewalDate");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Renewal Date");
                    headerRow.AppendChild(cell6);

                    columns.Add("LastAmount");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Last Amount");
                    headerRow.AppendChild(cell7);

                    columns.Add("NewReferenceId");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("New ReferenceId");
                    headerRow.AppendChild(cell8);

                    columns.Add("NewPolicyNo");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("New Policy No.");
                    headerRow.AppendChild(cell9);

                    columns.Add("CompanyName");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insurance Company");
                    headerRow.AppendChild(cell10);

                    columns.Add("ProductType");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Product Type");
                    headerRow.AppendChild(cell11);

                    columns.Add("CurrentAmount");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Current Amount");
                    headerRow.AppendChild(cell12);

                    columns.Add("SequenceNumber");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Sequence Number");
                    headerRow.AppendChild(cell13);

                    columns.Add("Nin");
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NationalId");
                    headerRow.AppendChild(cell14);

                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();

                    foreach (var item in data)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "OldReferenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell01 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell01.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell01.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.OldReferenceId);
                                newRow.AppendChild(cell01);
                            }
                            else if (col == "OldPolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell02 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell02.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell02.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.OldPolicyNo);
                                newRow.AppendChild(cell02);
                            }
                            if (col == "OldCompanyName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell03 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell03.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell03.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.OldCompanyName);
                                newRow.AppendChild(cell03);
                            }
                            else if (col == "OldProductType")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell04 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell04.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell04.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.OldProductType);
                                newRow.AppendChild(cell04);
                            }
                            else if (col == "ExpiryDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell05 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell05.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell05.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ExpiryDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell05);
                            }
                            else if (col == "RenewalDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell06 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell06.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell06.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.RenewalDate?.ToString("dd-MM-yyyy"));
                                newRow.AppendChild(cell06);
                            }
                            else if (col == "LastAmount")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell07 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell07.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell07.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.LastAmount?.ToString());
                                newRow.AppendChild(cell07);
                            }
                            if (col == "NewReferenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell08 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell08.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell08.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NewReferenceId);
                                newRow.AppendChild(cell08);
                            }
                            else if (col == "NewPolicyNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell09 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell09.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell09.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NewPolicyNo);
                                newRow.AppendChild(cell09);
                            }
                            else if (col == "CompanyName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell010 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell010.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell010.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CompanyName);
                                newRow.AppendChild(cell010);
                            }
                            else if (col == "ProductType")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell011 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell011.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell011.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ProductType);
                                newRow.AppendChild(cell011);
                            }
                            else if (col == "CurrentAmount")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell012 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell012.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell012.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CurrentAmount?.ToString());
                                newRow.AppendChild(cell012);
                            }
                            else if (col == "SequenceNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell013 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell013.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell013.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.SequenceNumber);
                                newRow.AppendChild(cell013);
                            }
                            else if (col == "Nin")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell014 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell014.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell014.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CarOwnerNIN);
                                newRow.AppendChild(cell014);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }

                }

                workbook.Close();
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion
        #region Commission and Fees
        public byte[] GenerateExcelCommissionAndFees(List<CommissionAndFeesModel> commissions)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CommissionAndFees" + dt.ToString("dd-MM-yyyy"));
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Successcommission" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();

                    {
                        columns.Add("CompanyId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompanyId");
                        headerRow.AppendChild(cell);


                        columns.Add("CompanyKey");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompanyKey");
                        headerRow.AppendChild(cell2);

                        columns.Add("InsuranceTypeCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuranceTypeCode");
                        headerRow.AppendChild(cell3);


                        columns.Add("Key");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Key");
                        headerRow.AppendChild(cell5);


                        columns.Add("Percentage");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Percentage");
                        headerRow.AppendChild(cell6);


                        columns.Add("FixedFees");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("FixedFees");
                        headerRow.AppendChild(cell7);


                        columns.Add("CalculatedFromBasic");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CalculatedFromBasic");
                        headerRow.AppendChild(cell8);

                        columns.Add("IsCommission");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IsCommission");
                        headerRow.AppendChild(cell9);

                        columns.Add("PaymentMethodId");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PaymentMethodId");
                        headerRow.AppendChild(cell10);

                        columns.Add("CreatedDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cell12);

                        columns.Add("CreatedBy");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedBy");
                        headerRow.AppendChild(cell13);

                        columns.Add("ModifiedDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ModifiedDate");
                        headerRow.AppendChild(cell14);

                        columns.Add("ModifiedBy");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ModifiedBy");
                        headerRow.AppendChild(cell15);

                        columns.Add("IncludeAdditionalDriver");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IncludeAdditionalDriver");
                        headerRow.AppendChild(cell16);

                        columns.Add("IsPercentageNegative");
                        
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IsPercentageNegative");
                        headerRow.AppendChild(cell17);
                        columns.Add("IsFixedFeesNegative");
                        
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IsFixedFeesNegative");
                        headerRow.AppendChild(cell18);
                    }

                    sheetData.AppendChild(headerRow);

                    workbook.WorkbookPart.Workbook.Save();

                    foreach (CommissionAndFeesModel commission in commissions)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();


                        foreach (string col in columns)
                        {
                            if (col == "CompanyId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.CompanyId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CompanyKey")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.CompanyKey); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuranceTypeCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.InsuranceTypeCode.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Key")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.Key); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Percentage")
                            {
                                //  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.Percentage?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "FixedFees")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.FixedFees?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CalculatedFromBasic")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.CalculatedFromBasic.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "IsCommission")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.IsCommission.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PaymentMethodId")
                            {
                                ///  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.PaymentMethodId?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CreatedDate")
                            {
                                ///  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.CreatedDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }

                            else if (col == "CreatedBy")
                            {
                                /// Loza
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.CreatedBy?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ModifiedDate")
                            {
                                ///  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.ModifiedDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ModifiedBy")
                            {
                                ///  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.ModifiedBy?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "IncludeAdditionalDriver")
                            {
                                ///  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.IncludeAdditionalDriver?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "IsPercentageNegative")
                            {
                                ///  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.IsPercentageNegative.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "IsFixedFeesNegative")
                            {
                                ///  
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(commission.IsFixedFeesNegative.ToString()); //
                                newRow.AppendChild(cell);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }

                }

                workbook.Close();
            }


            return GetFileAsByte(SPREADSHEET_NAME);

        }
        #endregion
        #region Policy Details Report
        public byte[] GeneratePoliciesDetailsExcel(List<CheckOutInfo> CheckOutDetails)
        {
            if (CheckOutDetails != null)
            {
                DateTime dt = DateTime.Now;
                string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PoliciesDetails" + dt.ToString("dd-MM-yyyy"));
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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PoliciesDetails" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        List<String> columns = new List<string>();

                        columns.Add("Name");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Name");
                        headerRow.AppendChild(cell1);

                        columns.Add("Reference No");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reference No");
                        headerRow.AppendChild(cell2);

                        columns.Add("Insured national id");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured national id");
                        headerRow.AppendChild(cell3);

                        columns.Add("Policy No");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy No");
                        headerRow.AppendChild(cell4);

                        columns.Add("Issue date");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Issue date");
                        headerRow.AppendChild(cell5);

                        columns.Add("NIN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NIN");
                        headerRow.AppendChild(cell6);

                        columns.Add("IBAN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IBAN");
                        headerRow.AppendChild(cell7);

                        columns.Add("Insured phone");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured phone");
                        headerRow.AppendChild(cell8);

                        columns.Add("User email");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("User email");
                        headerRow.AppendChild(cell9);

                        columns.Add("Channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell10);

                        sheetData.AppendChild(headerRow);
                        workbook.WorkbookPart.Workbook.Save();

                        foreach (var item in CheckOutDetails)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (string col in columns)
                            {
                                if (col == "Name")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.FullName);
                                    newRow.AppendChild(cell11);
                                }
                                else if (col == "Reference No")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ReferenceId);
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "Insured national id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NationalId);
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "Policy No")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyNo);
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "Issue date")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyIssueDate.ToString());
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "NIN")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NIN);
                                    newRow.AppendChild(cell11);
                                }
                                else if (col == "IBAN")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.IBAN);
                                    newRow.AppendChild(cell11);
                                }
                                else if (col == "Insured phone")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Phone);
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "User email")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Email);
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Channel);
                                    newRow.AppendChild(cell88);
                                }
                            }

                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }

                    }

                    workbook.Close();
                }

                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #endregion
        #region Policy Najm ResponseTime Export
        public byte[] GetNajmResponseTimeForConnectWithPolicyExcel(List<NajmResponseTimeModel> NajmResponseTimeModel)
        {
            if (NajmResponseTimeModel != null)
            {
                DateTime dt = DateTime.Now;
                string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PoliciesDetails" + dt.ToString("dd-MM-yyyy"));
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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PoliciesDetails" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        List<String> columns = new List<string>();

                        columns.Add("Name");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Name");
                        headerRow.AppendChild(cell1);

                        columns.Add("Reference No");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reference No");
                        headerRow.AppendChild(cell2);

                        columns.Add("Insured national id");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured national id");
                        headerRow.AppendChild(cell3);

                        columns.Add("Policy No");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell4 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell4.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell4.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy No");
                        headerRow.AppendChild(cell4);

                        columns.Add("Vehicle Id");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell40 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell40.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell40.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Vehicle Id");
                        headerRow.AppendChild(cell40);

                        columns.Add("Issue date");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Issue date");
                        headerRow.AppendChild(cell5);

                        columns.Add("NIN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NIN");
                        headerRow.AppendChild(cell6);

                        columns.Add("Policy Effective Date");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Effective Date");
                        headerRow.AppendChild(cell7);

                        columns.Add("Insured phone");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured phone");
                        headerRow.AppendChild(cell8);

                        columns.Add("User email");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("User email");
                        headerRow.AppendChild(cell9);

                        columns.Add("Policy Expire Date");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Expire Date");
                        headerRow.AppendChild(cell10);

                        columns.Add("Najm Status");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Najm Status");
                        headerRow.AppendChild(cell100);

                        columns.Add("Response Time");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell200 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell200.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell200.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Response Time");
                        headerRow.AppendChild(cell200);

                        sheetData.AppendChild(headerRow);
                        workbook.WorkbookPart.Workbook.Save();

                        foreach (var item in NajmResponseTimeModel)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (string col in columns)
                            {
                                if (col == "Name")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.FullName);
                                    newRow.AppendChild(cell11);
                                }
                                else if (col == "Reference No")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ReferenceId);
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "Insured national id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NationalId);
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "Policy No")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyNo);
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "Vehicle Id")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell444 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell444.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell444.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleId);
                                    newRow.AppendChild(cell444);
                                }
                                else if (col == "Issue date")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyIssueDate.ToString());
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "NIN")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NIN);
                                    newRow.AppendChild(cell11);
                                }
                                else if (col == "Policy Effective Date")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyEffectiveDate.ToString());
                                    newRow.AppendChild(cell11);
                                }
                                else if (col == "Insured phone")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Phone);
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "User email")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Email);
                                    newRow.AppendChild(cell77);
                                }
                                else if (col == "Policy Expire Date")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyExpiryDate.ToString());
                                    newRow.AppendChild(cell88);
                                }

                                else if (col == "Najm Status")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.NajmStatus);
                                    newRow.AppendChild(cell99);
                                }

                                else if (col == "Response Time")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell299 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell299.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell299.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ResponseTime.ToString());
                                    newRow.AppendChild(cell299);
                                }
                            }

                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }

                    }

                    workbook.Close();
                }

                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }
        #endregion

        #region Pending Processing Queue

        public byte[] GenerateExcelPendingPolicies(List<ProcessingQueueInfo> pendingPolicies)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PendingPolicy" + dt.ToString("dd-MM-yyyy"));
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PendingPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    {
                        columns.Add("createdDate");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("createdDate");
                        headerRow.AppendChild(cell);

                        columns.Add("referenceId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("referenceId");
                        headerRow.AppendChild(cell2);


                        columns.Add("triesNumber");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("triesNumber");
                        headerRow.AppendChild(cell3);

                        columns.Add("errorDescription");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("errorDescription");
                        headerRow.AppendChild(cell5);


                        columns.Add("companyName");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("companyName");
                        headerRow.AppendChild(cell6);


                        columns.Add("nationalId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("nationalId");
                        headerRow.AppendChild(cell7);


                        columns.Add("vehicleId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("vehicleId");
                        headerRow.AppendChild(cell8);

                        columns.Add("serviceRequest");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("serviceRequest");
                        headerRow.AppendChild(cell9);

                        columns.Add("serviceResponse");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("serviceResponse");
                        headerRow.AppendChild(cell10);

                    }

                    sheetData.AppendChild(headerRow);

                    workbook.WorkbookPart.Workbook.Save();

                    foreach (ProcessingQueueInfo pendingPolicy in pendingPolicies)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "createdDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.CreatedDate.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "referenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.ReferenceId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "triesNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.ProcessingTries.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "errorDescription")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.ErrorDescription); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "companyName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.CompanyName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "nationalId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.DriverNin); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "vehicleId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.VehicleId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "serviceRequest")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.ServiceRequest); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "serviceResponse")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(pendingPolicy.ServiceResponse); //
                                newRow.AppendChild(cell);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }

                }
                workbook.Close();
            }
            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion

        #region MyRegion

        public byte[] GenearetOldQuotation(List<OldQuotationDetails> oldQuotationDetails)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OldQuotation" + dt.ToString("dd-MM-yyyy"));
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PendingPolicy" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    {
                        columns.Add("RequestId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RequestId");
                        headerRow.AppendChild(cell);

                        columns.Add("InsuranceTypeCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuranceTypeCode");
                        headerRow.AppendChild(cell2);


                        columns.Add("VehicleAgencyRepair");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleAgencyRepair");
                        headerRow.AppendChild(cell3);

                        columns.Add("ReferenceId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceId");
                        headerRow.AppendChild(cell5);

                        columns.Add("CompanyName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompanyName");
                        headerRow.AppendChild(cell6);

                        columns.Add("CreateDateTime");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreateDateTime");
                        headerRow.AppendChild(cell7);


                        columns.Add("ICQuoteReferenceNo");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ICQuoteReferenceNo");
                        headerRow.AppendChild(cell8);

                        columns.Add("PromotionProgramCode");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PromotionProgramCode");
                        headerRow.AppendChild(cell9);

                        columns.Add("NationalId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NationalId");
                        headerRow.AppendChild(cell10);

                        columns.Add("CardIdTypeId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CardIdTypeId");
                        headerRow.AppendChild(cell11);


                        columns.Add("InsuredBirthDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredBirthDate");
                        headerRow.AppendChild(cell12);

                        columns.Add("InsuredBirthDateH");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell113 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell113.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell113.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredBirthDateH");
                        headerRow.AppendChild(cell113);

                        columns.Add("InsuredGenderId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredGenderId");
                        headerRow.AppendChild(cell14);

                        columns.Add("InsuerdNationalityCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuerdNationalityCode");
                        headerRow.AppendChild(cell15);

                        columns.Add("InsuredFirstNameAr");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell16 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell16.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell16.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredFirstNameAr");
                        headerRow.AppendChild(cell16);

                        columns.Add("InsuredMiddleNameAr");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell17 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell17.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell17.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredMiddleNameAr");
                        headerRow.AppendChild(cell17);
                        columns.Add("InsuredLastNameAr");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell18 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell18.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell18.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredLastNameAr");
                        headerRow.AppendChild(cell18);

                        columns.Add("InsuredFirstNameEn");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell19 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell19.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell19.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredFirstNameEn");
                        headerRow.AppendChild(cell19);

                        columns.Add("InsuredMiddleNameEn");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell20 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell20.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell20.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredMiddleNameEn");
                        headerRow.AppendChild(cell20);


                        columns.Add("InsuredLastNameEn");

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell21 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell21.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell21.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredLastNameEn");
                        headerRow.AppendChild(cell21);

                        columns.Add("InsuredSocialStatusId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredSocialStatusId");
                        headerRow.AppendChild(cell22);

                        columns.Add("InsuredOccupationId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell23 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell23.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell23.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredOccupationId");
                        headerRow.AppendChild(cell23);


                        columns.Add("InsuredSocialStatusId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell25 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell25.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredSocialStatusId");
                        headerRow.AppendChild(cell25);

                        columns.Add("InsuredEducationId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell26 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell26.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell26.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredEducationId");
                        headerRow.AppendChild(cell26);

                        columns.Add("InsuredChildrenBelow16Years");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell27 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell27.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell27.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("InsuredChildrenBelow16Years");
                        headerRow.AppendChild(cell27);

                        columns.Add("DriverOccupationCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell28 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell28.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell28.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverOccupationCode");
                        headerRow.AppendChild(cell28);

                        columns.Add("DriverOccupationName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell29 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell29.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell29.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverOccupationName");
                        headerRow.AppendChild(cell29);

                        columns.Add("DriverEnglishFirstName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell30 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell30.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell30.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverEnglishFirstName");
                        headerRow.AppendChild(cell30);

                        columns.Add("DriverEnglishLastName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell31 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell31.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell31.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverEnglishLastName");
                        headerRow.AppendChild(cell31);

                        columns.Add("DriverEnglishSecondName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell32 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell32.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell32.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverEnglishSecondName");
                        headerRow.AppendChild(cell32);

                        columns.Add("DriverEnglishThirdName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverEnglishThirdName");
                        headerRow.AppendChild(cell33);

                        columns.Add("DriverLastName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell34 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell34.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell34.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverLastName");
                        headerRow.AppendChild(cell34);

                        columns.Add("DriverSecondName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell35 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell35.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell35.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverSecondName");
                        headerRow.AppendChild(cell35);

                        columns.Add("DriverFirstName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell36 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell36.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell36.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverFirstName");
                        headerRow.AppendChild(cell36);

                        columns.Add("DriverThirdName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell37 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell37.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell37.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverThirdName");
                        headerRow.AppendChild(cell37);

                        columns.Add("DriverSubtribeName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell38 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell38.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell38.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverSubtribeName");
                        headerRow.AppendChild(cell38);

                        columns.Add("DriverDateOfBirthG");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell39 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell39.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell39.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverDateOfBirthG");
                        headerRow.AppendChild(cell39);

                        columns.Add("DriverNationalityCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell40 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell40.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell40.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverNationalityCode");
                        headerRow.AppendChild(cell40);

                        columns.Add("DriverDateOfBirthH");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell41 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell41.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell41.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverDateOfBirthH");
                        headerRow.AppendChild(cell41);

                        columns.Add("DriverNIN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell42 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell42.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell42.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverNIN");
                        headerRow.AppendChild(cell42);

                        columns.Add("DriverIdIssuePlace");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell43 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell43.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell43.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverIdIssuePlace");
                        headerRow.AppendChild(cell43);

                        columns.Add("DriverIdExpiryDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverIdExpiryDate");
                        headerRow.AppendChild(cell44);

                        columns.Add("DriverResidentOccupation");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell45 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell45.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell45.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverResidentOccupation");
                        headerRow.AppendChild(cell45);

                        columns.Add("DrivingPercentage");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell46 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell46.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell46.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DrivingPercentage");
                        headerRow.AppendChild(cell46);

                        columns.Add("DriverChildrenBelow16Years");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell47 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell47.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell47.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("DriverChildrenBelow16Years");
                        headerRow.AppendChild(cell47);

                        columns.Add("NOALast5Years");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell48 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell48.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell48.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NOALast5Years");
                        headerRow.AppendChild(cell48);

                        columns.Add("NOCLast5Years");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell49 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell49.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell49.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NOCLast5Years");
                        headerRow.AppendChild(cell49);

                        columns.Add("NCDFreeYears");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell50 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell50.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell50.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NCDFreeYears");
                        headerRow.AppendChild(cell50);


                        columns.Add("NCDReference");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell51 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell51.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell51.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NCDReference");
                        headerRow.AppendChild(cell51);

                        columns.Add("SaudiLicenseHeldYears");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell52 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell52.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell52.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SaudiLicenseHeldYears");
                        headerRow.AppendChild(cell52);

                        columns.Add("CityName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell53 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell53.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell53.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CityName");
                        headerRow.AppendChild(cell53);

                        columns.Add("OccupationName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("OccupationName");
                        headerRow.AppendChild(cell55);

                        columns.Add("EducationName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell56 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell56.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell56.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("EducationName");
                        headerRow.AppendChild(cell56);

                        columns.Add("SocialStatusName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell57 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell57.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell57.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SocialStatusName");
                        headerRow.AppendChild(cell57);

                        columns.Add("PostCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell58 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell58.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell58.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PostCode");
                        headerRow.AppendChild(cell58);


                        columns.Add("Licenses");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell59 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell59.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell59.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Licenses");
                        headerRow.AppendChild(cell59);

                        columns.Add("Violations");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell60 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell60.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell60.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Violations");
                        headerRow.AppendChild(cell60);

                        columns.Add("SequenceNumber");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell61 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell61.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell61.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("SequenceNumber");
                        headerRow.AppendChild(cell61);

                        columns.Add("CustomCardNumber");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell62 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell62.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell62.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CustomCardNumber");
                        headerRow.AppendChild(cell62);

                        columns.Add("Cylinders");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell63 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell63.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell63.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Cylinders");
                        headerRow.AppendChild(cell63);

                        columns.Add("LicenseExpiryDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell64 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell64.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell64.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("LicenseExpiryDate");
                        headerRow.AppendChild(cell64);

                        columns.Add("MajorColor");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell65 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell65.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell65.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MajorColor");
                        headerRow.AppendChild(cell65);

                        columns.Add("MinorColor");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("MinorColor");
                        headerRow.AppendChild(cell66);

                        columns.Add("ModelYear");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell67 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell67.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell67.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ModelYear");
                        headerRow.AppendChild(cell67);

                        columns.Add("PlateTypeCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell68 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell68.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell68.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PlateTypeCode");
                        headerRow.AppendChild(cell68);

                        columns.Add("RegisterationPlace");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell69 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell69.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell69.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("RegisterationPlace");
                        headerRow.AppendChild(cell69);

                        columns.Add("VehicleBodyCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell70 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell70.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell70.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleBodyCode");
                        headerRow.AppendChild(cell70);

                        columns.Add("VehicleWeight");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell71 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell71.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell71.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleWeight");
                        headerRow.AppendChild(cell71);

                        columns.Add("VehicleLoad");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell72 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell72.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell72.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleLoad");
                        headerRow.AppendChild(cell72);

                        columns.Add("VehicleMaker");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell73 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell73.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell73.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMaker");
                        headerRow.AppendChild(cell73);

                        columns.Add("VehicleModel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell74 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell74.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell74.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModel");
                        headerRow.AppendChild(cell74);

                        columns.Add("ChassisNumber");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell75 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell75.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell75.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ChassisNumber");
                        headerRow.AppendChild(cell75);

                        columns.Add("VehicleMakerCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell76 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell76.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell76.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleMakerCode");
                        headerRow.AppendChild(cell76);

                        columns.Add("VehicleModelCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell77 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell77.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell77.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleModelCode");
                        headerRow.AppendChild(cell77);

                        columns.Add("CarPlateText1");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell78 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell78.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell78.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CarPlateText1");
                        headerRow.AppendChild(cell78);

                        columns.Add("CarPlateText2");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell79 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell79.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell79.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CarPlateText2");
                        headerRow.AppendChild(cell79);

                        columns.Add("CarPlateText3");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell80 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell80.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell80.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ChassiCarPlateText3sNumber");
                        headerRow.AppendChild(cell80);

                        columns.Add("CarPlateNumber");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell81 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell81.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell81.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CarPlateNumber");
                        headerRow.AppendChild(cell81);

                        columns.Add("CarOwnerNIN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell82 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell82.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell82.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CarOwnerNIN");
                        headerRow.AppendChild(cell82);

                        columns.Add("CarOwnerName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell83 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell83.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell83.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CarOwnerName");
                        headerRow.AppendChild(cell83);

                        columns.Add("VehicleValue");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell84 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell84.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell84.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleValue");
                        headerRow.AppendChild(cell84);

                        columns.Add("ModificationDetails");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell86 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell86.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell86.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ModificationDetails");
                        headerRow.AppendChild(cell86);

                        columns.Add("CompnayName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell87 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell87.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell87.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CompnayName");
                        headerRow.AppendChild(cell87);


                        columns.Add("PriceDetail");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("PriceDetail");
                        headerRow.AppendChild(cell88);



                        columns.Add("Product_benefit");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell89 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell89.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell89.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Product_benefit");
                        headerRow.AppendChild(cell89);

                        columns.Add("AdditionalDriverIdOne");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell90 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell90.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell90.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("AdditionalDriverIdOne");
                        headerRow.AppendChild(cell90);

                        columns.Add("VehicleLimitValue");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell91 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell91.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell91.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("VehicleLimitValue");
                        headerRow.AppendChild(cell91);

                    }

                    sheetData.AppendChild(headerRow);

                    workbook.WorkbookPart.Workbook.Save();

                    foreach (OldQuotationDetails oldQuotation in oldQuotationDetails)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "RequestId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.RequestId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuranceTypeCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.InsuranceTypeCode.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleAgencyRepair")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleAgencyRepair.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ReferenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ReferenceId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CompanyName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CompanyName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CreateDateTime")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CreateDateTime.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ICQuoteReferenceNo")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ICQuoteReferenceNo); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PromotionProgramCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.PromotionProgramCode); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NationalId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NationalId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CardIdTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CardIdTypeId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredBirthDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.BirthDate.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredBirthDateH")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.BirthDateH); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredGenderId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.GenderId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuerdNationalityCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NationalityCode); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CardIdTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CardIdTypeId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredFirstNameAr")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.FirstNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredMiddleNameAr")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.MiddleNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredLastNameAr")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.LastNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredFirstNameEn")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.FirstNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredMiddleNameEn")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.MiddleNameAr); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredLastNameEn")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.LastNameEn); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredSocialStatusId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.SocialStatusId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredOccupationId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.OccupationId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredSocialStatusId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.SocialStatusId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredEducationId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.EducationId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InsuredChildrenBelow16Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ChildrenBelow16Years.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverOccupationCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.OccupationCode); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CardIdTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CardIdTypeId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverOccupationName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.OccupationName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverEnglishFirstName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.EnglishFirstName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverEnglishLastName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.EnglishLastName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverEnglishSecondName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.EnglishSecondName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverEnglishThirdName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.EnglishThirdName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverLastName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.LastName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverSecondName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.SecondName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverFirstName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.FirstName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverThirdName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ThirdName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverSubtribeName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.SubtribeName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CardIdTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CardIdTypeId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverDateOfBirthG")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.DateOfBirthG.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverNationalityCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NationalityCode.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverDateOfBirthH")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.DateOfBirthH.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverNIN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NIN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CardIdTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CardIdTypeId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverIdIssuePlace")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.IdIssuePlace.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CardIdTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CardIdTypeId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverIdExpiryDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.IdExpiryDate.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverResidentOccupation")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ResidentOccupation); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DrivingPercentage")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.DrivingPercentage.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "DriverChildrenBelow16Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ChildrenBelow16Years.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NOALast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NOALast5Years.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NOCLast5Years")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NOCLast5Years.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NCDFreeYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NCDFreeYears.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "NCDReference")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.NCDReference.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "SaudiLicenseHeldYears")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.SaudiLicenseHeldYears.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CityName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CityName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "OccupationName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.OccupationName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "EducationName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.EducationName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "SocialStatusName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.SocialStatusName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CardIdTypeId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CardIdTypeId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PostCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.PostCode); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Licenses")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.Licenses); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Violations")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.Violations); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "SequenceNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.SequenceNumber); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CustomCardNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CustomCardNumber); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Cylinders")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.Cylinders.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "LicenseExpiryDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.LicenseExpiryDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "MajorColor")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.MajorColor); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "MinorColor")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.MinorColor); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ModelYear")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ModelYear?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PlateTypeCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.PlateTypeCode?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "RegisterationPlace")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.RegisterationPlace?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleBodyCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleBodyCode.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleWeight")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleWeight.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleLoad")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleLoad.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleMaker")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleMaker?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleModel")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleModel?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ChassisNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ChassisNumber); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleMakerCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleMakerCode?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleModelCode")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleMakerCode?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarPlateText1")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CarPlateText1); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarPlateText2")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CarPlateText2); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarPlateText3")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CarPlateText3); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarPlateNumber")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CarPlateNumber?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarOwnerNIN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CarOwnerNIN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarOwnerName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CarOwnerName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleValue")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleValue?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "ModificationDetails")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.ModificationDetails); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CompnayName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.CompanyName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "PriceDetail")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.PriceDetail); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Product_benefit")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.Product_benefit); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "AdditionalDriverIdOne")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.AdditionalDriverIdOne); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "VehicleLimitValue")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(oldQuotation.VehicleLimitValue?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }

                }
                workbook.Close();
            }
            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion


        #region
        public byte[] GenearetBlockedUsers(List<BlockedUsersDTO> blockedUsersData)        {            DateTime dt = DateTime.Now.AddDays(-1);            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BlockedUsers" + dt.ToString("dd-MM-yyyy"));            SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";            using (var workbook = SpreadsheetDocument.Create(SPREADSHEET_NAME, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))            {                var workbookPart = workbook.AddWorkbookPart();                {                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);                    workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();                    workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);                    uint sheetId = 1;                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)                    {                        sheetId =                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;                    }                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "PendingBlockedUsers" };                    sheets.Append(sheet);                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();                    List<String> columns = new List<string>();                    columns.Add("NationalId");                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("NationalId");                    headerRow.AppendChild(cell1);

                    columns.Add("CreatedBy");                    DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                    cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                    cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedBy");                    headerRow.AppendChild(cell3);                    columns.Add("CreatedDate");                    DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                    cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                    cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");                    headerRow.AppendChild(cell5);

                    columns.Add("BlockReason");                    DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                    cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                    cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("BlockReason");                    headerRow.AppendChild(cell6);

                    sheetData.AppendChild(headerRow);                    workbook.WorkbookPart.Workbook.Save();                    foreach (BlockedUsersDTO blockedUsersDTO in blockedUsersData)                    {                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();                        foreach (string col in columns)                        {                            if (col == "NationalId")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(blockedUsersDTO.NationalId.ToString()); //
                                newRow.AppendChild(cell10);                            }
                            else if (col == "CreatedBy")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(blockedUsersDTO.CreatedBy.ToString()); //
                                newRow.AppendChild(cell11);                            }                            else if (col == "CreatedDate")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(blockedUsersDTO.CreatedDate.ToString()); //
                                newRow.AppendChild(cell12);                            }                            else if (col == "BlockReason")                            {                                DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();                                cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;                                cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(blockedUsersDTO?.BlockReason); //
                                newRow.AppendChild(cell13);                            }
                        }                        sheetData.AppendChild(newRow);                        workbook.WorkbookPart.Workbook.Save();                    }                    workbook.Close();                }                return GetFileAsByte(SPREADSHEET_NAME);            }        }
        #endregion

        #region Tabby Web Hok Export

        public byte[] GenerateServiceRequestForTabbby(List<TabbyWebHookServiceRequestLogModel> request, string name)
        {
            if (request != null)
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = "Tameenak_Request_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();
                        columns.Add("ReferenceID");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellReferenceID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellReferenceID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellReferenceID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceID");
                        headerRow.AppendChild(cellReferenceID);

                        columns.Add("UserID");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserID");
                        headerRow.AppendChild(cell);

                        columns.Add("UserName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("UserName");
                        headerRow.AppendChild(cell2);

                        columns.Add("Method");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Method");
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

                        columns.Add("ErrorCode");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorCode");
                        headerRow.AppendChild(cell8);

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

                        columns.Add("ServerIP");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServerIP");
                        headerRow.AppendChild(cell12);

                        columns.Add("ServiceResponseTimeInSeconds");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ServiceResponseTimeInSeconds");
                        headerRow.AppendChild(cell14);

                        columns.Add("Channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell15);

                        sheetData.AppendChild(headerRow);
                        foreach (var Request in request)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "ReferenceID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ReferenceId); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "UserID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell1.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell1.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserID); //
                                    newRow.AppendChild(cell1);
                                }
                                else if (col == "UserName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell22 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell22.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell22.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.UserName); //
                                    newRow.AppendChild(cell22);
                                }
                                else if (col == "Method")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell33 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell33.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell33.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Method); //
                                    newRow.AppendChild(cell33);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate?.ToString()); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "CompanyID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell55 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell55.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell55.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyID?.ToString()); //
                                    newRow.AppendChild(cell55);
                                }
                                else if (col == "CompanyName")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell66 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell66.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell66.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyName); //
                                    newRow.AppendChild(cell66);
                                }
                                else if (col == "ErrorCode")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell88 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell88.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell88.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail"); //
                                    newRow.AppendChild(cell88);
                                }
                                else if (col == "ErrorDescription")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                    newRow.AppendChild(cell99);
                                }
                                else if (col == "ServiceRequest")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell100 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell100.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell100.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceRequest); //
                                    newRow.AppendChild(cell100);
                                }
                                else if (col == "ServiceResponse")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell111 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell111.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell111.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponse); //
                                    newRow.AppendChild(cell111);
                                }
                                else if (col == "ServerIP")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell122 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell122.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell122.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServerIP); //
                                    newRow.AppendChild(cell122);
                                }
                                else if (col == "ServiceResponseTimeInSeconds")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell144 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell144.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell144.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponseTimeInSeconds?.ToString()); //
                                    newRow.AppendChild(cell144);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell155 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell155.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell155.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                    newRow.AppendChild(cell155);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #endregion



        #region details Of Drivers Over 5 Policies
        public byte[] GenerateExcelDriverswithPolicyDetails(List<DriverswithPolicyDetails> PolicyDetails)
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OverFivePolicies" + dt.ToString("dd-MM-yyyy"));
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "OverFivePolicies" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    List<String> columns = new List<string>();
                    {
                     
                        columns.Add("referenceId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reference Id");
                        headerRow.AppendChild(cell2);

                        columns.Add("vehicleId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Vehicle Id");
                        headerRow.AppendChild(cell3);

                        columns.Add("isCorporateUser");
                        DocumentFormat.OpenXml.Spreadsheet.Cell corporateCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        corporateCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        corporateCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Corporate User");
                        headerRow.AppendChild(corporateCell);

                        columns.Add("loggedInEmail");
                        DocumentFormat.OpenXml.Spreadsheet.Cell LoggedInEmailCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        LoggedInEmailCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        LoggedInEmailCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Logged In Email");
                        headerRow.AppendChild(LoggedInEmailCell);

                        columns.Add("isCompanyUser");
                        DocumentFormat.OpenXml.Spreadsheet.Cell compnayUserCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        compnayUserCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        compnayUserCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Company User");
                        headerRow.AppendChild(compnayUserCell);

                        columns.Add("CarOwnerName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell carOwnerNameCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        carOwnerNameCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        carOwnerNameCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Car Owner Name");
                        headerRow.AppendChild(carOwnerNameCell);

                        columns.Add("CarOwnerNIN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell carOwnerNINCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        carOwnerNINCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        carOwnerNINCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Car Owner NIN");
                        headerRow.AppendChild(carOwnerNINCell);

                        columns.Add("OwnerTransfer");
                        DocumentFormat.OpenXml.Spreadsheet.Cell ownerTransferCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        ownerTransferCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        ownerTransferCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Owner Transfer");
                        headerRow.AppendChild(ownerTransferCell);

                        columns.Add("nationalId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured Id");
                        headerRow.AppendChild(cell10);

                        columns.Add("insuredNin");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Driver NIN");
                        headerRow.AppendChild(cell5);

                        columns.Add("iBAN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IBAN");
                        headerRow.AppendChild(cell6);

                        columns.Add("email");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Email");
                        headerRow.AppendChild(cell7);

                        columns.Add("phone");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Phone");
                        headerRow.AppendChild(cell8);

                        columns.Add("fullName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Full Name");
                        headerRow.AppendChild(cell9);

                        columns.Add("channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell11);

                        columns.Add("policyIssueDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Issue Date");
                        headerRow.AppendChild(cell12);
                    }

                    sheetData.AppendChild(headerRow);
                    workbook.WorkbookPart.Workbook.Save();
                    foreach (DriverswithPolicyDetails Policy in PolicyDetails)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "referenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.ReferenceId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "vehicleId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.VehicleId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "isCorporateUser")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.IsCorporateUser == 1 ? "True" : "False"); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "loggedInEmail")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.LoggedInEmail); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "isCompanyUser")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.IsCompanyUser == 1 ? "True" : "False"); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarOwnerName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.CarOwnerName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarOwnerNIN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.CarOwnerNIN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "OwnerTransfer")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.OwnerTransfer ? "True" : "False"); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "nationalId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.InsuredId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "insuredNin")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.DriverNin); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "iBAN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.IBAN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "email")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.Email); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "phone")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.Phone); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "fullName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.FullName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "channel")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.Channel); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "policyIssueDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Policy.PolicyIssueDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }
                }
                workbook.Close();
            }
            return GetFileAsByte(SPREADSHEET_NAME);
        }
        #endregion

        #region Policies With Duplcate Data
        public byte[] GenerateExcelRepeatedPolicies(List<PoliciesDuplicationModel> policiesModels, string lang = "ar")
        {
            DateTime dt = DateTime.Now;
            string fileName = "Tameenak_duplicated_policies" + dt.ToString("dd-MM-yyyy");
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            SPREADSHEET_NAME = SPREADSHEET_NAME + ".xlsx";
            Utilities.DeleteExcelSheetFiles();
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    {
                        Id = relationshipId,
                        SheetId = sheetId,
                        Name = "DuplicatedPolicies"
                    };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    {

                        columns.Add("referenceId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell2.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell2.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Reference Id");
                        headerRow.AppendChild(cell2);

                        columns.Add("vehicleId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell3 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell3.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell3.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Vehicle Id");
                        headerRow.AppendChild(cell3);

                        columns.Add("isCorporateUser");
                        DocumentFormat.OpenXml.Spreadsheet.Cell corporateCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        corporateCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        corporateCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Corporate User");
                        headerRow.AppendChild(corporateCell);

                        columns.Add("loggedInEmail");
                        DocumentFormat.OpenXml.Spreadsheet.Cell LoggedInEmailCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        LoggedInEmailCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        LoggedInEmailCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Logged In Email");
                        headerRow.AppendChild(LoggedInEmailCell);

                        columns.Add("isCompanyUser");
                        DocumentFormat.OpenXml.Spreadsheet.Cell compnayUserCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        compnayUserCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        compnayUserCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Company User");
                        headerRow.AppendChild(compnayUserCell);

                        columns.Add("CarOwnerName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell carOwnerNameCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        carOwnerNameCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        carOwnerNameCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Car Owner Name");
                        headerRow.AppendChild(carOwnerNameCell);

                        columns.Add("CarOwnerNIN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell carOwnerNINCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        carOwnerNINCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        carOwnerNINCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Car Owner NIN");
                        headerRow.AppendChild(carOwnerNINCell);

                        columns.Add("OwnerTransfer");
                        DocumentFormat.OpenXml.Spreadsheet.Cell ownerTransferCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        ownerTransferCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        ownerTransferCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Owner Transfer");
                        headerRow.AppendChild(ownerTransferCell);

                        columns.Add("nationalId");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Insured Id");
                        headerRow.AppendChild(cell10);

                        columns.Add("insuredNin");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell5 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell5.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell5.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Driver NIN");
                        headerRow.AppendChild(cell5);

                        columns.Add("iBAN");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell6 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell6.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell6.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("IBAN");
                        headerRow.AppendChild(cell6);

                        columns.Add("email");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell7 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell7.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell7.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Email");
                        headerRow.AppendChild(cell7);

                        columns.Add("phone");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell8 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell8.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell8.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Phone");
                        headerRow.AppendChild(cell8);

                        columns.Add("fullName");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell9 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell9.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell9.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Full Name");
                        headerRow.AppendChild(cell9);

                        columns.Add("channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cell11);

                        columns.Add("policyIssueDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Policy Issue Date");
                        headerRow.AppendChild(cell12);
                    }

                    sheetData.AppendChild(headerRow);
                    foreach (var item in policiesModels)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (string col in columns)
                        {
                            if (col == "referenceId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ReferenceId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "vehicleId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VehicleId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "isCorporateUser")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.IsCorporateUser == 1 ? "True" : "False"); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "loggedInEmail")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.LoggedInEmail); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "isCompanyUser")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.IsCompanyUser == 1 ? "True" : "False"); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarOwnerName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CarOwnerName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "CarOwnerNIN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.CarOwnerNIN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "OwnerTransfer")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.OwnerTransfer ? "True" : "False"); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "nationalId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InsuredId); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "insuredNin")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.DriverNin); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "iBAN")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.IBAN); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "email")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Email); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "phone")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Phone); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "fullName")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.FullName); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "channel")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Channel); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "policyIssueDate")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PolicyIssueDate?.ToString()); //
                                newRow.AppendChild(cell);
                            }
                        }
                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();
                    }
                    workbook.Close();
                }
            }

            return GetFileAsByte(SPREADSHEET_NAME);
        }

        #endregion

        #region App Notifications Export

        public byte[] GenerateAppnotificationsServiceLogExcel(List<FirebaseNotificationLog> request, string name)
        {
            if (request != null)
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string fileName = name;
                if (string.IsNullOrEmpty(name))
                    fileName = "Tameenak_Request_" + dt.ToString("dd-MM-yyyy");

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
                            sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "FailPolicy" };
                        sheets.Append(sheet);

                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        List<String> columns = new List<string>();
                        columns.Add("ReferenceID");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellReferenceID = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellReferenceID.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellReferenceID.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ReferenceID");
                        headerRow.AppendChild(cellReferenceID);

                        columns.Add("CreatedDate");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellCreatedDate = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellCreatedDate.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellCreatedDate.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("CreatedDate");
                        headerRow.AppendChild(cellCreatedDate);

                        columns.Add("ErrorDescription");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellErrorDescription = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellErrorDescription.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellErrorDescription.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("ErrorDescription");
                        headerRow.AppendChild(cellErrorDescription);

                        columns.Add("Channel");
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellChannel = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellChannel.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellChannel.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Channel");
                        headerRow.AppendChild(cellChannel);

                        sheetData.AppendChild(headerRow);
                        foreach (var Request in request)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            foreach (String col in columns)
                            {
                                if (col == "ReferenceID")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell referenceIDCell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    referenceIDCell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    referenceIDCell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ReferenceId); //
                                    newRow.AppendChild(referenceIDCell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell44 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell44.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell44.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CreatedDate?.ToString()); //
                                    newRow.AppendChild(cell44);
                                }
                                else if (col == "ErrorDescription")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell99 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell99.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell99.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                    newRow.AppendChild(cell99);
                                }
                                else if (col == "Channel")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell155 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell155.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell155.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.Channel); //
                                    newRow.AppendChild(cell155);
                                }
                            }
                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();
                        }
                        workbook.Close();
                    }
                }
                return GetFileAsByte(SPREADSHEET_NAME);
            }
            return null;
        }

        #endregion
    }
}
