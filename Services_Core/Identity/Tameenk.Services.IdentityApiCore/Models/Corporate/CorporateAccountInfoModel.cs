using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.IdentityApi.Models
{
    public class CorporateAccountInfoModel
    {
        public string NameAr { get; internal set; }
        public string NameEn { get; internal set; }
        public decimal? Balance { get; internal set; }
    }
}