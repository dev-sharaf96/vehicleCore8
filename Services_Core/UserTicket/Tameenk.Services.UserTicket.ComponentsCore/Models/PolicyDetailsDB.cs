using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.UserTicket.Components
{
    public class PolicyDetailsDB
    {
        #region Policy
        public int Id { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? PolicyIssueDate { get; internal set; }
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
        #endregion

        #region Insurance Company
        public string InsuranceCompanyNameAR { get; set; }
        public string InsuranceCompanyNameEN { get; set; }
        #endregion

        #region Driver
        public string NIN { get; set; }
        #endregion

        #region CheckoutDetails
        public string ReferenceId { get; set; }
        public string CheckoutDetailEmail { get; set; }
        public string CheckoutDetailPhone { get; set; }
        #endregion

        #region Invoice
        public int? InvoiceId { get; set; }
        public int? InvoiceNo { get; set; }
        #endregion
    }
}
