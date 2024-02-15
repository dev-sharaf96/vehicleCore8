using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Services.Implementation.Payments;

namespace Tameenk.Services.Core.Payments
{
    public interface IPaymentService
    {
        IPagedList<PaymentAdmin> GetAllFailAndPendingPayment(PaymentFilter filter, out int count, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "", bool sortOrder = false);
        void ReProcessFailPayment(string referenceId, string userName);
        PaymentAdmin GetDetailsOfFailPayment(string referenceId);
        IEnumerable<PaymentMethod> GetAllPaymentMethod();
        bool AddHyperPayResponse(HyperpayResponse hyperpayResponse);
        List<PaymentMethod> GetActivePaymentMethod();        HyperpayResponse GetFromHyperpayResponseSuccessTransaction(string referenceId);
    }
}
