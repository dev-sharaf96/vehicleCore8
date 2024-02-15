using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Core.Payments
{
    public interface IPayfortPaymentService
    {
        void ProcessPayment(string merchantReference, PayfortPaymentResponse reponse);

        /// <summary>
        /// Reprocess payment by regenerating payfortPayment response and update checkout status
        /// </summary>
        /// <param name="merchantReference">Merchant Reference</param>
        void ReProcessPayment(string merchantReference);

        /// <summary>
        /// Process policy update payment 
        /// </summary>
        /// <param name="merchantReference">Merchant reference</param>
        /// <param name="response">Payfort response</param>
        void ProcessPolicyUpdPayment(string merchantReference, PayfortPaymentResponse response);

        /// <summary>
        /// Validate the payfort response signature.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        bool ValidatedResponse(Dictionary<string, string> payFortParams);

        /// <summary>
        /// Build payfort payment request paramater that will be posted to payfort.
        /// </summary>
        /// <param name="payfortPaymentRequestId">The payfort payment request identifier</param>
        /// <param name="language">The user language</param>
        /// <param name="baseUrl">The application base url.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> BuildPaymentRequestParameters(int payfortPaymentRequestId, string language, string baseUrl, string returnUrl = null);



        /// <summary>
        /// Get checkout details reference identifier from payfort response
        /// </summary>
        /// <param name="payfortResponse">The pay fort response object.</param>
        /// <returns></returns>
        string GetCheckoutReferenceIdFromPayfortResponse(PayfortResponse payfortResponse);


        /// <summary>
        /// Build the payfort payment response entity 
        /// </summary>
        /// <param name="payfortResponse">The payfort response.</param>
        /// <returns></returns>
        PayfortPaymentResponse BuildPayfortPaymentResponse(PayfortResponse payfortResponse);
    }
}
