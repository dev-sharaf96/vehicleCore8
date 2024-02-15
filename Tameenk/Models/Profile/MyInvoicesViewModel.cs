using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos.Profile;

namespace Tameenk.Models
{
    public class MyInvoicesViewModel
    {
        public List<InvoiceModel> InvoicesList { get; internal set; }
        public int InvoicesTotalCount { get; internal set; }
        public int CurrentPage { get; set; }
        public string Lang { get; set; }
    }
}