using System;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace TameenkDAL.Models
{
    public class PolicyModel
    {
        #region Policy Master Data.

        public int Id { get; set; }
        public int? InsuranceCompanyID { get; set; }
        public byte StatusCode { get; set; }
        public string NajimStatus { get; set; }
        public string PolicyNo { get; set; }

        public string PolicyStatusKey { get; set; }


        public Nullable<System.DateTime> PolicyIssueDate { get; set; }
        public Nullable<System.DateTime> PolicyEffectiveDate { get; set; }
        public Nullable<System.DateTime> PolicyExpiryDate { get; set; }
        public string CheckOutDetailsId { get; set; }
        public string CheckOutDetailsEmail { set; get; }
        public bool? CheckOutDetailsIsEmailVerified { set; get; }
        public Nullable<System.Guid> PolicyFileId { get; set; }

        #region Car info
        public Vehicle Vehicle { get; set; }
        public string CarPlateNumberAr { get; set; }
        public string CarPlateNumberEn { get; set; }
        public string CarPlateTextAr { get; set; }
        public string CarPlateTextEn { get; set; }
        public string VehiclePlateColor { get; set; }
        #endregion


        #endregion

        #region Another Objects Data

        public string InsuranceCompanyName { set; get; }
        public string PolicyStatusName { set; get; }
        public string VehicleModelName { set; get; }

        #endregion

        public string ExternalId { get; set; }
        public bool IsRenewal { get; set; }
        public VehiclePlateModel VehiclePlate { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
    }
}
