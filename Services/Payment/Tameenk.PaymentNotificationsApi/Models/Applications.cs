using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Models
{
    public class Applications
    {

        public const string Wafier = "01";
        public const string Waffer = "02";
        public const string tameenk = "03";

        public static string GetName(string code)
        {
            switch (code)
            {
                case "01":
                    return "wafier";
                case "02":
                    return "waffer";
                case "03":
                    return "tameenk";
                default:
                    return "wafier";
            }
        }
    }
}