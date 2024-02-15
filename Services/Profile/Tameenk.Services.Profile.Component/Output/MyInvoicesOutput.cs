using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos.Profile;

namespace Tameenk.Services.Profile.Component.Output
{
    public class MyInvoicesOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput = 2,
            Exception = 3,
            FileIdIsNull=4,
            FileNotExist=5,
            FailedToRegenerateInvoiceFile=6,
            InvoiceDataFilePathIsNull=7
        }

        public ErrorCodes ErrorCode { get; set; }

        public string ErrorDescription { get; set; }

        public List<InvoiceModel> InvoicesList { get; set; }
        public int InvoicesTotalCount { get; set; }

        public string Result { get; set; }
    }
}
