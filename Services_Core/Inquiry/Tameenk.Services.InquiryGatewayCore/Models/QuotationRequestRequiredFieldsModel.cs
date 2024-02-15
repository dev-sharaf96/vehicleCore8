using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Services.Extensions;
using Tameenk.Services.InquiryGateway.CustomAttributes;

namespace Tameenk.Services.InquiryGateway.Models
{
    public class QuotationRequestRequiredFieldsModel
    {
        public QuotationRequestRequiredFieldsModel()
        {

        }


        //[Required]
        [FieldDetail(Enums.FieldDataType.String, controlType: Enums.ControlType.Textbox)]
        public string MainDriverEnglishFirstName { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.String, controlType: Enums.ControlType.Textbox)]
        public string MainDriverEnglishLastName { get; set; }


        //[Required]
        [FieldDetail(Enums.FieldDataType.String, controlType: Enums.ControlType.Textbox)]
        public string MainDriverLastName { get; set; }


        //[Required]
        [FieldDetail(Enums.FieldDataType.String, controlType: Enums.ControlType.Textbox)]
        public string MainDriverFirstName { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.DateTime, controlType: Enums.ControlType.DatePicker)]
        public DateTime MainDriverDateOfBirthG { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.Short, "Cities", controlType: Enums.ControlType.Dropdown)]
        public short? MainDriverNationalityCode { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.DateTime, controlType: Enums.ControlType.DatePicker)]
        public string MainDriverDateOfBirthH { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.String, "Cities", controlType: Enums.ControlType.Dropdown)]
        public string MainDriverIdIssuePlace { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.Int, "SocialStatus", controlType: Enums.ControlType.Dropdown)]
        public int? MainDriverSocialStatusCode { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.Int, "Occupations", controlType: Enums.ControlType.Dropdown)]
        public int? MainDriverOccupationCode { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.Int, "Genders", controlType: Enums.ControlType.Dropdown)]
        public int MainDriverGenderCode { get; set; }



        [Required]
        [FieldDetail(Enums.FieldDataType.DateTime, controlType: Enums.ControlType.DatePicker)]
        public string VehicleLicenseExpiry { get; set; }



        [Required]
        [FieldDetail(Enums.FieldDataType.String, controlType: Enums.ControlType.Textbox)]
        public string VehicleMajorColor { get; set; }


        //[Required]
        [FieldDetail(Enums.FieldDataType.Short, "ModelYears", controlType: Enums.ControlType.Dropdown)]
        public short? VehicleModelYear { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.Short, "VehiclePlateTypes", controlType: Enums.ControlType.Dropdown)]
        public short? VehiclePlateTypeCode { get; set; }

        //[Required]
        [FieldDetail(Enums.FieldDataType.String, "Cities", controlType: Enums.ControlType.Dropdown)]
        public string VehicleRegisterationPlace { get; set; }

        [Required]
        [FieldDetail(Enums.FieldDataType.Short, "VehicleBodyTypes", controlType: Enums.ControlType.Dropdown)]
        public short? VehicleBodyCode { get; set; }


        [Required]
        [FieldDetail(Enums.FieldDataType.Int, "VehicleLoads", controlType: Enums.ControlType.Dropdown)]
        public int? VehicleLoad { get; set; }

        [Required]
        [FieldDetail(Enums.FieldDataType.String, "VehicleMakers", controlType: Enums.ControlType.Dropdown)]
        public string VehicleMaker { get;
            set; }

        [Required]
        [FieldDetail(Enums.FieldDataType.String, "VehicleModels", controlType: Enums.ControlType.Dropdown)]
        public string VehicleModel { get;
            set; }


        [Required]
        [FieldDetail(Enums.FieldDataType.String, controlType: Enums.ControlType.Textbox)]
        public string VehicleChassisNumber { get; set; }


        [Required]
        [FieldDetail(Enums.FieldDataType.String, "VehicleMakers", controlType: Enums.ControlType.Dropdown)]
        public int? VehicleMakerCode {
            get;
            set; }

        [Required]
        [FieldDetail(Enums.FieldDataType.String, "VehicleModels", controlType: Enums.ControlType.Dropdown)]
        public int? VehicleModelCode { get;
            set; }


        public List<IdNamePairModel> Cities { get; set; }
        public List<IdNamePairModel> SocialStatus { get; set; }
        public List<IdNamePairModel> Occupations { get; set; }
        public List<IdNamePairModel> Genders { get; set; }
        public List<IdNamePairModel> ModelYears { get; set; }
        public List<IdNamePairModel> VehiclePlateTypes { get; set; }
        public List<IdNamePairModel> VehicleBodyTypes { get; set; }
        public List<IdNamePairModel> VehicleMakers { get; set; }
        public List<IdNamePairModel> VehicleModels { get; set; }
        public List<IdNamePairModel> VehicleLoads { get; set; }


    }
}