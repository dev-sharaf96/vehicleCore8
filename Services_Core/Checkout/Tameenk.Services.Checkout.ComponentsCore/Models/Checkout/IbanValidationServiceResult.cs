using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Checkout.Components
{
    public partial class IbanValidationServiceResult
    {
        [JsonProperty("bank_data")]
        public BankData BankData { get; set; }

        [JsonProperty("errors")]
        public List<ErrorItem> Errors { get; set; }

        [JsonProperty("validations")]
        public Validations Validations { get; set; }

        [JsonProperty("sepa_data")]
        public SepaData SepaData { get; set; }
    }

    public partial class BankData
    {
        [JsonProperty("bic")]
        public string Bic { get; set; }

        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("bank")]
        public string Bank { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("www")]
        public string Www { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_iso")]
        public string CountryIso { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("bank_code")]
        public string BankCode { get; set; }

        [JsonProperty("branch_code")]
        public string BranchCode { get; set; }
    }

    public partial class SepaData
    {
        [JsonProperty("SCT")]
        public string Sct { get; set; }

        [JsonProperty("SDD")]
        public string Sdd { get; set; }

        [JsonProperty("COR1")]
        public string Cor1 { get; set; }

        [JsonProperty("B2B")]
        public string B2B { get; set; }

        [JsonProperty("SCC")]
        public string Scc { get; set; }
    }

    public partial class Validations
    {
        [JsonProperty("chars")]
        public ValidationItem Chars { get; set; }

        [JsonProperty("account")]
        public ValidationItem Account { get; set; }

        [JsonProperty("iban")]
        public ValidationItem Iban { get; set; }

        [JsonProperty("structure")]
        public ValidationItem Structure { get; set; }

        [JsonProperty("length")]
        public ValidationItem Length { get; set; }

        [JsonProperty("country_support")]
        public ValidationItem CountrySupport { get; set; }

        public List<string> SuccessValidationCodes
        {
            get
            {
                return new List<string>
                {
                    Chars?.Code,
                    Account?.Code,
                    Iban?.Code,
                    Structure?.Code,
                    Length?.Code,
                    CountrySupport?.Code
                }
                .Where(e => e != null && IBANValidationnDictionaries.SuccessValidationCodes.Contains(e))
                .ToList();
            }
        }

        public List<string> FailureValidationCodes
        {
            get
            {
                return new List<string>
                {
                    Chars?.Code,
                    Account?.Code,
                    Iban?.Code,
                    Structure?.Code,
                    Length?.Code,
                    CountrySupport?.Code
                }
                .Where(e => e != null && IBANValidationnDictionaries.FailureValidationCodes.Contains(e))
                .ToList();
            }
        }
    }

    public partial class ValidationItem
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public partial class ErrorItem
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
