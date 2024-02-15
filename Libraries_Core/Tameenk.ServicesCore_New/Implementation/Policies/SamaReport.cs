using System;

namespace Tameenk.Services.Implementation.Policies
{
    public class SamaReport
    {
        public  string? ReferenceNo { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// Invoice No
        /// </summary>
        public int? BcareInvoiceNumber { get; set; }

        /// <summary>
        /// Scheme
        /// </summary>
        public  string? Scheme { get; set; }


        /// <summary>
        /// Main driver First Name + Second Name + Last Name
        /// </summary>
        public  string? PolicyHolder { get; set; }

        /// <summary>
        /// Checkout Phone
        /// </summary>
        public  string? Mob { get; set; }

        /// <summary>
        /// checkout Email
        /// </summary>
        public  string? Email { get; set; }

        /// <summary>
        /// company NameAR
        /// </summary>
        public  string? Insurer { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        public  string? PolicyNo { get; set; }

        /// <summary>
        /// Payfort As Payment Method
        /// </summary>
        public  string? PaymentMethod { get; set; }

        /// <summary>
        /// CardNumber from PayfortPaymentResponse 
        /// </summary>
        public  string? CardNumber { get; set; }

        /// <summary>
        /// insurance product Type
        /// </summary>
        public  string? InsuranceProduct { get; set; }

        /// <summary>
        /// invoice Extra Premium Price
        /// </summary>
        public decimal? ExtraPremiumPrice { get; set; }


        /// <summary>
        /// invoice Vat
        /// </summary>
        public decimal? Vat { get; set; }

        public decimal? TotalBCareFees { get; set; }
        public decimal? TotalBCareCommission { get; set; }
        public decimal? TotalCompanyAmount { get; set; }

        public DateTime DriverBirthDate { get; set; }
        public  string? NationalId { get; set; }
        public  string? IBAN { get; set; }
        public  string? AccountMob { get; set; }
        public  string? AccountEmail { get; set; }
      
        public decimal? ProductPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? Discount { get; set; }

        public decimal? Fees { get; set; }
        public  string? Channel { get; set; }
        public  string? AdditionalDriverOneNIN { get; set; }
        public  string? AdditionalDriverTwoNIN { get; set; }
        public  string? AdditionalDriverThreeNIN { get; set; }
        public  string? AdditionalDriverFourNIN { get; set; }
        public  string? AdditionalDriverOneName { get; set; }
        public  string? AdditionalDriverTwoName { get; set; }
        public  string? AdditionalDriverThreeName { get; set; }
        public  string? AdditionalDriverFourName { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public int? NoOfAccident { get; set; }
        public  string? ChassisNumber { get; set; }

        public  string? EnglishBankName { get; set; }
        public  string? ArabicBankName { get; set; }
        public  string? CarOwnerNIN { get; set; }
        public  string? InsuredNationalId { get; set; }
        public byte VehicleBodyCode { get; set; }
        public  string? ArabicVehicleBody { get; set; }
        public  string? EnglishVehicleBody { get; set; }
        public Guid? MerchantTransactionId { get; set; }

        public DateTime? PolicyIssueDate { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public  string? DiscountCode { get; set; }
        public decimal? TotalBCareDiscount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? BasicPrice { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? ActualBankFees { get; set; }
        public  string? FeesCalculationDetails { get; set; }
    }
}
