using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.UserTicket.Components
{
    public class UserInvoiceDetailsDBModel
    {
        public int InvoiceNo { get; set; }
        public string ReferenceId { get; set; }

        #region CheckOutDetails
        public string CheckoutDetailsEmail { get; set; }
        public string CheckoutDetailsPhone { get; set; }
        #endregion

        #region Insurance Company
        public int? InsuranceCompanyId { get; set; }
        public string InsuranceCompanyNameAr { get; set; }
        public string InsuranceCompanyNameEn { get; set; }
        #endregion

        #region Vehicle
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
        #endregion

        public string NIN { get; set; }
    }
}
