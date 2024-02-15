using System;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.Implementation.Policies
{
    public class CorporatePolicyModel
    {
        #region Policy Master Data.

        public int Id { get; set; }
        public int? InsuranceCompanyID { get; set; }
        public byte StatusCode { get; set; }
        public  string? NajimStatus { get; set; }
        public  string? PolicyNo { get; set; }

        public  string? PolicyStatusKey { get; set; }


        public Nullable<System.DateTime> PolicyIssueDate { get; set; }
        public Nullable<System.DateTime> PolicyEffectiveDate { get; set; }
        public Nullable<System.DateTime> PolicyExpiryDate { get; set; }
        public  string? CheckOutDetailsId { get; set; }
        public  string? CheckOutDetailsEmail { set; get; }
        public bool? CheckOutDetailsIsEmailVerified { set; get; }
        public Nullable<System.Guid> PolicyFileId { get; set; }

        #region Car info
        public Vehicle Vehicle { get; set; }
        public  string? CarPlateNumberAr { get; set; }
        public  string? CarPlateNumberEn { get; set; }
        public  string? CarPlateTextAr { get; set; }
        public  string? CarPlateTextEn { get; set; }
        public  string? VehiclePlateColor { get; set; }
        #endregion


        #endregion

        #region Another Objects Data

        public  string? InsuranceCompanyName { set; get; }
        public  string? PolicyStatusName { set; get; }
        public  string? VehicleModelName { set; get; }

        #endregion

        public  string? ExternalId { get; set; }
        public bool IsRenewal { get; set; }
        //public VehiclePlateModel VehiclePlate { get; set; }
        public  string? SequenceNumber { get; set; }
        public  string? CustomCardNumber { get; set; }

        public  string? IBAN { get; set; }
        public  string? InsuredFullNameAr { get; set; }
        public  string? Phone { get; set; }
        public  string? DriverNIN { get; set; }
        public  string? UserAccountEmail { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal QuotationPrice { get; set; }
        public decimal PolicyPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BenfitsPrice { get; set; }
        public  string? InsuranceType { get; set; }
        public  string? PaymentMethod { get; set; }
    }
}
