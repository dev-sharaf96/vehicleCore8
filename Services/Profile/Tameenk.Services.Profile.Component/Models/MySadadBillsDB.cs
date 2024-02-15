using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component.Models
{
    public class MySadadBillsDB
    {
        public long Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ReferenceId { get; set; }
        public int? BillStatusId { get; set; }
        public string BillStatus { get; set; }
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDueDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string InsuranceCompanyNameAr { get; set; }
        public string InsuranceCompanyNameEn { get; set; }

        #region Vehicle Data
        public System.Guid VehicleId { get; set; }
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
        #endregion

        public double RemainingTimeToExpireInSeconds { get; set; }
        public string CarImage { get; set; }
        public VehiclePlateModel VehiclePlate { get; set; }
        public string SadadBillNo { get; set; }
    }
}
