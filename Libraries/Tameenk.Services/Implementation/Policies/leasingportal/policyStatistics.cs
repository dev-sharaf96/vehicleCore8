using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tameenk.Core.Utilities;

namespace Tameenk.Services.Implementation.Policies
{
  public  class policyStatistics
    {
        [JsonIgnore]
        public short? VehicleMakerCode { get; set; }
        
        [JsonIgnore]
        public string CarPlateText1 { get; set; }
        
        [JsonIgnore]
        public string CarPlateText2 { get; set; }

        [JsonIgnore]
        public string CarPlateText3 { get; set; }

        [JsonProperty("vehicleMaker")]
        public string VehicleMaker { get; set; }
               
        
        [JsonProperty("carPlateNumber")]
        public short ? CarPlateNumber { get; set; }
        
        [JsonProperty("chassisNumber")]
        public string ChassisNumber { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        [JsonProperty("parentExternalId")]
        public string ParentExternalId { get; set; }

        [JsonProperty("makerCodeFormatted")]
        public string MakerCodeFormatted
        {
            get { return VehicleMakerCode.Value.ToString("0000") ?? default(string); }
        }

        private string _carPlateTextAr;
        [JsonProperty("carPlateTextAr")]
        public string CarPlateTextAr 
        {
            get
            {
                if (string.IsNullOrEmpty(_carPlateTextAr))
                    handleCarPlateText();
                return _carPlateTextAr;
            }
        }

        private string _carPlateTextEn;
        [JsonProperty("carPlateTextEn")]
        public string CarPlateTextEn 
        {
            get
            {
                if (string.IsNullOrEmpty(_carPlateTextEn))
                    handleCarPlateText();
                return _carPlateTextEn;
            }
        }


        [JsonIgnore]
        public string ConvertedCarPlateText1 { get; set; }

        [JsonIgnore]
        public string ConvertedCarPlateText2 { get; set; }

        [JsonIgnore]
        public string ConvertedCarPlateText3 { get; set; }

        [JsonIgnore]
        public short? ConvertedCarPlateNumber { get; set; }

        [JsonIgnore]
        public string SequenceNumber { get; set; }

        [JsonIgnore]
        public string CustomCardNumber { get; set; }

        [JsonIgnore]
        public int VehicleIdTypeId { get; set; }

        #region Private Methods

        private void handleCarPlateText()
        {
            string carPlateText = CarPlateText1 + " " + CarPlateText2 + " " + CarPlateText3;
            if (Regex.IsMatch(carPlateText, "[\u0600-\u06ff\\s]+"))
            {
                _carPlateTextAr = carPlateText;
                _carPlateTextEn = CarPlateUtils.ConvertCarPlateTextFromArabicToEnglish(_carPlateTextAr, false);
                _carPlateTextAr = CarPlateText3 + " " + CarPlateText2 + " " + CarPlateText1;
            }
            else
            {
                _carPlateTextEn = carPlateText;
                _carPlateTextAr = CarPlateUtils.ConvertCarPlateTextFromEnglishToArabic(_carPlateTextEn, false);
            }
        }

        #endregion
    }
}
