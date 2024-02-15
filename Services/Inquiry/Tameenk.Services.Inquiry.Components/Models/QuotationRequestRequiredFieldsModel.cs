using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Models;

namespace Tameenk.Services.Inquiry.Components
{
    public class QuotationRequestRequiredFieldsModel
    {
        public QuotationRequestRequiredFieldsModel()
        {

        }


        //[Required]
        [FieldDetail(FieldDataType.String, controlType: ControlType.Textbox)]
        public string MainDriverEnglishFirstName { get; set; }

        //[Required]
        [FieldDetail(FieldDataType.String, controlType: ControlType.Textbox)]
        public string MainDriverEnglishLastName { get; set; }


        //[Required]
        [FieldDetail(FieldDataType.String, controlType: ControlType.Textbox)]
        public string MainDriverLastName { get; set; }


        //[Required]
        [FieldDetail(FieldDataType.String, controlType: ControlType.Textbox)]
        public string MainDriverFirstName { get; set; }

        //[Required]
        [FieldDetail(FieldDataType.DateTime, controlType: ControlType.DatePicker)]
        public DateTime MainDriverDateOfBirthG { get; set; }

        //[Required]
        [FieldDetail(FieldDataType.Short, "Cities", controlType: ControlType.Dropdown)]
        public short? MainDriverNationalityCode { get; set; }

        //[Required]
        [FieldDetail(FieldDataType.DateTime, controlType: ControlType.DatePicker)]
        public string MainDriverDateOfBirthH { get; set; }

        //[Required]
        [FieldDetail(FieldDataType.String, "Cities", controlType: ControlType.Dropdown)]
        public string MainDriverIdIssuePlace { get; set; }

        [Required]
        [FieldDetail(FieldDataType.Int, "SocialStatus", controlType: ControlType.Dropdown)]
        public int? MainDriverSocialStatusCode { get; set; }

        [Required]
        [FieldDetail(FieldDataType.Int, "AdditionalDriverOneSocialStatus", controlType: ControlType.Dropdown)]
        public int? AdditionalDriverOneSocialStatusCode { get; set; }
        [Required]
        [FieldDetail(FieldDataType.Int, "AdditionalDriverTwoSocialStatus", controlType: ControlType.Dropdown)]
        public int? AdditionalDriverTwoSocialStatusCode { get; set; }
        public string MainDriverNationalId { get; set; }
        public string AdditionalDriverOneNationalId { get; set; }
        public string AdditionalDriverTwoNationalId { get; set; }
        //[Required]
        [FieldDetail(FieldDataType.Int, "Occupations", controlType: ControlType.Dropdown)]
        public int? MainDriverOccupationCode { get; set; }

        //[Required]
        [FieldDetail(FieldDataType.Int, "Genders", controlType: ControlType.Dropdown)]
        public int MainDriverGenderCode { get; set; }



        [Required]
        [FieldDetail(FieldDataType.DateTime, controlType: ControlType.DatePicker)]
        public string VehicleLicenseExpiry { get; set; }


        [Required]
        [FieldDetail(FieldDataType.String, "VehicleColors", controlType: ControlType.Dropdown)]
        public string VehicleMajorColor { get; set; }


        //[Required]
        [FieldDetail(FieldDataType.Short, "ModelYears", controlType: ControlType.Dropdown)]
        public short? VehicleModelYear { get; set; }

        [Required]
        [FieldDetail(FieldDataType.Short, "VehiclePlateTypes", controlType: ControlType.Dropdown)]
        public short? VehiclePlateTypeCode { get; set; }

        //[Required]
        [FieldDetail(FieldDataType.String, "Cities", controlType: ControlType.Dropdown)]
        public string VehicleRegisterationPlace { get; set; }

        [Required]
        [FieldDetail(FieldDataType.Short, "VehicleBodyTypes", controlType: ControlType.Dropdown)]
        public short? VehicleBodyCode { get; set; }


        [Required]
        [FieldDetail(FieldDataType.Int, "VehicleLoads", controlType: ControlType.Dropdown)]
        public int? VehicleLoad { get; set; }

        [Required]
        [FieldDetail(FieldDataType.String, "VehicleMakers", controlType: ControlType.Dropdown)]
        public string VehicleMaker { get; set; }

        [Required]
        [FieldDetail(FieldDataType.String, "VehicleModels", controlType: ControlType.Dropdown)]
        public string VehicleModel { get; set; }


        [Required]
        [FieldDetail(FieldDataType.String, controlType: ControlType.Textbox)]
        public string VehicleChassisNumber { get; set; }


        [Required]
        [FieldDetail(FieldDataType.String, "VehicleMakers", controlType: ControlType.Dropdown)]
        public int? VehicleMakerCode { get; set; }

        [Required]
        [FieldDetail(FieldDataType.String, "VehicleModels", controlType: ControlType.Dropdown)]
        public int? VehicleModelCode { get; set; }


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
        public List<IdNamePairModel> VehicleColors { get; set; }
        public List<IdNamePairModel> AdditionalDriverOneSocialStatus { get; set; }
        public List<IdNamePairModel> AdditionalDriverTwoSocialStatus { get; set; }
    }
}