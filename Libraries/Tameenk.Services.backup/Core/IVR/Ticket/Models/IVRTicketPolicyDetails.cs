using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    public class IVRTicketPolicyDetails
    {
        #region Policy
        public int PolicyId { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? PolicyIssueDate { get; internal set; }
        public DateTime? PolicyExpiryDate { get; internal set; }
        #endregion

        #region CheckoutDetails
        public string ReferenceId { get; set; }
        public string CheckoutDetailEmail { get; set; }
        public string CheckoutDetailPhone { get; set; }
        public string CheckoutUserId { get; set; }
        public short? SelectedInsuranceTypeCode { get; set; }
        #endregion

        #region Invoice
        public int? InvoiceId { get; set; }
        public int? InvoiceNo { get; set; }
        #endregion

        #region Vehicle
        public Guid VehicleID { get; set; }
        public short? CarPlateNumber { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public byte? PlateTypeCode { get; set; }
        public string VehicleMaker { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public long? VehicleModelCode { get; set; }
        public short? ModelYear { get; set; }
        public string SequenceNumber { get; internal set; }
        public string CustomCardNumber { get; internal set; }
        public int VehicleIdTypeId { get; set; }
        #endregion

        public string ExternalId { get; set; }

        public int? DeductableValue { get; set; }
        public bool? VehicleAgencyRepair { get; set; }
        public int SelectedLanguage { get; set; }
    }
}
