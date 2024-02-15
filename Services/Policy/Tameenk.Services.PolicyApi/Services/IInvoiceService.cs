using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.PolicyApi.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> GenerateAndSaveInvoicePdf(PolicyRequest policyRequestMessage, PolicyResponse policy);
        Task<byte[]> GenerateInvoiceFileFromPolicyData(object invoiceData);
    }
}