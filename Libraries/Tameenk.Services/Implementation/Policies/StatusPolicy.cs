using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.Implementation.Policies
{
    public class StatusPolicy
    {
        /// <summary>
        /// Product type
        /// </summary>
        public ProductType ProductType { get; set; }

        /// <summary>
        /// Policy
        /// </summary>
        public Policy Policy { get; set; }

        /// <summary>
        /// Vehicle
        /// </summary>
        public Vehicle Vehicle { get; set; }
        /// <summary>
        /// CheckOutDetails 
        /// </summary>
       public CheckoutDetail CheckoutDetail { get; set; }

        /// <summary>
        /// Policy processing Queue
        /// </summary>
      public PolicyProcessingQueue PolicyProcessingQueue { get; set; }

        /// <summary>
        /// Invoice
        /// </summary>
       public List<Invoice> Invoices { get; set; }

        /// <summary>
        /// insurance Company
        /// </summary>
        public InsuranceCompany InsuranceCompany { get; set; }

        /// <summary>
        /// Driver
        /// </summary>
        public Driver Driver { get; set; }

        /// <summary>
        /// Policy Status
        /// </summary>
        public PolicyStatus PolicyStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Address Address { get; set; }
    }
}
