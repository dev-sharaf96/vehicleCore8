using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("SamaReport")]

    public class SamaReportModel
    {
        /// <summary>
        /// Reference No
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>
        [JsonProperty("invoiceDate")]
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// Invoice No
        /// </summary>
        [JsonProperty("bcareInvoiceNumber")]
        public int? BcareInvoiceNumber { get; set; }

        /// <summary>
        /// Scheme
        /// </summary>
        [JsonProperty("scheme")]
        public string Scheme { get; set; }


        /// <summary>
        /// Main driver First Name + Second Name + Last Name
        /// </summary>
        [JsonProperty("policyHolder")]
        public string PolicyHolder { get; set; }

        /// <summary>
        /// Checkout Phone
        /// </summary>
        [JsonProperty("mob")]
        public string Mob { get; set; }

        /// <summary>
        /// checkout Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// company NameAR
        /// </summary>
        [JsonProperty("insurer")]
        public string Insurer { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Payfort As Payment Method
        /// </summary>
        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// CardNumber from PayfortPaymentResponse 
        /// </summary>
        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }

        /// <summary>
        /// insurance product Type
        /// </summary>
        [JsonProperty("insuranceProduct")]
        public string InsuranceProduct { get; set; }

        /// <summary>
        /// invoice Extra Premium Price
        /// </summary>
        [JsonProperty("extraPremiumPrice")]
        public decimal? ExtraPremiumPrice { get; set; }


        /// <summary>
        /// invoice Vat
        /// </summary>
        [JsonProperty("vat")]
        public decimal? Vat { get; set; }


        [JsonProperty("insuranceCompanyId")]
        public int InsuranceCompanyId { get; set; }

        [JsonProperty("insuranceCompanyName")]
        public string InsuranceCompanyName { get; set; }

        [JsonProperty("driverBirthDate")]
        public DateTime DriverBirthDate { get; set; }

        [JsonProperty("productPrice")]
        public decimal? ProductPrice { get; set; }

        [JsonProperty("totalPrice")]
        public decimal? TotalPrice { get; set; }

        [JsonProperty("discount")]
        public decimal? Discount { get; set; }

        [JsonProperty("fees")]
        public decimal? Fees { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        public decimal? TotalBCareFees { get; set; }
        public decimal? TotalBCareCommission { get; set; }
        public decimal? TotalCompanyAmount { get; set; }

        public string NationalId { get; set; }
        public string IBAN { get; set; }
        public string AccountMob { get; set; }
        public string AccountEmail{ get; set; }
        public string AdditionalDriverOneNIN { get; set; }
        public string AdditionalDriverTwoNIN { get; set; }
        public string AdditionalDriverThreeNIN { get; set; }
        public string AdditionalDriverFourNIN { get; set; }
        public string AdditionalDriverOneName { get; set; }
        public string AdditionalDriverTwoName { get; set; }
        public string AdditionalDriverThreeName { get; set; }
        public string AdditionalDriverFourName { get; set; }

        public int? NajmNcdFreeYears { get; set; }
        public int? NoOfAccident { get; set; }

        public string ChassisNumber { get; set; }

        public string EnglishBankName { get; set; }
        public string ArabicBankName { get; set; }
        public string CarOwnerNIN { get; set; }
        public string InsuredNationalId { get; set; }
        public byte VehicleBodyCode { get; set; }
        public string ArabicVehicleBody { get; set; }
        public string EnglishVehicleBody { get; set; }
        public Guid? MerchantTransactionId { get; set; }
        public DateTime? PolicyIssueDate { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string DiscountCode { get; set; }
        public decimal? TotalBCareDiscount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? BasicPrice { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? ActualBankFees { get; set; }
        public string FeesCalculationDetails { get; set; }
    }
}