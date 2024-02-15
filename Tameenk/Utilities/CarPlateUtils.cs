using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.Services;

namespace Tamkeen.Utilities
{
    class CarPlateUtils
    {
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
                    if (ServicesConst.CarPlateCharactersEnglishToArabicMapping.ContainsKey(letter))
                        arabicLetter = ServicesConst.CarPlateCharactersEnglishToArabicMapping[letter];

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
                    if (ServicesConst.CarPlateCharactersArabicToEnglishMapping.ContainsKey(letter))
                        englishLetter = ServicesConst.CarPlateCharactersArabicToEnglishMapping[letter];
                    englishResult.Append(englishLetter);
                }
            }

            if (isReversed)
                return string.Join("", englishResult.ToString().Reverse());
            else
                return englishResult.ToString();
        }

        public static string GetCarPlateColorByCode(int? carPlateCode)
        {
            string resultColor = "";
            switch (carPlateCode)
            {
                case 1:
                case 10:
                    resultColor = "white";
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 7:
                    resultColor = "blue";
                    break;
                case 6:
                    resultColor = "yellow";
                    break;
                case 8:
                case 11:
                    resultColor = "black";
                    break;
                case 9:
                    resultColor = "green";
                    break;
            }

            return resultColor;
        }
    }
}
