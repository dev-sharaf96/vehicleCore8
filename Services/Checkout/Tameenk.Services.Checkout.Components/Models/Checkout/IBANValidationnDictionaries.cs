using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Checkout.Components
{
    public static class IBANValidationnDictionaries
    {
        public static List<string> FailureValidationCodes { get; set; } = new List<string>
        {
            "201",
            "202",
            "203",
            "205",
            "206",
            "207"
        };
        public static List<string> SuccessValidationCodes { get; set; } = new List<string>
        {
            "001",
            "002",
            "003",
            "004",
            "005",
            "006",
            "007",
        };
        public static Dictionary<string, string> ValidationsFailureDictionary { get; set; } = new Dictionary<string, string>
        {
            {"201", "Account Number check digit not correct" },
            {"202", "IBAN Check digit not correct" },
            {"203", "IBAN Length is not correct" },
            {"205", "IBAN structure is not correct" },
            {"206", "IBAN contains illegal characters" },
            {"207", "Country does not support IBAN standard" }
        };

        public static Dictionary<string, string> ValidationsSuccessDictionary { get; set; } = new Dictionary<string, string>
        {
            {"001", "IBAN Check digit is correct" },
            {"002", "Account Number check digit is correct" },
            {"003", "IBAN Length is correct" },
            {"004", "Account Number check digit is not performed for this bank or branch" },
            {"005", "IBAN structure is correct" },
            {"006", "IBAN does not contain illegal characters" },
            {"007", "Country supports IBAN standard" }
        };
        public static Dictionary<string, string> ErrorsDictionary { get; set; } = new Dictionary<string, string>
        {
            {"301", "API Key is invalid" },
            {"302", "Subscription expired" },
            {"303", "No queries available" },
            {"304", "You have no access to this API" },
            {"305", "IP Address not allowed" }
        };
    }
}
