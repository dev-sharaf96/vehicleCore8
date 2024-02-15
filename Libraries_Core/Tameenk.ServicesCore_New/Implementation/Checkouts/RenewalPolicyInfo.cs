using System;

namespace Tameenk.Services.Implementation.Checkouts
{
    public class RenewalPolicyInfo
    {
        public  string? Phone { get; set; }

        public int? SelectedLanguage { get; set; }
        public  string? SequenceNumber { get; set; }
        public  string? CustomCardNumber { get; set; }
        public  string? VehicleModel { get; set; }
        public short? CarPlateNumber { get; set; }
        public  string? CarPlateText1 { get; set; }
        public  string? CarPlateText2 { get; set; }
        public  string? CarPlateText3 { get; set; }
        public  string? FirstName { get; set; }
        public  string? EnglishFirstName { get; set; }
        public  string? ReferenceId { get; set; }
        public  string? ExternalId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public  string? MakerDescAr { get; set; }
        public  string? MakerDescEn { get; set; }
        public  string? ModelDescAr { get; set; }
        public  string? ModelDescEn { get; set; }
        public  string? UserId { get; set; }
        public short? ModelYear { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public  string? InsuranceCompanyName { get; set; }
        public  string? Channel { get; set; }
        public Guid VehicleId { get; set; }
        public  string? PolicyNo { get; set; }
        public  string? CarOwnerNIN { get; set; }
        public bool OwnerTransfer { get; set; }
        public  string? NationalId { get; set; }
        public  string? Email { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
