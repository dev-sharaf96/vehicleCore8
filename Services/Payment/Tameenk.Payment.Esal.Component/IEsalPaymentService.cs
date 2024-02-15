using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Esal;
using Tameenk.Payment.Esal.Component.Model.Settlement;

namespace Tameenk.Payment.Esal.Component
{
    public interface IEsalPaymentService
    {
        EsalOutput UploadEsalInvoice(EsalRequestDto esalRequest, string channel, int companyId, string companyName, string driverNin, Guid userId,
            string referenceId, string vehicleId, string externalId);
        EsalOutput CancelEsalInvoice(EsalCancelRequestDto esalRequest);
        EsalError UpdateInvoiceWithSadatID(EsalUploadInvoiceNotification invoiceNotification,string requestIPAddress);
        EsalError UpdateInvoicePayment(EsalPaymentNotification esalPaymentNotification, out CheckoutDetail checkoutDetails);

        EsalOutput SaveEsalSettlement(EsalSettlementModel _esalSettlement);
        void CancelExpiredInvoices();
        string GetSadadNumber(string invoiceNo, string referenceId);
        bool CheckIfRequestExceededOneMinutes(string invoiceNo, string referenceId);
        bool IsSadadBillIdExistBefore(string referenceId);

    }
}
