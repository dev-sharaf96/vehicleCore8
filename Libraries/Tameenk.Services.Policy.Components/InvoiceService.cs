using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;


namespace Tameenk.Services.Policy.Components
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IQuotationService _quotationService;
        private readonly IPolicyService _policyService;
        private readonly IRepository<Invoice_Benefit> _invoiceBenefitRepository;
        private readonly TameenkConfig _tameenkConfig;

        private readonly ILogger _logger;
        private readonly IRepository<InvoiceFileTemplates> _invoiceFileTemplatesRepository;
        private readonly IRepository<Tameenk.Core.Domain.Entities.Policy> _policyRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailRepository;
        public InvoiceService(IRepository<Invoice> invoiceRepository
            , IRepository<InsuranceCompany> insuranceCompanyRepository
            , IQuotationService quotationService
            , ILogger logger, IPolicyService policyService, IRepository<Invoice_Benefit> invoiceBenefitRepository,
            TameenkConfig tameenkConfig, IRepository<InvoiceFileTemplates> invoiceFileTemplatesRepository,
            IRepository<Tameenk.Core.Domain.Entities.Policy> policyRepository, IRepository<CheckoutDetail> checkoutDetailRepository)
        {
            _invoiceRepository = invoiceRepository;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _quotationService = quotationService;
            _logger = logger;
            _policyService = policyService;
            _invoiceBenefitRepository = invoiceBenefitRepository;
            _tameenkConfig = tameenkConfig;
            _invoiceFileTemplatesRepository= invoiceFileTemplatesRepository;
            _policyRepository = policyRepository;
            _checkoutDetailRepository = checkoutDetailRepository;
        }

        public Invoice GenerateAndSaveInvoicePdf(PolicyRequest policyRequestMessage, PolicyResponse policy,
            string serverIP, string companyNameAr, string companyNameEn, string companyKey,
            Insured insured, string vat,Vehicle vehicle, bool isAutoleasingPolicy, out byte[] invoiceFileAsByteArray,out string exception)
        {
            exception = string.Empty;
            try
            {
                int paymentBillNumber = int.Parse(policyRequestMessage.PaymentBillNumber);
                var invoice = _invoiceRepository.Table.Include(x => x.InvoiceFile)
                    .Where(i => i.InvoiceNo == paymentBillNumber).FirstOrDefault();

                var templateInfo = _invoiceFileTemplatesRepository.TableNoTracking.Where(a => a.Id == invoice.TemplateId).FirstOrDefault();
                if (templateInfo == null)
                {
                    exception += "no Template exists with id "+invoice.TemplateId;
                    invoiceFileAsByteArray = null;
                    return null;
                }

                string insuranceTypeEn = string.Empty;
                string insuranceTypeAr = string.Empty;

                if (invoice.InsuranceTypeCode == 2)
                {
                    if (companyKey == "ACIG")
                    {
                        insuranceTypeEn = "ACIG Plus Insurance";
                        insuranceTypeAr = "تأمين أسيج المميز";
                    }
                    else if (companyKey == "UCA")
                    {
                        insuranceTypeEn = "TPL&OD";
                        insuranceTypeAr = "تأمين الطرف الثالث والاضرار الخاصة للسيارات الخاصة";
                    }
                    else if (companyKey == "TUIC")
                    {
                        insuranceTypeEn = "Aman ALETIHAD";
                        insuranceTypeAr = "أمان الاتحاد";
                    }
                    //else if (companyKey == "TokioMarine")
                    //{
                    //    insuranceTypeEn = "Retail Motor Insurance";
                    //    insuranceTypeAr = "تأمين المركبات للأفراد";
                    //}
                    else
                    {
                        insuranceTypeEn = "Comprehensive";
                        insuranceTypeAr = "تأمين شامل";
                    }
                }
                else if (invoice.InsuranceTypeCode == 7)
                {
                    insuranceTypeEn = "Sanad Plus";
                    insuranceTypeAr = "سند بلس";
                }
                else if (invoice.InsuranceTypeCode == 8)
                {
                    insuranceTypeEn = "Wafi Smart";
                    insuranceTypeAr = "وافي سمارت";
                }
                else if (invoice.InsuranceTypeCode == 13)
                {
                    insuranceTypeEn = "Motor Plus";
                    insuranceTypeAr = "موتر بلس";
                }
                else
                {
                    insuranceTypeEn = "Third Party Liability";
                    insuranceTypeAr = "تأمين ضد الغير";
                }

                string clientFullName = insured.FirstNameEn;
                if (!string.IsNullOrEmpty(insured.MiddleNameEn))
                    clientFullName = clientFullName + " " + insured.MiddleNameEn;
                if (!string.IsNullOrEmpty(insured.LastNameEn))
                    clientFullName = clientFullName + " " + insured.LastNameEn;


                string clientFullNameArabic = insured.FirstNameAr;
                if (!string.IsNullOrEmpty(insured.MiddleNameAr))
                    clientFullNameArabic = clientFullNameArabic + " " + insured.MiddleNameAr;
                if (!string.IsNullOrEmpty(insured.LastNameAr))
                    clientFullNameArabic = clientFullNameArabic + " " + insured.LastNameAr;

                if (policy != null && policy.PolicyExpiryDate == null && policy.PolicyEffectiveDate != null)
                {
                    policy.PolicyExpiryDate = policy.PolicyEffectiveDate.Value.AddYears(1).AddDays(-1);
                }

                decimal bcareDiscount = 0;
                if (invoice.TotalBCareDiscount.HasValue && invoice.TotalBCareDiscount.Value > 0)
                    bcareDiscount = invoice.TotalBCareDiscount.Value;

                var benfits=_invoiceBenefitRepository.TableNoTracking.Include(x => x.Benefit).Where(a => a.InvoiceId == invoice.Id).ToList();
                if (benfits.Count() > 0)
                {
                    exception = " benfits.Count() " + benfits.Count() + " id is " + invoice.Id+"  --";
                    var invoiceBenefits = from invoiceBenefit in benfits.AsEnumerable()
                                          select new
                                          {
                                              DescriptionEnglish = invoiceBenefit?.Benefit.EnglishDescription,
                                              DescriptionArabic = invoiceBenefit?.Benefit.ArabicDescription,
                                              Price = invoiceBenefit.BenefitPrice
                                          };
                    var invoiceDataObj = new
                    {
                        InvoiceNo = invoice.InvoiceNo,
                        ClientName = clientFullName,
                        ClientNameArabic = clientFullNameArabic,
                        Email = policyRequestMessage.InsuredEmail,
                        PhoneNumber = policyRequestMessage.InsuredMobileNumber,
                        InvoiceDate = invoice.InvoiceDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                        InvoiceDueDate = invoice.InvoiceDueDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                        ReferenceNumber = policyRequestMessage.ReferenceId,
                        InsuranceTypeAr = insuranceTypeAr,
                        InsuranceTypeEn = insuranceTypeEn,
                        InsurerNameAr = companyNameAr,
                        InsurerNameEn = companyNameEn,
                        VATNumber = vat,
                        PolicyNo = policy == null ? "" : policy.PolicyNo,
                        PolicyEffectiveDate = policy == null ? ""
                            : (policy.PolicyEffectiveDate.HasValue ? policy.PolicyEffectiveDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ""),
                        PolicyExpiryDate = policy == null ? ""
                            : (policy.PolicyExpiryDate.HasValue ? policy.PolicyExpiryDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ""),
                        PolicyIssueDate = policy == null ? ""
                            : (policy.PolicyIssuanceDate.HasValue ? policy.PolicyIssuanceDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ""),
                        ProductPrice = invoice.ProductPrice,
                        Fees = invoice.Fees,
                        Vat = invoice.Vat,
                        TotalVat = invoice.Vat,
                        ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                        Discount = invoice.Discount,
                        TotalDiscount = invoice.TotalDiscount + bcareDiscount,
                        SubTotalPrice = invoice.SubTotalPrice,
                        TotalPrice = invoice.TotalPrice,
                        Benefits = invoiceBenefits.ToArray(),
                        VehiclePlateNoAr = vehicle.CarPlateNumber,
                        VehiclePlateText = vehicle.CarPlateText1 + " " + vehicle.CarPlateText2 + " " + vehicle.CarPlateText3,
                        VehicleMakeEn = vehicle.VehicleMaker,
                        VehicleModelEn= vehicle.VehicleModel,
                        VehicleYear=vehicle.ModelYear,
                        BenefitPrice = invoice.TotalBenefitPrice,
                        AdminFees=invoice.Fees
                    };
                    invoiceFileAsByteArray = GenerateInvoiceFileFromPolicyData(invoiceDataObj, templateInfo.TemplateFilePath);
                }
                else
                {
                    var invoiceDataObj = new
                    {
                        InvoiceNo = invoice.InvoiceNo,
                        ClientName = clientFullName,
                        ClientNameArabic = clientFullNameArabic,
                        Email = policyRequestMessage.InsuredEmail,
                        PhoneNumber = policyRequestMessage.InsuredMobileNumber,
                        InvoiceDate = invoice.InvoiceDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                        InvoiceDueDate = invoice.InvoiceDueDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                        ReferenceNumber = policyRequestMessage.ReferenceId,
                        InsuranceTypeAr = insuranceTypeAr,
                        InsuranceTypeEn = insuranceTypeEn,
                        InsurerNameAr = companyNameAr,
                        InsurerNameEn = companyNameEn,
                        VATNumber = vat,
                        PolicyNo = policy == null ? "" : policy.PolicyNo,
                        PolicyEffectiveDate = policy == null ? ""
                            : (policy.PolicyEffectiveDate.HasValue ? policy.PolicyEffectiveDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ""),
                        PolicyExpiryDate = policy == null ? ""
                            : (policy.PolicyExpiryDate.HasValue ? policy.PolicyExpiryDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ""),
                        PolicyIssueDate = policy == null ? ""
                            : (policy.PolicyIssuanceDate.HasValue ? policy.PolicyIssuanceDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : ""),
                        ProductPrice = invoice.ProductPrice,
                        Fees = invoice.Fees,
                        Vat = invoice.Vat,
                        TotalVat = invoice.Vat,
                        ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                        Discount = invoice.Discount,
                        TotalDiscount = invoice.TotalDiscount,
                        SubTotalPrice = invoice.SubTotalPrice,
                        TotalPrice = invoice.TotalPrice,
                        VehiclePlateNoAr = vehicle.CarPlateNumber,
                        VehiclePlateText = vehicle.CarPlateText1 +" "+ vehicle.CarPlateText2+" " + vehicle.CarPlateText3,
                        VehicleMakeEn = vehicle.VehicleMaker,
                        VehicleModelEn = vehicle.VehicleModel,
                        VehicleYear = vehicle.ModelYear,
                        BenefitPrice = 0,
                        AdminFees = invoice.Fees
                    };
                    invoiceFileAsByteArray = GenerateInvoiceFileFromPolicyData(invoiceDataObj, templateInfo.TemplateFilePath);
                }

                string filePath = Utilities.SavePdfFile(policyRequestMessage.ReferenceId, invoiceFileAsByteArray, companyKey, false, _tameenkConfig.RemoteServerInfo.UseNetworkDownload, _tameenkConfig.RemoteServerInfo.DomainName, _tameenkConfig.RemoteServerInfo.ServerIP, _tameenkConfig.RemoteServerInfo.ServerUserName, _tameenkConfig.RemoteServerInfo.ServerPassword,invoice.InvoiceDate, isAutoleasingPolicy, out exception);
                if (string.IsNullOrEmpty(filePath))
                {
                    exception = "Failed to save pdf file on server due to "+exception;
                    invoiceFileAsByteArray = null;
                    return null;
                }

               // string filePath = Utilities.SaveCompanyFile(policyRequestMessage.ReferenceId, invoiceFileAsByteArray, companyKey, false);
                if (invoice.InvoiceFile == null)
                    invoice.InvoiceFile = new InvoiceFile() { Id = invoice.Id, FilePath=filePath,ServerIP=serverIP, TemplateId = templateInfo.Id,CreatedDate=DateTime.Now,ModifiedDate=DateTime.Now };
                else
                {
                    invoice.InvoiceFile.FilePath = filePath;
                    invoice.InvoiceFile.ServerIP = serverIP;
                    invoice.InvoiceFile.TemplateId = templateInfo.Id;
                    invoice.InvoiceFile.ModifiedDate = DateTime.Now;
                }
                _invoiceRepository.Update(invoice);
                return invoice;
            }
            catch (Exception ex)
            {
                exception += ex.ToString();
                ErrorLogger.LogError(ex.Message, ex, false);
                invoiceFileAsByteArray = null;
                return null;
            }
        }

        public byte[] GenerateInvoiceFileFromPolicyData(object invoiceData,string templatePath)
        {
            try
            {
                string invoiceDataJsonString = JsonConvert.SerializeObject(invoiceData);
                ReportGenerationModel reportGenerationModel = new ReportGenerationModel
                {
                    ReportType = "TameenkInvoice",
                    TemplatePath=templatePath,
                    ReportDataAsJsonString = invoiceDataJsonString
                };
                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(System.Configuration.ConfigurationManager.AppSettings["PolicyPDFGeneratorAPIURL"] + "api/generatepdftemplate", httpContent).GetAwaiter().GetResult();
                var pdfGeneratorResult = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {

                    return JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResult);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public bool GenerateInvoicePdf(string referenceId, out string exception)
        {
            exception = string.Empty;
            try
            {
                bool isAutoleasingPolicy = false;
                var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(i => i.ReferenceId == referenceId);
                if (invoice != null)
                {
                    var quotation = _quotationService.GetInvoiceDataFromQuotationResponseByReferenceId(invoice.ReferenceId);
                    if (quotation != null)
                    {
                        var policy = _policyRepository.TableNoTracking.FirstOrDefault(a => a.CheckOutDetailsId == referenceId);
                        if (policy == null)
                        {
                            var checkout = _checkoutDetailRepository.TableNoTracking.FirstOrDefault(a => a.ReferenceId == referenceId && a.PolicyStatusId != 1 && a.PolicyStatusId != 3 && a.IsCancelled == false);
                            if (checkout != null)
                            {
                                isAutoleasingPolicy = checkout.Channel.ToString().ToLower() == Channel.autoleasing.ToString().ToLower() ? true : false;
                                policy = new Tameenk.Core.Domain.Entities.Policy();
                                policy.CheckoutDetail = new CheckoutDetail();
                                policy.CheckoutDetail.Email = checkout.Email;
                                policy.CheckoutDetail.Phone = checkout.Phone;
                                policy.PolicyNo = "-";
                                policy.PolicyEffectiveDate = quotation.QuotationRequest?.RequestPolicyEffectiveDate?.Date;
                                policy.PolicyExpiryDate = quotation.QuotationRequest?.RequestPolicyEffectiveDate?.AddYears(1).AddDays(-1).Date;
                                policy.PolicyIssueDate = quotation.QuotationRequest.CreatedDateTime.Date;
                            }
                        }
                        if (policy != null)
                        {
                            var policyRequestMessage = new PolicyRequest();
                            policyRequestMessage.ReferenceId = invoice.ReferenceId;
                            policyRequestMessage.PaymentBillNumber = invoice.InvoiceNo.ToString();
                            policyRequestMessage.InsuredEmail = policy.CheckoutDetail?.Email;
                            policyRequestMessage.InsuredMobileNumber = policy.CheckoutDetail?.Phone;
                            PolicyResponse policyResponseMessage = new PolicyResponse();
                            policyResponseMessage.PolicyNo = policy.PolicyNo;
                            policyResponseMessage.PolicyEffectiveDate = policy.PolicyEffectiveDate;
                            policyResponseMessage.PolicyExpiryDate = policy.PolicyExpiryDate;
                            policyResponseMessage.PolicyIssuanceDate = policy.PolicyIssueDate;
                            byte[] invoiceFileAsByteArray = null;
                            Invoice generatedInvoice = GenerateAndSaveInvoicePdf(policyRequestMessage, policyResponseMessage, Utilities.GetInternalServerIP(),
                                quotation.InsuranceCompany.NameAR,
                                quotation.InsuranceCompany.NameEN,
                                quotation.InsuranceCompany.Key,
                                quotation.QuotationRequest.Insured,
                                quotation.InsuranceCompany.VAT,
                                 quotation.QuotationRequest.Vehicle,
                                 isAutoleasingPolicy,
                                out invoiceFileAsByteArray, out exception);
                            return generatedInvoice != null ? true : false;
                        }
                        else
                        {
                            exception = "policy is null";
                        }
                    }
                    else
                    {
                        exception = "quotation is null";
                    }
                }
                else
                {
                    exception = "invoice is null";
                }
                return false;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }

    }
        
}
