using System;
using System.Collections.Generic;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Payments;

namespace Tameenk.Services.Implementation.Payments
{
    public class PaymentMethodService : IPaymentMethodService
    {
        #region Fields

        private readonly IRepository<PaymentMethod> _paymentMethodRepository;

        #endregion

        #region Ctor
        public PaymentMethodService(IRepository<PaymentMethod> paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<PaymentMethod>));
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get payment methods.
        /// </summary>
        /// <param name="activeOnly">Get only active payment method if true otherwise get all.</param>
        /// <returns>List of payment methods.</returns>
        public List<PaymentMethod> GetPaymentMethods(bool activeOnly = true)
        {
            var query = _paymentMethodRepository.Table;
            if (activeOnly)
            {
                query = query.Where(pm => pm.Active);
            }
            var paymentMethods = query.OrderBy(pm => pm.Order).ToList();
            bool applePayExist = paymentMethods.Where(a => a.Code == (int)PaymentMethodCode.ApplePay).Any();
            if (applePayExist)
            {
                if (Utilities.GetUserAgent().ToLower().Contains("safari"))
                {
                    var deviceInfo = Utilities.GetDeviceInfo();
                    if (deviceInfo != null
                        && deviceInfo.Client.ToLower().Contains("safari")
                        &&deviceInfo.OS.ToLower().Contains("ios"))
                    {
                        return paymentMethods;
                    }
                    else
                    {
                        // get all except apple pay
                        paymentMethods = paymentMethods.Where(p => p.Code != (int)PaymentMethodCode.ApplePay).ToList();
                        return paymentMethods;
                    }
                }
            }
            return paymentMethods;
        }
        public PaymentMethod GetPaymentMethodsByCode(int code)
        {
          return _paymentMethodRepository.TableNoTracking.Where(a=>a.Code==code&&a.Active).FirstOrDefault();
        }
        public int GetPaymentMethodIdByBrand(string brandName)
        {
            if (brandName == "mada")
                return (int)PaymentMethodCode.Mada;
            if (brandName == "visa"|| brandName == "master"|| brandName == "mastercard")
                return (int)PaymentMethodCode.Hyperpay;
            if (brandName == "amex")
                return (int)PaymentMethodCode.AMEX;
            return -1;
        }
        #endregion
    }
}
