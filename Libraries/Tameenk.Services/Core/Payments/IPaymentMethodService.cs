using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Payments;

namespace Tameenk.Services.Core.Payments
{
    public interface IPaymentMethodService
    {
        /// <summary>
        /// Get payment methods.
        /// </summary>
        /// <param name="activeOnly">Get only active payment method if true otherwise get all.</param>
        /// <returns>List of payment methods.</returns>
        List<PaymentMethod> GetPaymentMethods(bool activeOnly = true);
        PaymentMethod GetPaymentMethodsByCode(int code);
        int GetPaymentMethodIdByBrand(string brandName);
    }
}
