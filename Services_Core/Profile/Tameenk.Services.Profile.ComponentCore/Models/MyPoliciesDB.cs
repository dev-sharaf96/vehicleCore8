using System;

namespace Tameenk.Services.Profile.Component.Models
{
    public class MyPoliciesDB
    {
        #region Policy Master Data.
        public int Id { get; set; }
        public string CheckOutDetailsId { get; set; }
        public string Name { get; set; }
        public int? InsuranceCompanyID { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public Guid? PolicyFileId { get; set; }
        public DateTime? PolicyIssueDate { get; set; }
        public string PolicyNo { get; set; }
        public byte StatusCode { get; set; }
        public string CheckOutDetailsEmail { set; get; }
        public bool? CheckOutDetailsIsEmailVerified { set; get; }
        public string PolicyStatusKey { get; set; }
        public string NajmStatusNameAr { get; set; }
        public string NajmStatusNameEn { get; set; }
        public string InsuranceCompanyNameAR { set; get; }
        public string InsuranceCompanyNameEN { set; get; }
        public string PolicyStatusNameAr { set; get; }
        public string PolicyStatusNameEn { set; get; }
        public string ExternalId { get; set; }
        #endregion

        #region Car info
        public System.Guid ID { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleMaker { get; set; }
        public short? ModelYear { get; set; }
        public string VehicleModel { get; set; }
        public long? VehicleModelCode { get; set; }
        public string RegisterationPlace { get; set; }

        public byte? PlateTypeCode { get; set; }
        public short? CarPlateNumber { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        #endregion

        public string IBAN { get; set; }
        public string InsuredFullNameAr { get; set; }
        public string Phone { get; set; }
        public string DriverNIN { get; set; }
        public string CorporateUserEmail { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal QuotationPrice { get; set; }
        public decimal PolicyPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BenfitsPrice { get; set; }
        public string InsuranceType { get; set; }
    }
}