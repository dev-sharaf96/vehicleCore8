using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;
using Tamkeen.bll.Model;
using Tamkeen.bll.Services.Companies.Model;

namespace Tamkeen.bll.Model
{
    public class PolicyRequestMessage
    {
        public string ReferenceId { get; set; }
        public string QuotationNo { get; set; }
        public string ProductId { get; set; }
        public List<Services.Companies.Model.BenefitRequest> Benefits { get; set; }
        public long InsuredId { get; set; }
        public string InsuredFirstNameAr { get; set; }
        public string InsuredMiddleNameAr { get; set; }
        public string InsuredLastNameAr { get; set; }
        public string InsuredFirstNameEn { get; set; }
        public string InsuredMiddleNameEn { get; set; }
        public string InsuredLastNameEn { get; set; }
        public string InsuredMobileNumber { get; set; }
        public string InsuredEmail { get; set; }
        public int InsuredBuildingNo { get; set; }
        public int InsuredZipCode { get; set; }
        public int InsuredAdditionalNumber { get; set; }
        public int InsuredUnitNo { get; set; }
        public string InsuredCity { get; set; }
        public string InsuredCityCode { get; set; }
        public string InsuredDistrict { get; set; }
        public string InsuredStreet { get; set; }
        public string InsuredIBAN { get; set; }
        public string InsuredBankCode { get; set; }
        public int VehicleIdTypeCode { get; set; }
        public int VehiclePlateNumber { get; set; }
        public string VehiclePlateText1 { get; set; }
        public string VehiclePlateText2 { get; set; }
        public string VehiclePlateText3 { get; set; }
        public string VehicleChassisNumber { get; set; }
        public string VehicleOwnerName { get; set; }
        public bool InsuredOrPastThreeYears { get; set; }
        public bool HaveCarLoan { get; set; }
        public bool RenewVehicleReg { get; set; }
        public int PaymentMethodCode { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentBillNumber { get; set; }
        public string PaymentUsername { get; set; }
        public virtual List<PolicyRequestDriver> Drivers { get; set; }
        public AdditionalInfoDetails AdditionalInfoDetails { get; set; }

        [JsonIgnore]
        public Company Company { get; set; }
    }
}