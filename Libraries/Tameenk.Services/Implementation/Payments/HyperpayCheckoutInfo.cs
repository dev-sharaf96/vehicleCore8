using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Payments.Sadad;
using Tameenk.Integration.Dto;
using Tameenk.Integration.Dto.Payment;

namespace Tameenk.Services
{
    public class HyperpayCheckoutInfo
    {
        public string ReferenceId { get; set; }
        public string Email { get; set; }
        public int? PolicyStatusId { get; set; }
        public string Channel { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public bool? IsEmailVerified { get; set; } = false;
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal OrderItemPrice { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public string NationalId { get; set; }
        public Guid? MerchantTransactionId { get; set; }
    }
}
