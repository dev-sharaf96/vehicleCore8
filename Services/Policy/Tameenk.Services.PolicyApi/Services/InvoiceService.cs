using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.Core.Quotations;
using Tameenk.Services.Logging;
using Tameenk.Services.PolicyApi.Models;

namespace Tameenk.Services.PolicyApi.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InsuranceCompany> _insuranceCompanyRepository;
        private readonly IQuotationService _quotationService;
        private readonly ILogger _logger;

        public InvoiceService(IRepository<Invoice> invoiceRepository
            , IRepository<InsuranceCompany> insuranceCompanyRepository
            , IQuotationService quotationService
            , ILogger logger)
        {
            _invoiceRepository = invoiceRepository;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _quotationService = quotationService;
            _logger = logger;
        }

        public async Task<Invoice> GenerateAndSaveInvoicePdf(PolicyRequest policyRequestMessage, PolicyResponse policy)
        {
            try
            {
                int paymentBillNumber = int.Parse(policyRequestMessage.PaymentBillNumber);
                var invoice = _invoiceRepository.Table.Include(x => x.Invoice_Benefit).Include(x => x.InvoiceFile).FirstOrDefaultAsync(
                    i => i.InvoiceNo == paymentBillNumber).Result;
                var quotationResponse = _quotationService.GetQuotationResponseByReferenceId(policyRequestMessage.ReferenceId);

                string insuranceTypeEn = invoice.InsuranceTypeCode == 1 ? "Third Party Liability" : "Comprehensive";
                string insuranceTypeAr = invoice.InsuranceTypeCode == 1 ? "تأمين ضد الغير" : "تأمين شامل";

                string clientFullName = quotationResponse.QuotationRequest.Insured.FirstNameEn;
                if (!string.IsNullOrEmpty(quotationResponse.QuotationRequest.Insured.MiddleNameEn))
                    clientFullName = clientFullName + " " + quotationResponse.QuotationRequest.Insured.MiddleNameEn;
                if (!string.IsNullOrEmpty(quotationResponse.QuotationRequest.Insured.LastNameEn))
                    clientFullName = clientFullName + " " + quotationResponse.QuotationRequest.Insured.LastNameEn;

                var invoiceBenefits = from invoiceBenefit in invoice.Invoice_Benefit.AsEnumerable()
                                      select new
                                      {
                                          DescriptionEnglish = invoiceBenefit.Benefit.EnglishDescription,
                                          DescriptionArabic = invoiceBenefit.Benefit.ArabicDescription,
                                          Price = invoiceBenefit.BenefitPrice
                                      };
                if (policy != null && policy.PolicyExpiryDate == null && policy.PolicyEffectiveDate != null)
                {
                    policy.PolicyExpiryDate = policy.PolicyEffectiveDate.Value.AddYears(1).AddDays(-1);
                }

                var invoiceDataObj = new
                {
                    InvoiceNo = invoice.InvoiceNo,
                    ClientName = clientFullName,
                    Email = policyRequestMessage.InsuredEmail,
                    PhoneNumber = policyRequestMessage.InsuredMobileNumber,
                    InvoiceDate = invoice.InvoiceDate.ToString("dd/MM/yyyy"),
                    InvoiceDueDate = invoice.InvoiceDueDate.ToString("dd/MM/yyyy"),
                    ReferenceNumber = policyRequestMessage.ReferenceId,
                    InsuranceTypeAr = insuranceTypeAr,
                    InsuranceTypeEn = insuranceTypeEn,
                    InsurerNameAr = quotationResponse.InsuranceCompany?.NameAR,
                    InsurerNameEn = quotationResponse.InsuranceCompany?.NameEN,
                    PolicyNo = policy == null ? "" : policy.PolicyNo,
                    PolicyEffectiveDate = policy == null ? ""
                        : (policy.PolicyEffectiveDate.HasValue ? policy.PolicyEffectiveDate.Value.ToString("dd/MM/yyyy") : ""),
                    PolicyExpiryDate = policy == null ? ""
                        : (policy.PolicyExpiryDate.HasValue ? policy.PolicyExpiryDate.Value.ToString("dd/MM/yyyy") : ""),
                    PolicyIssueDate = policy == null ? ""
                        : (policy.PolicyIssuanceDate.HasValue ? policy.PolicyIssuanceDate.Value.ToString("dd/MM/yyyy") : ""),
                    ProductPrice = invoice.ProductPrice,
                    Fees = invoice.Fees,
                    Vat = invoice.Vat,
                    TotalVat = invoice.Vat,
                    ExtraPremiumPrice = invoice.ExtraPremiumPrice,
                    Discount = invoice.Discount,
                    TotalDiscount = invoice.Discount,
                    SubTotalPrice = invoice.SubTotalPrice,
                    TotalPrice = invoice.TotalPrice,
                    Benefits = invoiceBenefits.ToArray()
                };

                var invoiceFileAsByteArray = await GenerateInvoiceFileFromPolicyData(invoiceDataObj);
                if (invoice.InvoiceFile == null)
                    invoice.InvoiceFile = new InvoiceFile() { Id = invoice.Id, InvoiceData = invoiceFileAsByteArray };
                else
                    invoice.InvoiceFile.InvoiceData = invoiceFileAsByteArray;

                _invoiceRepository.Update(invoice);

                return invoice;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<byte[]> GenerateInvoiceFileFromPolicyData(object invoiceData)
        {
            _logger.Log($"InvoicSerivce -> GenerateInvoiceFileFromPolicyData <<start>> invoice data:{JsonConvert.SerializeObject(invoiceData)}");
            try
            {
                string invoiceDataJsonString = JsonConvert.SerializeObject(invoiceData);
                ReportGenerationModel reportGenerationModel = new ReportGenerationModel
                {
                    ReportType = "TameenkInvoice",
                    ReportDataAsJsonString = invoiceDataJsonString
                };

                HttpClient client = new HttpClient();
                string reportGenerationModelAsJson = JsonConvert.SerializeObject(reportGenerationModel);
                var httpContent = new StringContent(reportGenerationModelAsJson, Encoding.UTF8, "application/json");
                _logger.Log($"InvoicSerivce -> GenerateInvoiceFileFromPolicyData calling pdf generator on url: {System.Configuration.ConfigurationManager.AppSettings["PolicyPDFGeneratorAPIURL"] }api/PolicyPdfGenerator");
                HttpResponseMessage response = await client.PostAsync(System.Configuration.ConfigurationManager.AppSettings["PolicyPDFGeneratorAPIURL"] + "api/PolicyPdfGenerator", httpContent);
                var pdfGeneratorResult = await response.Content.ReadAsStringAsync();
                _logger.Log($"InvoicSerivce -> GenerateInvoiceFileFromPolicyData pdf generator response string: {pdfGeneratorResult}");
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<byte[]>(pdfGeneratorResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"InvoicSerivce -> GenerateInvoiceFileFromPolicyData exception happend while generating invoice, base exception: {ex.GetBaseException().ToString()}");
            }
            return null;
        }
    }
}
