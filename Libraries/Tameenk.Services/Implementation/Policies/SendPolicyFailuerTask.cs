using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Notifications.Models;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;
using Tameenk.Services.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class SendPolicyFailuerTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IHttpClient _httpClient;
        private readonly INotificationService _notificationService;
        #endregion

        #region Ctor
        public SendPolicyFailuerTask(IPolicyProcessingService policyProcessingService,
            IHttpClient httpClient,
            ILogger logger,
            IRepository<InsuranceCompany> insuranceCompanyRepository,
            IRepository<QuotationResponse> quotationResponseRepository,
            INotificationService notificationService,
            TameenkConfig config,
            IRepository<ScheduleTask> scheduleTaskRepository)
        {
            _logger = logger;
            _config = config;
            _policyProcessingService = policyProcessingService;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _quotationResponseRepository = quotationResponseRepository;
            _httpClient = httpClient;
            _scheduleTaskRepository = scheduleTaskRepository;
            _notificationService = notificationService;
        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            //int maxTries = 1;
            var failedPoliciesQueue = _policyProcessingService.GetFailedPolicyFromProcessingQueue(null, null, true, true, maxTrials);
            if(failedPoliciesQueue != null)
            { 
            var failedCheckoutReferenceIds=  failedPoliciesQueue.Select(x => x.ReferenceId).ToList();
            var failedPolicies = _policyProcessingService.GetFailedPolicyItems(failedCheckoutReferenceIds);

            var accessToken = GetAccessToken();
            DateTime dt = DateTime.Now;
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

                        DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "QuotationRequest" };
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



                        }

                        sheetData.AppendChild(headerRow);
                        //  logging.LogDebug("Entries Count " + serviceRequestLogs.Count.ToString());



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
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.Driver?.FullArabicName); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "VehicleId")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(!string.IsNullOrEmpty(policyItem.Vehicle?.CustomCardNumber)? policyItem.Vehicle?.CustomCardNumber : policyItem.Vehicle?.SequenceNumber); //
                                    newRow.AppendChild(cell);
                                }
                                else if (col == "CreatedDate")
                                {
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.CreatedDateTime.ToString()); //
                                    newRow.AppendChild(cell);
                                }
                                //else if (col == "CompanyID")
                                //{
                                //    // Loza
                                //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem..ToString()); //
                                //    newRow.AppendChild(cell);
                                //}
                                //else if (col == "CompanyName")
                                //{
                                //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.CompanyName); //
                                //    newRow.AppendChild(cell);
                                //}
                                //else if (col == "ErrorCode")
                                //{
                                //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorCode == 1 ? "Success" : "Fail"); //
                                //    newRow.AppendChild(cell);
                                //}

                                //else if (col == "ErrorDescription")
                                //{
                                //    /// Loza
                                //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ErrorDescription); //
                                //    newRow.AppendChild(cell);
                                //}
                                //else if (col == "ServiceRequest")
                                //{
                                //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceRequest); //
                                //    newRow.AppendChild(cell);
                                //}
                                //else if (col == "ServiceResponse")
                                //{
                                //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(Request.ServiceResponse); //
                                //    newRow.AppendChild(cell);
                                //}


                                else if (col == "ReferenceId")
                                {
                                    // Loza
                                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem.ReferenceId); //
                                    newRow.AppendChild(cell);
                                }
                                //else if (col == "InvoiceNo")
                                //{
                                //    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                //    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                //    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(policyItem..ToString()); //
                                //    newRow.AppendChild(cell);
                                //}
                            }

                            sheetData.AppendChild(newRow);
                            workbook.WorkbookPart.Workbook.Save();

                        }
                        workbook.Close();
                    }

                }


              
                if (File.Exists(SPREADSHEET_NAME))
                {
                    FileStream file = new FileStream(SPREADSHEET_NAME, FileMode.Open);
                    var attachment = new EmailAttacmentFileModel();
                    var listAttachements = new List<EmailAttacmentFileModel>();
                    

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.CopyTo(ms);

                        attachment.ContentType = new ContentType(MediaTypeNames.Application.Octet);
                        attachment.FileAsByteArrayDetails = new FileAsByteArrayDetails();
                        attachment.FileAsByteArrayDetails.FileAsByterArray = ms.ToArray();
                        attachment.FileAsByteArrayDetails.FileName = fileName;

                    }
                    listAttachements.Add(attachment);
                    file.Dispose();
                    file.Close();
                    await _notificationService.SendEmailAsync("ahmed.s@bcare.com.sa", "Failed Policies Report", " please check the attached report for failed policies.", listAttachements);
                }
            }
            
        }


        private void SendFailurePolicyGenerateEmail(string referenceId, int sendingThreshold, string commonPolicyFailureRecipient)
        {
            var InsuranceCompany = _quotationResponseRepository.Table
                .Include(x => x.InsuranceCompany)
                .SingleOrDefault(x => x.ReferenceId.ToUpper() == referenceId.ToUpper())
                .InsuranceCompany;

            var FailureRecipients = InsuranceCompany.PolicyFailureRecipient.Split(',').Concat(commonPolicyFailureRecipient.Split(','));

            string EmailSubject = $"{WebResources.PolicyGenerationFailureEmailSubject}";

            string EmailBody = $"{WebResources.PolicyGenerationFailureEmailBody1} {referenceId} {WebResources.PolicyGenerationFailureEmailBody2} {InsuranceCompany.NameAR}";

            _notificationService.SendEmailAsync(FailureRecipients, EmailSubject, EmailBody);

        }
        #endregion

        private string GetAccessToken()
        {
            try
            {
                var formParamters = new Dictionary<string, string>();
                formParamters.Add("grant_type", "client_credentials");
                formParamters.Add("client_Id", _config.Identity.ClientId);
                formParamters.Add("client_secret", _config.Identity.ClientSecret);

                var content = new FormUrlEncodedContent(formParamters);
                var postTask = _httpClient.PostAsync($"{_config.Identity.Url}token", content);
                postTask.ConfigureAwait(false);
                postTask.Wait();
                var response = postTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AccessTokenResult>(jsonString);
                    return result.access_token;
                }
                return "";

            }
            catch (Exception ex)
            {
                var logId = DateTime.Now.GetTimestamp();
                _logger.Log($"PolicyProcessingTask -> GetAccessToken [key={logId}]", ex);
                return "";
            }
        }
        public class AccessTokenResult
        {
            [JsonProperty("access_token")]
            public string access_token { get; set; }
            [JsonProperty("expires_in")]
            public int expires_in { get; set; }


        }
    }
}
