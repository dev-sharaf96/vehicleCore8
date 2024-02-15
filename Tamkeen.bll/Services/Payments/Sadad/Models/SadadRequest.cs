using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tamkeen.bll.Services.Sadad.Models
{
    public class SadadRequest
    {
        public int ID { get; set; }
        public int BillerID { get; set; }
        public int ExactFlag { get; set; }
        public string BillsCustomerAccount { get; set; }
        public string BillsCustomerName { get; set; }
        public string BillslAccountStatus { get; set; }
        public decimal BillsAmountDue { get; set; }
        public string BillsOpenDate { get; set; }
        public string BillsDueDate { get; set; }
        public string BillsExpiryDate { get; set; }
        public string BillsCloseDate { get; set; }
        public decimal? BillsMaxAdvanceAmt { get; set; }
        public decimal? BillsMinAdvanceAmt { get; set; }
        public decimal? BillsMinPartialAmt { get; set; }
    }
}