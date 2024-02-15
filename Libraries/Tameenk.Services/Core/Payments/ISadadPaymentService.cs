using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Sadad;

namespace Tameenk.Services.Core.Payments
{
    public interface ISadadPaymentService
    {
        SadadResponse ExecuteSadadPayment(SadadRequest sadadRequest, bool isActive, int companyId, string companyName, string referenceId, string externalId);


        /// <summary>
        /// Update checkout payment status
        /// </summary>
        /// <param name="checkoutDetailsId">Reference ID</param>
        CheckoutDetail UpdateCheckoutPaymentStatus(string checkoutDetailsId);
    }
}
