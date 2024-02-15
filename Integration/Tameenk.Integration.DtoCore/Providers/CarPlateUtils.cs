using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public static class CarPlateUtils
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
