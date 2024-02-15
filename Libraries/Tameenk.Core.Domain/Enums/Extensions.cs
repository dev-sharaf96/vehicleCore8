using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Enums
{
    public static class Extensions
    {
        /// <summary>
        /// Get the name of the enum.
        /// </summary>
        /// <typeparam name="T">The enum.</typeparam>
        /// <param name="enumerationValue">The enumeration value.</param>
        /// <returns></returns>
        public static string GetLocalizedName<T>(this T enumerationValue, CultureInfo cultureInfo = null)
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }
            string name = null; 
            //Tries to find a NameAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(LocalizedNameAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the name value
                    name = ((LocalizedNameAttribute)attrs[0]).GetName(cultureInfo);
                }
            }
            //If we have no name attribute, just return the ToString of the enum
            return string.IsNullOrWhiteSpace(name) ? enumerationValue.ToString() : name;
        }


        /// <summary>
        /// Get the enum code.
        /// </summary>
        /// <typeparam name="T">The enum.</typeparam>
        /// <param name="enumerationValue">The enum value.</param>
        /// <returns></returns>
        public static string GetCode<T>(this T enumerationValue)
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a CodeAttribute for a potential code.
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(CodeAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((CodeAttribute)attrs[0]).Code;
                }
            }
            //If we have no code attribute, just return the ToString of the enum
            return Convert.ToInt32(enumerationValue).ToString();
        }

        public static T FromCode<T>(string code)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                if (code == item.GetCode())
                    return (T)item;
            }
            return default(T);
        }

        public static T FromLocalizedName<T>(string name, CultureInfo cultureInfo=null)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                if (name.Equals(item.GetLocalizedName(cultureInfo), StringComparison.InvariantCultureIgnoreCase))
                    return (T)item;
            }
            return default(T);
        }

        public static IEnumerable<string> GetLocalizedNames<T>(this T enumuration)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            List<string> names = new List<string>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                names.Add(((T)value).GetLocalizedName());
            }
            return names;
        }

        public static IEnumerable<IdNamePair> GetAsKeyValuePair<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            List<IdNamePair> result = new List<IdNamePair>();

            foreach (var item in Enum.GetValues(typeof(T)))
            {

                result.Add(new IdNamePair()
                {
                    Id = (int)item,
                    Name = ((T)item).GetLocalizedName()
                });
            }
            return result;
        }

        public static List<Dtos.InquiryLookup> GetLookupAsKeyValuePair<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            List<Dtos.InquiryLookup> result = new List<Dtos.InquiryLookup>();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                result.Add(new Dtos.InquiryLookup()
                {
                    Id = (int)item,
                    Name = ((T)item).GetLocalizedName(CultureInfo.GetCultureInfo(LanguageTwoLetterIsoCode.Ar.ToString())),
                    NameEn = ((T)item).GetLocalizedName(CultureInfo.GetCultureInfo(LanguageTwoLetterIsoCode.En.ToString()))
                });
            }
            return result;
        }

        public static String FromCodeLocalizedName<T>(string code, CultureInfo cultureInfo = null)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                if (code == item.GetCode())
                    return ((T)item).GetLocalizedName(cultureInfo);
            }
            return null;
        }
    }
}