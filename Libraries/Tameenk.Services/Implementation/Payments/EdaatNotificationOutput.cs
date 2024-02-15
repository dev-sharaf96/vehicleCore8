using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments
{
    public class EdaatNotificationOutput
    {
        public decimal PaymentAmount { set; get; }
        public string PaymentDate { set; get; }
        public string ReferenceId { set; get; }
        public string InvoiceNo { set; get; }
        public string FirstNameAr { set; get; }
        public string FatherNameAr { set; get; }
        public string GrandFatherNameAr { set; get; }
        public string LastNameAr { set; get; }
        public string FullName { set; get; }
        public string Email { set; get; }
        public string NationalID { set; get; }
        public string MobileNo { set; get; }
        public int ErrorCode  { set; get; }
        public string  ErrorDescription { set; get; }

    }
}
