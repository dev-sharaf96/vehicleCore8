using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
{
    public interface IInvoiceService
    {
        Invoice GenerateAndSaveInvoicePdf(PolicyRequest policyRequestMessage, PolicyResponse policy,
              string serverIP, string companyNameAr, string companyNameEn, string companyKey,
              Insured insured, string vat, Vehicle vehicle, bool isAutoleasingPolicy, out byte[] invoiceFileAsByteArray, out string exception);
        byte[] GenerateInvoiceFileFromPolicyData(object invoiceData, string templatePath);
        bool GenerateInvoicePdf(string referenceId, out string exception);
    }
}