using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.UserTicket.Components
{
    public class UserTicketsDBModel
    {
        public int TicketId { get; set; }
        public int TicketTypeId { get; set; }
        public string UserNotes { get; set; }
        public string PolicyNo { get; set; }
        public int? InvoiceNo { get; set; }
        public string AdminReply { get; set; }

        #region Status
        public int StatusId { get; set; }
        public string StatusNameAr { get; set; }
        public string StatusNameEn { get; set; }
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

    }
}
