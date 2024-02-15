using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Core.Payments
{
    public interface IRiyadBankMigsPaymentService
    {
        /// <summary>
        /// Generate riyadh bank request url.
        /// </summary>
        /// <param name="riyadBankMigsRequest">The RiyadBank request parameters</param>
        /// <returns></returns>
        string CreateRiyadBankRequestUrl(RiyadBankMigsRequest riyadBankMigsRequest);


        /// <summary>
        /// Validate the response of parameter
        /// </summary>
        /// <param name="secureHash">The secure hash (signature) send from MIGS portal in response parameter.</param>
        /// <param name="list">The list of parameters without the secure hash and secure hash type.</param>
        /// <returns></returns>
        bool ValidateResponse(string secureHash, IEnumerable<KeyValuePair<string, string>> list);

        /// <summary>
        /// Process payment according to MIGS response.
        /// </summary>
        /// <param name="riyadBankMigsResponse">The riyadbank MIGS response.</param>
        bool ProcessPayment(RiyadBankMigsResponse riyadBankMigsResponse, LanguageTwoLetterIsoCode culture);
    }
}
