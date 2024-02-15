using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.PaymentNotificationsApi.Services.Core
{
    public interface IPolicyGenerationTask
    {
        Task GenerateAndSavePolicyAndInvoiceAsPdfViaExternalService(string referenceId);
    }
}
