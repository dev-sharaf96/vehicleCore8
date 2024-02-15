using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Resources.Checkout;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Checkouts;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation;

namespace Tameenk.PaymentNotificationsApi.Services.Implementation
{
    public class PaymentNotificationSmsSender : IPaymentNotificationSmsSender
    {
        private readonly INotificationService _notificationService;
        private readonly IInsuranceCompanyService _inuranceCompanyService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICheckoutsService _checkoutsService;

        public PaymentNotificationSmsSender(INotificationService notificationService,
            IInsuranceCompanyService inuranceCompanyService, IAuthorizationService authorizationService, ICheckoutsService checkoutsService)
        {
            _notificationService = notificationService;
            _inuranceCompanyService = inuranceCompanyService;
            _authorizationService = authorizationService;
            _checkoutsService = checkoutsService;
        }

        public void SendSms(CheckoutDetail checkoutDetail, decimal paidAmount, LanguageTwoLetterIsoCode culture)
        {
            string exception = string.Empty;
            var isRenewalRequest = _checkoutsService.CheckIfQuotationIsRenewalByReferenceId(checkoutDetail.ReferenceId, out exception);

            var lang = culture == LanguageTwoLetterIsoCode.Ar ? "ar" : "en";
            var product = checkoutDetail !=null && checkoutDetail.OrderItems !=null && checkoutDetail.OrderItems.Count>0 ? checkoutDetail.OrderItems.FirstOrDefault().Product : null;
            var companyName = checkoutDetail.InsuranceCompanyId.HasValue
                                            ? WebResources.ResourceManager.GetString($"InsuranceCompany_{product.ProviderId.Value}", CultureInfo.GetCultureInfo(lang))
                                            : string.Empty;

            string productType = string.Empty;
            if (product.InsuranceTypeCode == 2)
            {
                if (checkoutDetail.InsuranceCompanyId.Value == 2)
                    productType = CheckoutResources.ResourceManager.GetString("COMP_ACIG", CultureInfo.GetCultureInfo(lang));
                else if (checkoutDetail.InsuranceCompanyId.Value == 5)
                    productType = CheckoutResources.ResourceManager.GetString("COMP_TUIC", CultureInfo.GetCultureInfo(lang));
                else if (checkoutDetail.InsuranceCompanyId.Value == 17)
                    productType = CheckoutResources.ResourceManager.GetString("COMP_UCA", CultureInfo.GetCultureInfo(lang));
                //else if (checkoutDetail.InsuranceCompanyId.Value == 23)
                //    productType = CheckoutResources.ResourceManager.GetString("COMP_TokioMarine", CultureInfo.GetCultureInfo(lang));
                else
                    productType = CheckoutResources.ResourceManager.GetString("COMP", CultureInfo.GetCultureInfo(lang));
            }
            else if (product.InsuranceTypeCode == 7)
                productType = CheckoutResources.ResourceManager.GetString("SANADPLUS", CultureInfo.GetCultureInfo(lang));
            else if (product.InsuranceTypeCode == 8)
                productType = CheckoutResources.ResourceManager.GetString("WafiSmart", CultureInfo.GetCultureInfo(lang));
            //else if (product.InsuranceTypeCode == 9)
            //    productType = CheckoutResources.ResourceManager.GetString("", CultureInfo.GetCultureInfo(lang));
            else if (product.InsuranceTypeCode == 13)
                productType = CheckoutResources.ResourceManager.GetString("MotorPlus", CultureInfo.GetCultureInfo(lang));
            else
                productType = CheckoutResources.ResourceManager.GetString("TPL_SMS", CultureInfo.GetCultureInfo(lang));

            var phoneNumber = checkoutDetail.Phone;
            if (!isRenewalRequest)
            {
                var userAccount = _authorizationService.GetUserDBByID(checkoutDetail.UserId);
                if (userAccount != null && !string.IsNullOrEmpty(userAccount.PhoneNumber))
                    phoneNumber = userAccount.PhoneNumber;
            }

            var message = string.Format(Tameenk.Resources.WebResources.WebResources.ProcessPayment_SendingSMS,
                productType, companyName, paidAmount); ;
            var smsModel = new SMSModel()
            {
                PhoneNumber = phoneNumber,
                MessageBody = message,
                Method = SMSMethod.SadadPaymentNotification.ToString(),
                Module = Module.Vehicle.ToString(),
                Channel = checkoutDetail.Channel
            };
            _notificationService.SendSmsBySMSProviderSettings(smsModel);
        }
    }
}