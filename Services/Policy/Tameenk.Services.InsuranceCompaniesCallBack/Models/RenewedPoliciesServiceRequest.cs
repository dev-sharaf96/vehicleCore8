using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Models
{
    public class RenewedPoliciesServiceRequest
    {
        public string ReferenceId {get;set;}
        public string OldPolicyNo {get;set;}
        public string NewPolicyNo {get;set;}
        public int ProductTypeCode { get; set; }
        public string RenewalDate { get; set; }
        public decimal PolicyAmount { get; set; }
        public decimal PolicyVAT { get; set; }
        public decimal PolicyTotalAmount { get; set; }
        public decimal PolicyRenewalCommission { get; set; }
    }
}