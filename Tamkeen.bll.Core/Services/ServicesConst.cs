using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tamkeen.bll.Services
{
    // it will replace with configration
    public static class ServicesConst
    {
        public static string SaudiPostApiKey = "a2ad46ebf3b642e588d772ea708d6827";

        public static string SadadKeyPassword = "b2b123";
        public static int SadadBillerID = 111;
        public static int SadadExactFlag = 1;  // Exact flag is always 1 for online transactions
        public static string SadadBillslAccountStatus = "ACTIVE";
        // public static string SadadUrl = "https://b2brb.riyadbank.com/soap?service=RB_SADAD_BILL_ONLINE_WS";
        public static string SadadUrl = "https://88.85.225.213/soap?service=RB_SADAD_BILL_ONLINE_WS";
        public static int SadadCustomerAccountNumberLength = 11;
        public static string SadadKeyRelativePath = "~/bin/Services/Payments/Sadad/Key/key_sadad_biller_online_uat.p12";
        
        public const string SenderEmailAddress = "noreply@bcare.com.sa";

        public static readonly Dictionary<char, char> CarPlateCharactersEnglishToArabicMapping;
        public static readonly Dictionary<char, char> CarPlateCharactersArabicToEnglishMapping;

        public static readonly XNamespace SoapEnvelopNameSpace;

        static ServicesConst()
        {
            SoapEnvelopNameSpace = (XNamespace)"http://schemas.xmlsoap.org/soap/envelope/";

            CarPlateCharactersEnglishToArabicMapping = new Dictionary<char, char>()
            {
                {'A', 'أ' },
                {'B', 'ب' },
                {'J', 'ح' },
                {'D', 'د' },
                {'R', 'ر' },
                {'S', 'س' },
                {'X', 'ص' },
                {'T', 'ط' },
                {'E', 'ع' },
                {'G', 'ق' },
                {'K', 'ك' },
                {'L', 'ل' },
                {'Z', 'م' },
                {'N', 'ن' },
                {'H', 'ه' },
                {'U', 'و' },
                {'V', 'ي' },
                {'0', '\u0660' },
                {'1', '\u0661' },
                {'2', '\u0662' },
                {'3', '\u0663' },
                {'4', '\u0664' },
                {'5', '\u0665' },
                {'6', '\u0666' },
                {'7', '\u0667' },
                {'8', '\u0668' },
                {'9', '\u0669' }
            };

            CarPlateCharactersArabicToEnglishMapping = new Dictionary<char, char>()
            {
                {'أ', 'A' },
                {'ب', 'B' },
                {'ح', 'J' },
                {'د', 'D' },
                {'ر', 'R' },
                {'س', 'S' },
                {'ص', 'X' },
                {'ط', 'T' },
                {'ع', 'E' },
                {'ق', 'G' },
                {'ك', 'K' },
                {'ل', 'L' },
                {'م', 'Z' },
                {'ن', 'N' },
                {'ه', 'H' },
                {'و', 'U' },
                {'ي', 'V' },
                {'\u0660', '0' },
                {'\u0661', '1' },
                {'\u0662', '2' },
                {'\u0663', '3' },
                {'\u0664', '4' },
                {'\u0665', '5' },
                {'\u0666', '6' },
                {'\u0667', '7' },
                {'\u0668', '8' },
                {'\u0669', '9' }
            };
        }
    }
}
