using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments.Edaat;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Enums.Payments;
using Tameenk.Loggin.DAL;
using Tameenk.PaymentNotificationsApi.Contexts;
using Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer;
using Tameenk.PaymentNotificationsApi.Models;
using Tameenk.PaymentNotificationsApi.Repository;
using Tameenk.PaymentNotificationsApi.Services.Core;
using Tameenk.Services.Core.Payments;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Orders;

namespace Tameenk.PaymentNotificationsApi.Services.Implementation
{
    public class EdaatNotificationsServices : IEdaatNotificationsServices
    {
        private readonly IRepository<EdaatRequest> edaatRequestRepository;
        private readonly IRepository<EdaatResponse> edaatResponseRepository;
        private readonly IRepository<EdaatNotification> edaatNotificationRepository;
        private readonly IRepository<CheckoutDetail> checkoutDetailsRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IPaymentNotificationSmsSender _smsSender;
        private readonly IPolicyProcessingService _policyProcessingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;


        public EdaatNotificationsServices(IRepository<EdaatRequest> edaatRequestRepository,
        IRepository<EdaatResponse> edaatResponseRepository,
        IRepository<EdaatNotification> edaatNotificationRepository,
        IRepository<CheckoutDetail> checkoutDetailsRepository,
        IRepository<Invoice> invoiceRepository,
        IPaymentNotificationSmsSender smsSender,
        IPolicyProcessingService policyProcessingService,
        IShoppingCartService shoppingCartService,
        IOrderService orderService)
        {
            this._invoiceRepository = invoiceRepository;
            this.checkoutDetailsRepository = checkoutDetailsRepository;
            this.edaatNotificationRepository = edaatNotificationRepository;
            this.edaatResponseRepository = edaatResponseRepository;
            this.edaatRequestRepository = edaatRequestRepository;
            this._smsSender = smsSender;
            this._policyProcessingService = policyProcessingService;
            this._shoppingCartService = shoppingCartService;
            _orderService = orderService;
        }
        public EdaatResponseMessage SaveAndProcessEdaatNotification(List<EdaatPayment> Payments)
        {
            EdaatResponseMessage output = new EdaatResponseMessage();
            var edaatNotification = new EdaatNotification();
            edaatNotification.ServerIP = Utilities.GetInternalServerIP();
            edaatNotification.UserIP = Utilities.GetUserIPAddress();
            try
            {
                if (Utilities.GetUserIPAddress() != "81.21.62.155" && Utilities.GetUserIPAddress() != "81.21.62.153")
                {
                    output.ErrorCode = 401;
                    output.Description = "Unauthorized request recived IP is:"+ Utilities.GetUserIPAddress();
                    edaatNotification.ErrorCode = output.ErrorCode;
                    edaatNotification.ErrorDescription = output.Description;
                    edaatNotificationRepository.Insert(edaatNotification);
                    return output;
                }
                if (Payments == null || Payments.Count == 0)
                {
                    output.ErrorCode = 2;
                    output.Description = "Payments is empty";
                    edaatNotification.ErrorCode = output.ErrorCode;
                    edaatNotification.ErrorDescription = output.Description;
                    edaatNotificationRepository.Insert(edaatNotification);
                    return output;
                }
                foreach (var payment in Payments)
                {
                    edaatNotification = new EdaatNotification();
                    edaatNotification.ServerIP = Utilities.GetInternalServerIP();
                    edaatNotification.UserIP = Utilities.GetUserIPAddress();
                    edaatNotification.JsonRequest = JsonConvert.SerializeObject(Payments);
                    if (string.IsNullOrEmpty(payment.BillNo))
                    {
                        output.ErrorCode = 3;
                        output.Description = "BillNo is null";
                        edaatNotification.ErrorCode = output.ErrorCode;
                        edaatNotification.ErrorDescription = output.Description;
                        edaatNotificationRepository.Insert(edaatNotification);
                       continue;
                    }
                    if (string.IsNullOrEmpty(payment.InvoiceNo))
                    {
                        output.ErrorCode = 4;
                        output.Description = "InvoiceNo is null";
                        edaatNotification.ErrorCode = output.ErrorCode;
                        edaatNotification.ErrorDescription = output.Description;
                        edaatNotificationRepository.Insert(edaatNotification);
                        continue;
                    }
                    if (payment.PaymentAmount == 0)
                    {
                        output.ErrorCode = 5;
                        output.Description = "PaymentAmount is 0";
                        edaatNotification.ErrorCode = output.ErrorCode;
                        edaatNotification.ErrorDescription = output.Description;
                        edaatNotificationRepository.Insert(edaatNotification);
                        continue;
                    }
                    edaatNotification.BillNo = payment.BillNo;
                    edaatNotification.InvoiceNo = payment.InvoiceNo;
                    edaatNotification.InternalCode = payment.InternalCode;
                    edaatNotification.PaymentAmount = payment.PaymentAmount;
                    edaatNotification.PaymentDate = payment.PaymentDate;
                    edaatNotification.CreatedDate = DateTime.Now;
                    var invoiceNo = payment.InvoiceNo;
                    var response = edaatResponseRepository.TableNoTracking.FirstOrDefault(x => x.InvoiceNo == invoiceNo&&x.ReferenceId== payment.InternalCode);
                    if (response == null)
                    {
                        output.ErrorCode = 6;
                        output.Description = "Edaat response is null";
                        edaatNotification.ErrorCode = output.ErrorCode;
                        edaatNotification.ErrorDescription = output.Description;
                        edaatNotificationRepository.Insert(edaatNotification);
                        continue;
                    }
                    edaatNotification.EdaatRequestId = response.EdaatRequestId;
                    edaatNotification.UserId = response.UserId;
                    edaatNotification.ReferenceId = response.ReferenceId;

                    var checkoutDetails = checkoutDetailsRepository.Table
                        .Include(x => x.OrderItems.Select(y => y.Product.InsuranceCompany))
                        .Include(x => x.OrderItems.Select(y => y.Product.QuotationResponse.ProductType))
                        .FirstOrDefault(c => c.ReferenceId == response.ReferenceId);
                    if (checkoutDetails == null)
                    {
                        output.ErrorCode = 7;
                        output.Description = "checkoutDetails is null for referenceID=" + response.ReferenceId;
                        edaatNotification.ErrorCode = output.ErrorCode;
                        edaatNotification.ErrorDescription = output.Description;
                        edaatNotificationRepository.Insert(edaatNotification);
                        continue;
                    }
                    edaatNotification.Channel = checkoutDetails.Channel;
                    var invoice = _invoiceRepository.TableNoTracking.FirstOrDefault(x => x.ReferenceId == response.ReferenceId);
                    if (invoice == null)
                    {
                        invoice = _orderService.CreateInvoice(response.ReferenceId, checkoutDetails.SelectedInsuranceTypeCode.Value,
                            checkoutDetails.InsuranceCompanyId.Value);
                    }
                    if (invoice == null)
                    {
                        output.ErrorCode = 8;
                        output.Description = "invoice is null for referenceID=" + response.ReferenceId;
                        edaatNotification.ErrorCode = output.ErrorCode;
                        edaatNotification.ErrorDescription = output.Description;
                        edaatNotificationRepository.Insert(edaatNotification);
                        continue;
                    }
                    if (invoice.TotalPrice != edaatNotification.PaymentAmount+ invoice.TotalBCareDiscount)
                    {
                        output.ErrorCode = 9;
                        output.Description = "PaymentAmount is not valid as we recived " + edaatNotification.PaymentAmount
                            + " and invoice amount is " + invoice.TotalPrice;
                        edaatNotification.ErrorCode = output.ErrorCode;
                        edaatNotification.ErrorDescription = output.Description;
                        edaatNotificationRepository.Insert(edaatNotification);
                        continue;
                    }
                    var edaatInfo = edaatNotificationRepository.TableNoTracking.Where(a => a.InvoiceNo == payment.InvoiceNo && a.BillNo == payment.BillNo && a.ReferenceId == response.ReferenceId&&a.ErrorCode==1).FirstOrDefault();
                    if (edaatInfo != null)
                    {
                        continue;
                    }
                    if (checkoutDetails.PolicyStatusId == (int)EPolicyStatus.PendingPayment ||
                        checkoutDetails.PolicyStatusId == (int)EPolicyStatus.PaymentFailure)
                    {
                        checkoutDetails.PolicyStatusId = (int)EPolicyStatus.PaymentSuccess;
                        checkoutDetails.ModifiedDate = DateTime.Now;
                        checkoutDetails.PaymentMethodId = (int)PaymentMethodCode.Edaat;
                        string companyName = string.Empty;
                        if (checkoutDetails.InsuranceCompany != null && !string.IsNullOrEmpty(checkoutDetails.InsuranceCompany.Key))
                        {
                            companyName = checkoutDetails.InsuranceCompany.Key;
                        }
                        else
                        {
                            companyName = checkoutDetails.InsuranceCompanyName;
                        }
                        _policyProcessingService.InsertPolicyProcessingQueue(checkoutDetails.ReferenceId, checkoutDetails.InsuranceCompanyId.Value, companyName, "Portal");
                        checkoutDetailsRepository.Update(checkoutDetails);
                        _shoppingCartService.EmptyShoppingCart(checkoutDetails.UserId, checkoutDetails.ReferenceId);
                        LanguageTwoLetterIsoCode culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals(LanguageTwoLetterIsoCode.Ar.ToString(), StringComparison.OrdinalIgnoreCase) ?
                                LanguageTwoLetterIsoCode.Ar : LanguageTwoLetterIsoCode.En;
                        _smsSender.SendSms(checkoutDetails, edaatNotification.PaymentAmount, culture);
                        if (!string.IsNullOrEmpty(checkoutDetails.DiscountCode)) //mark discount code as consumed
                        {
                            _orderService.UpdateDiscountCodeToBeConsumed(checkoutDetails.VehicleId, checkoutDetails.DiscountCode,checkoutDetails.ReferenceId);
                        }
                    }
                    output.ErrorCode = 1;
                    output.Description = "Success";
                    edaatNotification.ErrorCode = output.ErrorCode;
                    edaatNotification.ErrorDescription = output.Description;
                    edaatNotificationRepository.Insert(edaatNotification);
                }
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = 500;
                output.Description = ex.ToString();
                edaatNotification.ErrorCode = output.ErrorCode;
                edaatNotification.ErrorDescription = ex.ToString();
                edaatNotificationRepository.Insert(edaatNotification);
                return output;
            }
        }

    }
}