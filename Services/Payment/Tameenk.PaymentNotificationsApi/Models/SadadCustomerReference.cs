using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.PaymentNotificationsApi.Repository;

namespace Tameenk.PaymentNotificationsApi.Models
{
    public class SadadCustomerReference
    {
        private const int _applicationIdLength = 2;

        public readonly string SourceCustomerReferenceNumber;
        public readonly string DynamicPart;
        public readonly string ApplicationId;
        public readonly string CustomerAccountNumber;
        public readonly int TameenkInvoiceNo;

        public SadadCustomerReference(string customerReferenceNumber)
        {
            SourceCustomerReferenceNumber = customerReferenceNumber;
            if (!string.IsNullOrEmpty(customerReferenceNumber) &&
                customerReferenceNumber.Length > (RepositoryConstants.SadadStaticPartOfCustomerRefNo.Length + _applicationIdLength))
            {
                DynamicPart = customerReferenceNumber.ToLower().Replace(RepositoryConstants.SadadStaticPartOfCustomerRefNo.ToLower(), string.Empty);
                ApplicationId = DynamicPart.Substring(0, _applicationIdLength);
                CustomerAccountNumber = DynamicPart.Substring(_applicationIdLength);
                TameenkInvoiceNo = int.Parse(CustomerAccountNumber);
            }
        }
    }
}