using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Domain.Entities.Quotations;
namespace Tameenk.Core.Domain.Entities
{
    public class CheckoutDetail : BaseEntity
    {
        public CheckoutDetail()
        {
            CheckoutAdditionalDrivers = new HashSet<CheckoutAdditionalDriver>();
            PayfortPaymentRequests = new HashSet<PayfortPaymentRequest>();
            Policies = new HashSet<Policy>();
            OrderItems = new HashSet<OrderItem>();
           // Drivers = new HashSet<Driver>();

        }

        public string ReferenceId { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }

        public LanguageTwoLetterIsoCode? SelectedLanguage { get; set; }


        public int? BankCodeId { get; set; }


        public string IBAN { get; set; }

        public int? PaymentMethodId { get; set; }

        public int? ImageRightId { get; set; }

        public int? ImageLeftId { get; set; }

        public int? ImageFrontId { get; set; }

        public int? ImageBackId { get; set; }

        public int? ImageBodyId { get; set; }
        
        public string UserId { get; set; }

        public Guid VehicleId { get; set; }

        public Guid? MainDriverId { get; set; }

        public int? PolicyStatusId { get; set; }
        public string Channel { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public InsuranceCompany InsuranceCompany { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Guid? SelectedProductId { get; set; }

        public short? SelectedInsuranceTypeCode { get; set; }
        public Guid? AdditionalDriverIdOne { get; set; }
        public Guid? AdditionalDriverIdTwo { get; set; }
        public Guid? AdditionalDriverIdThree{ get; set; }
        public Guid? AdditionalDriverIdFour { get; set; }
        public AspNetUser AspNetUser { get; set; }

        public ICollection<CheckoutAdditionalDriver> CheckoutAdditionalDrivers { get; set; }

        public CheckoutCarImage ImageBack { get; set; }

        public CheckoutCarImage ImageBody { get; set; }

        public CheckoutCarImage ImageFront { get; set; }

        public CheckoutCarImage ImageLeft { get; set; }

        public CheckoutCarImage ImageRight { get; set; }

        public Driver Driver { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PolicyStatus PolicyStatus { get; set; }

        //public Product Product { get; set; }

        public ProductType ProductType { get; set; }

        public Vehicle Vehicle { get; set; }

        public ICollection<PayfortPaymentRequest> PayfortPaymentRequests { get; set; }
        public ICollection<HyperpayRequest> HyperpayRequests { get; set; }

        public ICollection<Policy> Policies { get; set; }
        public AdditionalInfo AdditionalInfo { get; set; }

        public BankCode BankCode { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public bool IsCancelled { get; set; } = false;
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
        public bool? IsEmailVerified { get; set; } = false;
        public int? BankId { get; set; }
        public int? CorporateAccountId { get; set; }
        public Guid? MerchantTransactionId { get; set; }
        public bool? IsAutoleasingWallet { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public int? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string ODReference { get; set; }        public string MainPolicyReferenceId { get; set; }

        public bool Isleasing { get; set; }
        public int InsuredId { get; set; }
        public Insured Insured { get; set; }

        public string ExternalId { get; set; }
        public CheckoutProviderServicesCodes? ProviderServiceId { get; set; }
      //  public ICollection<Driver> Drivers { get; set; }

    }
}
