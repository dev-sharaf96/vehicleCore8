using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL;

namespace TestQuotationReports
{
    public class QuotationContext
    {

        private static List<ServiceRequestLog> GetListOfQuotationSerive()
        {
            return ServiceRequestLogDataAccess.GetQuotationList(0);
        }
        public static void GenerateQuotationRequestReport()
        {

            List<ServiceRequestLog> serviceRequestLogs = QuotationContext.GetListOfQuotationSerive();


            DateTime dt = DateTime.Now.AddDays(-1);
            string SPREADSHEET_NAME = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QuotationRequest" + dt.ToString("dd-MM-yyyy"));
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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "QuotationRequest" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();

                    {
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



                    }

                    sheetData.AppendChild(headerRow);
                    //  logging.LogDebug("Entries Count " + serviceRequestLogs.Count.ToString());

                    decimal sum = 0;

                    int success = 0;
                    int fail = 0;

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


                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

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
                        cell32.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number Of Success");
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
                        cell25.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number Of Success Request");
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
                        cell85.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Number Of Fail Request");
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


        }

    }
}
    
