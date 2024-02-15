using Newtonsoft.Json;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Dto.Quotation;


namespace Tameenk.Services.Quotation.Components
{
    public class QuotationOutput
    {

        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha = 4,
            ServiceException = 5,
            OwnerNationalIdAndNationalIdAreEqual = 6,
            ComprehensiveIsNotAvailable = 7,
            NoReturnedQuotation=8,
            DifferentProductPrice = 9,
            NoCompanyReturned =10,
            ExpiredPolicyEffectiveDate=11,
            RepairMethodSettingsNotFoundForThisBank = 12,
            NoDataWithThisUserIdAndExternalId = 13,
            HashedNotMatched,
            CommercialProductNotSupported,
            InvalidODPolicyExpiryDate
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        public string LogDescription
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }


        [JsonProperty("quotationResponseModel")]
        public QuotationResponseModel QuotationResponseModel { get; set; }

        public QuotationResponse QuotationResponse { get; set; }
        public List<ProductDto> Products { get; set; }
        public QuotationServiceRequest QuotationServiceRequest { get; set; }

        public int VehicleValue { get; set; }
        public bool? IsInitialQuotation { get; set; }

        public string TermsAndConditionsFilePathComp
        {
            get;
            set;
        }
        public string TermsAndConditionsFilePath
        {
            get;
            set;
        }
        public string TermsAndConditionsFilePathSanadPlus
        {
            get;
            set;
        }
        public bool ShowTabby
        {
            get;
            set;
        }
        public bool ActiveTabbyTPL
        {
            get;
            set;
        }
        public bool ActiveTabbyComp
        {
            get;
            set;
        }
        public bool ActiveTabbySanadPlus
        {
            get;
            set;
        }
        public bool ActiveTabbyWafiSmart
        {
            get;
            set;
        }

        public bool IsRenewal
        {
            get;
            set;
        }

        public bool ActiveTabbyMotorPlus
        {
            get;
            set;
        }
    }
}
