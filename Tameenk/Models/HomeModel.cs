using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class HomeModel
    {
        public string QuotationRequestExternalId { get; set; }
        public bool IsEditRequest { get; set; }
        public bool IsRenual { get; set; }
        public string ReferenceId { get; set; }
        public bool IsCustomCard { get; set; }
    }
}