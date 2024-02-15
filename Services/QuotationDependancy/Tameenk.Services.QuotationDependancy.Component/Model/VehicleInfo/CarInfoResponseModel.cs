using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tameenk.Services.QuotationDependancy.Component
{
    public class CarInfoResponseModel
    {
        [JsonIgnore]
        public int ErrorCode { get; set; }
        [JsonIgnore]
        public string ErrorMsg { get; set; }

        public bool IsRenewal { get; set; }
        //public string NCDFreeYears { get; set; }
        public string NCDFreeYearsAr { get; set; }
        public string NCDFreeYearsEn { get; set; }
        public string QtRqstExtrnlId { get; set; }
        public string RenewalReferenceId { get; set; }
        public string FormatedMakerCode { get; set; }
        public string Maker { get; set; }
        public short? MakerCode { get; set; }
        public string Model { get; set; }
        public short? ModelYear { get; set; }
        public string PlateColor { get; set; }
        public byte? PlateTypeCode { get; set; }
        public bool VehicleAgencyRepair { get; set; }
        public string CustomCardNumber { get; set; }
        //public int TypeOfInsurance { get; set; }

        public CarPlateInfo CarPlate { get; set; }
    }

    public class CarPlateInfo
    {
        public readonly string PlateText1;
        public readonly string PlateText2;
        public readonly string PlateText3;
        public readonly int PlateNumber;
        public readonly string CarPlateNumberEn;
        public readonly string CarPlateNumberAr;

        private string _carPlateTextAr;
        public string CarPlateTextAr
        {
            get
            {
                if (string.IsNullOrEmpty(_carPlateTextAr))
                {
                    handleCarPlateText();
                }
                return _carPlateTextAr;
            }
        }

        private string _carPlateTextEn;
        public string CarPlateTextEn
        {
            get
            {
                if (string.IsNullOrEmpty(_carPlateTextEn))
                {
                    handleCarPlateText();
                }
                return _carPlateTextEn;
            }
        }

        public CarPlateInfo(string plateText1, string plateText2, string plateText3, int plateNumber)
        {
            PlateText1 = plateText1;
            PlateText2 = plateText2;
            PlateText3 = plateText3;
            PlateNumber = plateNumber;
            CarPlateNumberEn = plateNumber.ToString();
            CarPlateNumberAr = CarPlateUtils.ConvertCarPlateTextFromEnglishToArabic(CarPlateNumberEn, false);

            handleCarPlateText();
        }

        private void handleCarPlateText()
        {
            string carPlateText = PlateText1 + " " + PlateText2 + " " + PlateText3;
            if (Regex.IsMatch(carPlateText, "[\u0600-\u06ff\\s]+"))
            {
                _carPlateTextAr = carPlateText;
                _carPlateTextEn = CarPlateUtils.ConvertCarPlateTextFromArabicToEnglish(_carPlateTextAr, false);
                _carPlateTextAr = PlateText3 + " " + PlateText2 + " " + PlateText1;
            }
            else
            {
                _carPlateTextEn = carPlateText;
                _carPlateTextAr = CarPlateUtils.ConvertCarPlateTextFromEnglishToArabic(_carPlateTextEn, false);
            }
        }
    }

    public class CarPlateUtils
    {
        public static readonly Dictionary<char, char> CarPlateCharactersEnglishToArabicMapping;
        public static readonly Dictionary<char, char> CarPlateCharactersArabicToEnglishMapping;

        static CarPlateUtils()
        {
            CarPlateCharactersEnglishToArabicMapping = new Dictionary<char, char>()
            {
                {'A', 'أ' },
                {'B', 'ب' },
                {'J', 'ح' },
                {'D', 'د' },
                {'R', 'ر' },
                {'S', 'س' },
                {'X', 'ص' },
                {'T', 'ط' },
                {'E', 'ع' },
                {'G', 'ق' },
                {'K', 'ك' },
                {'L', 'ل' },
                {'Z', 'م' },
                {'N', 'ن' },
                {'H', 'ه' },
                {'U', 'و' },
                {'V', 'ي' },
                {'0', '\u0660' },
                {'1', '\u0661' },
                {'2', '\u0662' },
                {'3', '\u0663' },
                {'4', '\u0664' },
                {'5', '\u0665' },
                {'6', '\u0666' },
                {'7', '\u0667' },
                {'8', '\u0668' },
                {'9', '\u0669' }
            };

            CarPlateCharactersArabicToEnglishMapping = new Dictionary<char, char>()
            {
                {'أ', 'A' },
                {'ب', 'B' },
                {'ح', 'J' },
                {'د', 'D' },
                {'ر', 'R' },
                {'س', 'S' },
                {'ص', 'X' },
                {'ط', 'T' },
                {'ع', 'E' },
                {'ق', 'G' },
                {'ك', 'K' },
                {'ل', 'L' },
                {'م', 'Z' },
                {'ن', 'N' },
                {'ه', 'H' },
                {'و', 'U' },
                {'ي', 'V' },
                {'\u0660', '0' },
                {'\u0661', '1' },
                {'\u0662', '2' },
                {'\u0663', '3' },
                {'\u0664', '4' },
                {'\u0665', '5' },
                {'\u0666', '6' },
                {'\u0667', '7' },
                {'\u0668', '8' },
                {'\u0669', '9' }
            };
        }

        public static string ConvertCarPlateTextFromEnglishToArabic(string englishPlateText, bool isReversed)
        {
            StringBuilder arabicResult = new StringBuilder();
            for (int i = 0; i < englishPlateText.Length; i++)
            {
                char letter = englishPlateText[i];
                if (letter == ' ')
                {
                    arabicResult.Append(letter);
                }
                else
                {
                    var arabicLetter = letter;
                    if (CarPlateCharactersEnglishToArabicMapping.ContainsKey(letter))
                        arabicLetter = CarPlateCharactersEnglishToArabicMapping[letter];

                    arabicResult.Append(arabicLetter);
                }
            }

            if (isReversed)
                return string.Join("", arabicResult.ToString().Reverse());
            else
                return arabicResult.ToString();
        }

        public static string ConvertCarPlateTextFromArabicToEnglish(string englishPlateText, bool isReversed)
        {
            StringBuilder englishResult = new StringBuilder();
            foreach (char letter in englishPlateText)
            {
                if (letter == ' ')
                {
                    englishResult.Append(' ');
                }
                else
                {
                    var englishLetter = letter;
                    if (CarPlateCharactersArabicToEnglishMapping.ContainsKey(letter))
                        englishLetter = CarPlateCharactersArabicToEnglishMapping[letter];
                    englishResult.Append(englishLetter);
                }
            }

            if (isReversed)
                return string.Join("", englishResult.ToString().Reverse());
            else
                return englishResult.ToString();
        }
    }
}
