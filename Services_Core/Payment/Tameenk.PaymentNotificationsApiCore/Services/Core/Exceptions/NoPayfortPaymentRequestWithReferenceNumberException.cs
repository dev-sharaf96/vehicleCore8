using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Services.Core.Exceptions
{
    public class NoPayfortPaymentRequestWithReferenceNumberException
        : Exception
    {
        public NoPayfortPaymentRequestWithReferenceNumberException(string referenceNumber)
            : base("Unable to find payfort payment request with reference number = " + referenceNumber)
        {

        }
    }
}