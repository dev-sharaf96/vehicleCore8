using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Integration.Core.Providers;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Logging;
using Tameenk.Services.Quotation.Components;

namespace Tameenk.Services.QuotationApi.Services
{

    public class QuotationApiService : IQuotationApiService
    {
        #region Fields
        private readonly IRepository<QuotationResponse> _quotationResponseRepository;
        private readonly IRepository<AutomatedTestIntegrationTransaction> _automatedTestIntegrationTransactionRepository;
        private readonly IRepository<QuotationRequest> _quotationRequestRepository;
        private readonly IRequestInitializer _requestInitializer;
        private readonly IInsuranceCompanyService _insuranceCompanyService;
        private readonly IRepository<Benefit> _benefitRepository;
        private readonly IRepository<PriceType> _priceTypeRepository;
        private readonly ILogger _logger;

        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;

        #endregion


        #region Ctor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="quotationResponseRepository">Quoation Response Repository</param>
        /// <param name="quotationRequestRepository">quotation Request Repository</param>
        /// <param name="benefitRepository">benefit Repository</param>
        /// <param name="requestInitializer">request Initializer</param>
        /// <param name="insuranceCompanyService">insurance Company Service</param>
        /// <param name="priceTypeRepository">price Type Repository</param>
        /// <param name="logger"></param>
        /// <param name="tameenkConfig">Tameenk Config</param>
        public QuotationApiService(IRepository<QuotationResponse> quotationResponseRepository,
            IRepository<QuotationRequest> quotationRequestRepository,
            IRepository<AutomatedTestIntegrationTransaction> automatedTestIntegrationTransactionRepository,
            IRepository<Benefit> benefitRepository,
            IRequestInitializer requestInitializer,
            IInsuranceCompanyService insuranceCompanyService,
            IRepository<PriceType> priceTypeRepository
            , ILogger logger,
            TameenkConfig tameenkConfig,
            IRepository<InsuranceCompany> insuranceCompanyRepository
            )
        {
            _quotationResponseRepository = quotationResponseRepository ?? throw new TameenkArgumentNullException(nameof(quotationResponseRepository));
            _quotationRequestRepository = quotationRequestRepository ?? throw new TameenkArgumentNullException(nameof(quotationRequestRepository));
            _requestInitializer = requestInitializer ?? throw new TameenkArgumentNullException(nameof(requestInitializer));
            _insuranceCompanyService = insuranceCompanyService ?? throw new TameenkArgumentNullException(nameof(insuranceCompanyService));
            _benefitRepository = benefitRepository ?? throw new TameenkArgumentNullException(nameof(benefitRepository));
            _priceTypeRepository = priceTypeRepository ?? throw new TameenkArgumentNullException(nameof(priceTypeRepository));
            _logger = logger ?? throw new TameenkArgumentNullException(nameof(ILogger));
            _automatedTestIntegrationTransactionRepository = automatedTestIntegrationTransactionRepository;
            _tameenkConfig = tameenkConfig ?? throw new TameenkArgumentNullException(nameof(TameenkConfig));
            _insuranceCompanyRepository = insuranceCompanyRepository;

        }

        #endregion

        #region Methods

        #region Website Profile APIs

        /// <summary>
        /// Get Quotation Request by user id
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="pageIndx">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        public IPagedList<QuotationRequest> GetQuotationRequestsByUserId(string userId, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var _16HourBeforeNow = DateTime.Now.AddHours(-16);
            var query = _quotationRequestRepository.Table.Where(x => x.UserId == userId)
                .Include(x => x.Vehicle)
                .Include(x => x.Driver)
                .Include(x => x.City)
                .Where(y => DateTime.Compare(y.CreatedDateTime, _16HourBeforeNow) > 0).ToList();
            return new PagedList<QuotationRequest>(query, pageIndx, pageSize);
        }
        #endregion




        /// <summary>
        /// get number of offers fro specific user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        public int GetUserOffersCount(string id)
        {
            return _quotationRequestRepository.Table.Where(x => x.UserId == id).ToList().Where(y => GivenDateWithin16Hours(y.CreatedDateTime)).Count();
        }

        private bool GivenDateWithin16Hours(DateTime givenDate)
        {
            return DateTime.Now.Subtract(givenDate).TotalHours < 16;
        }


        /// <summary>
        /// Save byte data in dll file ( Bin folder ) 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public void SaveFileInBin(string fileName, Byte[] file)
        {
            // File.WriteAllBytes(fileName, file);
            string currentPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            var path = currentPath + "bin\\" + fileName + ".dll";

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);
            }
        }



        public QuotationResult GetQuotation(int insuranceCompanyId, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null, bool automatedTest = false)
        {
            string userId = predefinedLogInfo?.UserID?.ToString();
            QuotationResult quotationResult = new QuotationResult();
            try
            {
                if (insuranceTypeCode == 2 && (insuranceCompanyId == 12 || insuranceCompanyId == 3))
                {
                    var error = new QuotationError
                    {
                        Code = "400",
                        Field = "Comprehensive products is not supported",
                        Message = "Comprehensive products is not supported"
                    };
                    quotationResult.Errors = new List<QuotationError>();
                    quotationResult.Errors.Add(error);
                    return quotationResult;
                }
                if (insuranceTypeCode == 1 || insuranceCompanyId == 12)
                {
                    deductibleValue = null;
                }
                else if (!deductibleValue.HasValue)
                    deductibleValue = (short?)2000;
                var insuranceCompany = _insuranceCompanyService.GetById(insuranceCompanyId);
                Guid selectedUserId = predefinedLogInfo.UserID.HasValue ? predefinedLogInfo.UserID.Value : Guid.Empty;

                if ((insuranceCompany.AllowAnonymousRequest.HasValue && !insuranceCompany.AllowAnonymousRequest.Value) && selectedUserId == Guid.Empty && insuranceTypeCode == 1)
                {
                    quotationResult.QuotationResponse = new QuotationResponse()
                    {
                        CompanyAllowAnonymous = false,
                        AnonymousRequest = true,
                        ReferenceId = getNewReferenceId(),
                        RequestId = 5000,
                        InsuranceTypeCode = short.Parse(insuranceTypeCode.ToString()),
                        VehicleAgencyRepair = vehicleAgencyRepair,
                        DeductibleValue = deductibleValue,
                        CreateDateTime = DateTime.Now,
                        InsuranceCompanyId = insuranceCompanyId
                    };

                    Product temp = new Product();
                    PriceDetail price = new PriceDetail();
                    temp.ProductPrice = 1;
                    temp.InsuranceTypeCode = insuranceTypeCode;
                    temp.ProviderId = insuranceCompanyId;

                    price.PriceTypeCode = 8;
                    price.PriceValue = 12;

                    price.PriceType = new PriceType();
                    price.PriceType.Code = 8;
                    price.PriceType.EnglishDescription = "";
                    price.PriceType.ArabicDescription = "";

                    temp.PriceDetails.Add(price);
                    quotationResult.QuotationResponse.Products.Add(temp);
                    quotationResult.QuotationResponse.AnonymousRequest = true;
                    quotationResult.QuotationResponse.CompanyAllowAnonymous = false;
                    short insuranceCode = 0;
                    short.TryParse(insuranceTypeCode.ToString(), out insuranceCode);
                    quotationResult.QuotationResponse.InsuranceTypeCode = insuranceCode;
                    quotationResult.QuotationResponse.InsuranceCompanyId = insuranceCompanyId;
                    quotationResult.QuotationResponse.CreateDateTime = DateTime.Now;

                    if (insuranceCompany.HasDiscount.HasValue)
                    {
                        //output.QuotationResponse = new QuotationResponse();
                        if (DateTime.Now < insuranceCompany.DiscountEndDate)
                        {
                            insuranceCompany.HasDiscount = true;
                            quotationResult.QuotationResponse.HasDiscount = true;
                            quotationResult.QuotationResponse.DiscountText = insuranceCompany.DiscountText;
                        }
                        else
                        {
                            insuranceCompany.HasDiscount = false;
                        }
                    }
                    return quotationResult;
                }
                //quotationResult.QuotationResponse = GetQuotationResponse(insuranceCompanyId, qtRqstExtrnlId, insuranceTypeCode, vehicleAgencyRepair, deductibleValue);
                //if (quotationResult.QuotationResponse == null)
                //{
                DateTime startDateTime = DateTime.Now;
                string referenceId = getNewReferenceId();
                var quoteRequest = _quotationRequestRepository.Table
                    .Include(request => request.Vehicle)
                    .Include(request => request.Driver)
                    .Include(request => request.Insured)
                    .Include(request => request.Insured.Occupation)
                    .Include(request => request.Drivers.Select(d => d.DriverViolations))
                    .Include(request => request.Driver.Occupation)
                    .Include(e => e.Insured.IdIssueCity)
                    .Include(e => e.Insured.City)
                    .FirstOrDefault(q => q.ExternalId == qtRqstExtrnlId);
                if (quoteRequest == null)
                {
                    var error = new QuotationError
                    {
                        Code = "400",
                        Field = "Quote Request",
                        Message = WebResources.SerivceIsCurrentlyDown
                    };
                    quotationResult.Errors = new List<QuotationError>();
                    quotationResult.Errors.Add(error);
                    return quotationResult;
                }
                if (quoteRequest != null)
                {
                    if (quoteRequest.Driver != null)
                        predefinedLogInfo.DriverNin = quoteRequest.Driver.NIN;
                    if (quoteRequest.Vehicle != null)
                    {
                        if (quoteRequest.Vehicle.VehicleIdType == Tameenk.Core.Domain.Enums.Vehicles.VehicleIdType.CustomCard)
                            predefinedLogInfo.VehicleId = quoteRequest.Vehicle.CustomCardNumber;
                        else
                            predefinedLogInfo.VehicleId = quoteRequest.Vehicle.SequenceNumber;

                    }
                    if (quoteRequest.RequestPolicyEffectiveDate.HasValue && quoteRequest.RequestPolicyEffectiveDate.Value <= DateTime.Now.Date)
                    {
                        quoteRequest.RequestPolicyEffectiveDate = DateTime.Now.Date.AddDays(1);
                        _quotationRequestRepository.Update(quoteRequest);
                    }
                }
                quotationResult.QuotationResponse = new QuotationResponse()
                {
                    ReferenceId = getNewReferenceId(),
                    RequestId = quoteRequest.ID,
                    InsuranceTypeCode = short.Parse(insuranceTypeCode.ToString()),
                    VehicleAgencyRepair = vehicleAgencyRepair,
                    DeductibleValue = deductibleValue,
                    CreateDateTime = startDateTime,
                    InsuranceCompanyId = insuranceCompanyId
                };
                if (insuranceCompany.AllowAnonymousRequest.HasValue && insuranceCompany.AllowAnonymousRequest.Value)
                {
                    quotationResult.QuotationResponse.CompanyAllowAnonymous = true;
                }
                if (selectedUserId != Guid.Empty)
                {
                    quotationResult.QuotationResponse.AnonymousRequest = false;
                }
                var requestMessage = _requestInitializer.GetQuotationRequestData(quoteRequest, quotationResult.QuotationResponse, insuranceTypeCode, vehicleAgencyRepair, userId, deductibleValue);
                var response = RequestQuotationProducts(requestMessage, quotationResult.QuotationResponse, insuranceCompany, predefinedLogInfo, automatedTest);
                if (response != null && response.Errors != null && response.Errors.Count > 0)
                {
                    quotationResult.Errors = response.Errors.Select(er => new QuotationError
                    {
                        Code = er.Code,
                        Field = er.Field,
                        Message = er.Message
                    }).ToList();
                    return quotationResult;
                }
                if (response != null && response.Products != null)
                {
                    var products = new List<Product>();
                    // Load the benefit details Price Types  from database.
                    var allBenefitst = _benefitRepository.Table.ToList();
                    var allPriceTypes = _priceTypeRepository.Table.ToList();

                    foreach (var p in response.Products)
                    {
                        var product = p.ToEntity();
                        if (requestMessage != null && !string.IsNullOrEmpty(requestMessage.PromoCode))
                            product.IsPromoted = true;

                        product.ProviderId = insuranceCompanyId;
                        if (!product.InsuranceTypeCode.HasValue || product.InsuranceTypeCode.Value < 1)
                            product.InsuranceTypeCode = insuranceTypeCode;

                        if (product.Product_Benefits != null)
                            foreach (var pb in product.Product_Benefits)
                                pb.Benefit = allBenefitst.FirstOrDefault(bf => pb.BenefitId.HasValue && bf.Code == pb.BenefitId.Value);

                        // Load price details from database.
                        foreach (var pd in product.PriceDetails)
                            pd.PriceType = allPriceTypes.FirstOrDefault(pt => pt.Code == pd.PriceTypeCode);

                        product.QuotaionNo = response.QuotationNo;
                        products.Add(product);
                    }
                    quotationResult.QuotationResponse.Products = products;
                    _quotationResponseRepository.Insert(quotationResult.QuotationResponse);
                }
                //}
                if (insuranceTypeCode == 2 & insuranceCompany.Key == "Malath")
                {
                    quotationResult.QuotationResponse.Products = FilterOutProductsForMalath(quotationResult.QuotationResponse.Products.ToList(), deductibleValue ?? 2000);
                }
                else if (insuranceTypeCode == 2)
                {
                    quotationResult.QuotationResponse.Products = FilterOutProducts(quotationResult.QuotationResponse.Products.ToList(), deductibleValue ?? 2000);
                }

                quotationResult.QuotationResponse.Products = ExcludeProductOrBenefitWithZeroPrice(quotationResult.QuotationResponse.Products).ToList();
                if (insuranceCompany.ShowQuotationToUser.HasValue && !insuranceCompany.ShowQuotationToUser.Value)
                {
                    quotationResult.QuotationResponse.Products = null;
                }

                if (insuranceCompany.HasDiscount.HasValue && quotationResult.QuotationResponse.Products != null)
                {
                    //output.QuotationResponse = new QuotationResponse();
                    if (insuranceCompany.DiscountEndDate.HasValue && DateTime.Now < insuranceCompany.DiscountEndDate)
                    {
                        insuranceCompany.HasDiscount = true;
                        quotationResult.QuotationResponse.HasDiscount = true;
                        quotationResult.QuotationResponse.DiscountText = insuranceCompany.DiscountText;
                    }
                    else
                    {
                        quotationResult.QuotationResponse.HasDiscount = false;
                        insuranceCompany.HasDiscount = false;
                    }
                }
                else
                {
                    quotationResult.QuotationResponse.HasDiscount = false;
                }
            }
            catch (Exception exp)
            {
                _logger.Log($"GetQuotation ", exp);

            }
            return quotationResult;
        }
        



        private QuotationResponse GetQuotationResponse(int insuranceCompanyId, string qtRqstExtrnlId, int insuranceTypeCode, bool vehicleAgencyRepair, short? deductibleValue)
        {
            var _16HoursBeforeNow = DateTime.Now.AddHours(-16);
            return _quotationResponseRepository.Table
                .Include(qr => qr.QuotationRequest)
                        .Include(qr => qr.Products)
                        .Include(qr => qr.Products.Select(p => p.PriceDetails.Select(pd => pd.PriceType)))
                        .Include(qr => qr.Products.Select(p => p.Product_Benefits))
                        .Include(qr => qr.Products.Select(p => p.Product_Benefits.Select(pb => pb.Benefit)))
                        .Include(qr => qr.QuotationRequest.Vehicle)
                        .Include(qr => qr.QuotationRequest.Driver)
                        .Where(
                        x => x.InsuranceCompanyId == insuranceCompanyId && x.QuotationRequest.ExternalId == qtRqstExtrnlId &&
                        x.InsuranceTypeCode == insuranceTypeCode &&
                        (
                            (x.VehicleAgencyRepair.HasValue && x.VehicleAgencyRepair.Value == vehicleAgencyRepair) ||
                            (!vehicleAgencyRepair && !x.VehicleAgencyRepair.HasValue)
                        ) &&
                        (
                            (!deductibleValue.HasValue && !x.DeductibleValue.HasValue) ||
                            (deductibleValue.HasValue && x.DeductibleValue.HasValue && x.DeductibleValue.Value == deductibleValue.Value)
                        ))
                        .FirstOrDefault(y => (_16HoursBeforeNow < y.CreateDateTime));
        }

        //private bool IsGivenDateWithin16Hours(DateTime givenDate)
        //{
        //    return DateTime.Now.Subtract(givenDate).TotalHours < 16;
        //}

        private string getNewReferenceId()
        {
            string referenceId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15);
            if (_quotationResponseRepository.Table.Any(q => q.ReferenceId == referenceId))
                return getNewReferenceId();

            return referenceId;
        }


        private QuotationServiceResponse RequestQuotationProducts(QuotationServiceRequest requestMessage, QuotationResponse quotationResponse, InsuranceCompany insuranceCompany, ServiceRequestLog predefinedLogInfo, bool automatedTest)
        {
            requestMessage.InsuranceCompanyCode = insuranceCompany.InsuranceCompanyID;
            var providerFullTypeName = string.Empty;
            providerFullTypeName = insuranceCompany.ClassTypeName + ", " + insuranceCompany.NamespaceTypeName;

            QuotationServiceResponse results = null;
            IInsuranceProvider provider = null;
            object instance = Utilities.GetValueFromCache("instance_" + providerFullTypeName+ quotationResponse.InsuranceTypeCode);
            if (instance != null && insuranceCompany.Key != "Tawuniya")
            {
                provider = instance as IInsuranceProvider;
            }
            if (instance == null)
            {
                var scope = EngineContext.Current.ContainerManager.Scope();
                var providerType = Type.GetType(providerFullTypeName);

                if (providerType != null)
                {
                    if (!EngineContext.Current.ContainerManager.TryResolve(providerType, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(providerType, scope);
                    }
                    provider = instance as IInsuranceProvider;
                }
                if (provider == null)
                {
                    throw new Exception("Unable to find provider.");
                }
                if (insuranceCompany.Key != "Tawuniya")
                    Utilities.AddValueToCache("instance_" + providerFullTypeName+ quotationResponse.InsuranceTypeCode, instance, 1440);

                if (provider != null)
                {
                    results = provider.GetQuotation(requestMessage, predefinedLogInfo, automatedTest);
                }
                scope.Dispose();
            }
            else
            {
                if (provider != null)
                {
                    results = provider.GetQuotation(requestMessage, predefinedLogInfo, automatedTest);
                }
            }
            // Remove products if price is zero
            if (results != null && results.Products != null)
            {

                results.Products = results.Products.Where(e => e.ProductPrice > 0).ToList();

                var showZeroPremium = _tameenkConfig.Quotatoin.showZeroPremium;

                if (showZeroPremium)
                {
                    // Remove products if basic perineum equal zero
                    results.Products = results.Products.Where(e => e.PriceDetails.Any(p => p.PriceTypeCode == 7 && p.PriceValue > 0)).ToList();

                }
                // Remove benefits if price is zero
                foreach (var prod in results.Products)
                {
                    if (prod.Benefits != null && prod.Benefits.Count() > 0)
                    {
                        prod.Benefits = prod.Benefits.Where(e => e.BenefitPrice > 0 || (e.IsReadOnly && e.IsSelected == true)).ToList();
                    }
                }
            }

            return results;
        }

        private List<Product> FilterOutProducts(List<Product> products, short deductibleValue)
        {
            List<Product> filteredProducts = new List<Product>();
            Product product = null;
            int delta = int.MaxValue;
            foreach (Product p in products)
            {
                if (p.DeductableValue == deductibleValue)
                    filteredProducts.Add(p);
                if (Math.Abs(deductibleValue - p.DeductableValue.GetValueOrDefault()) < delta)
                {
                    delta = Math.Abs(deductibleValue - p.DeductableValue.GetValueOrDefault());
                    product = p;
                }
            }

            if (filteredProducts.Count == 0 && product != null)
            {

                if (!filteredProducts.Contains(product))
                    filteredProducts.Add(product);

            }

            foreach (Product p in products)
            {
                if (p.DeductableValue == 0 && !filteredProducts.Contains(p))
                    filteredProducts.Add(p);
            }

            return filteredProducts;
        }
        private List<Product> FilterOutProductsForMalath(List<Product> products, short deductibleValue)
        {
            List<Product> filteredProducts = new List<Product>();
            Product product = null;
            int delta = int.MaxValue;
            foreach (Product p in products)
            {
                if (p.DeductableValue == deductibleValue)
                    filteredProducts.Add(p);
                if (Math.Abs(deductibleValue - p.DeductableValue.GetValueOrDefault()) < delta)
                {
                    if (!filteredProducts.Contains(p))
                        filteredProducts.Add(p);
                }
            }
            foreach (Product p in products)
            {
                if (p.DeductableValue == 0 && !filteredProducts.Contains(p))
                    filteredProducts.Add(p);
            }

            return filteredProducts;
        }

        private IEnumerable<Product> ExcludeProductOrBenefitWithZeroPrice(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                var productBenefits = new List<Product_Benefit>();
                productBenefits.AddRange(product.Product_Benefits.Where(x => x.BenefitPrice > 0 || (x.IsReadOnly && x.IsSelected.HasValue && x.IsSelected == true)));
                product.Product_Benefits = productBenefits;
            }

            return products.Where(x => x.ProductPrice > 0);
        }

        public string ExportAutomatedTestResultToExcel(bool Quotation)
        {

            var dataToInsertInExcel = _automatedTestIntegrationTransactionRepository
                .Table.Where(x => x.Message.StartsWith(Quotation ? "Quotation" : "Policy") && !x.Retrieved)
                .ToList();

            dataToInsertInExcel.AsParallel().ForAll(x => x.Retrieved = true);

            _automatedTestIntegrationTransactionRepository.Update(dataToInsertInExcel);

            DateTime dt = DateTime.Now;
            string SPREADSHEET_NAME = null;

            if (Quotation)
                SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"QuotationAutomatedTest-{DateTime.Now.Ticks}-{dt.ToString("dd-MM-yyyy")}");
            else
                SPREADSHEET_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"PolicyAutomatedTest-{DateTime.Now.Ticks}-{dt.ToString("dd-MM-yyyy")}");

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

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = "CompamniesAutomatedTest" };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<string> columns = new List<string>();
                    List<string> propNames = typeof(AutomatedTestIntegrationTransaction).GetProperties()
                        .Where(x => !string.Equals(x.Name, "Retrieved") && !string.Equals(x.Name, "Id") && !string.Equals(x.Name, "Date"))
                        .Select(x => x.Name).ToList();
                    propNames.Add("Status");

                    foreach (var prop in propNames)
                    {
                        columns.Add(prop);
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(prop);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (var item in dataToInsertInExcel)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        foreach (String col in columns)
                        {
                            if (col == "Message")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Message); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "InputParams")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InputParams); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "OutputParams")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.InputParams); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "StatusId")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.StatusId.ToString()); //
                                newRow.AppendChild(cell);
                            }
                            else if (col == "Status")
                            {
                                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                                cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.StatusId == 0 ? "Success" : "Failed"); //
                                //cell.StyleIndex = item.StatusId == 0 ? 1U : 2U;
                                newRow.AppendChild(cell);
                            }
                        }

                        sheetData.AppendChild(newRow);
                        workbook.WorkbookPart.Workbook.Save();

                    }
                }

                workbook.Close();
            }
            return SPREADSHEET_NAME;
        }
        private List<Benefit> GetBenfits()
        {
            var allbenefits= Utilities.GetValueFromCache("ALL__bENFITS___");
            if (allbenefits == null)
            {
                var benefits = _benefitRepository.TableNoTracking.ToList();
                Utilities.AddValueToCache("ALL__bENFITS___", benefits, 1440);
                return benefits;
            }
            else
            {
               return (List<Benefit>)allbenefits;
            }
        }
        private List<PriceType> GetPriceTypes()
        {
            var pricetypes = Utilities.GetValueFromCache("ALL__pRICE__tYPES____");
            if (pricetypes == null)
            {
                var types = _priceTypeRepository.TableNoTracking.ToList();
                Utilities.AddValueToCache("ALL__pRICE__tYPES____", types, 1440);
                return types;
            }
            else
            {
                return (List<PriceType>)pricetypes;
            }
        }

        #endregion
    }
}
