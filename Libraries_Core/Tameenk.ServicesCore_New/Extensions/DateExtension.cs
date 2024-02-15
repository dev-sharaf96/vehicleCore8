using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Extensions
{
    public static class DateExtension
    {
        public static string ConvertToGregorianString(this DateTime obj)
        {
            GregorianCalendar gregorian = new GregorianCalendar();
            int y = gregorian.GetYear(obj);
            int m = gregorian.GetMonth(obj);
            int d = gregorian.GetDayOfMonth(obj);
            int hour = gregorian.GetHour(obj);
            int min = gregorian.GetMinute(obj);
            int sec = gregorian.GetSecond(obj);
            DateTime gregorianDate = new DateTime(y, m, d, hour, min, sec);
            return gregorianDate.ToString(CultureInfo.InvariantCulture);
        }

        public static DateTime ToArabStandardTime(this DateTime dateTime)
        {
            TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
            return TimeZoneInfo.ConvertTime(dateTime, TimeZone);
        }

        /// <summary>
        /// return true if given date within given hours
        /// </summary>
        /// <param name="givenDate">DateTime</param>
        /// <param name="hours">Hours</param>
        /// <returns></returns>
        public static bool GivenDateWithinGivenHours(this DateTime givenDate, int hours)
        {
            return DateTime.Now.Subtract(givenDate).TotalHours < hours;
        }


        public static int GetUserAge(this DateTime birthdate, bool isHijriDate = false)
        {
            // Save today's date.
            var today = DateTime.Today;
            UmAlQuraCalendar hijri = new UmAlQuraCalendar();
            // Calculate the age.
            int age = 0;
            // 1 if driver is citizen
            if (!isHijriDate)
                age = today.Year - birthdate.Year;
            else
                age = hijri.GetYear(today) - hijri.GetYear(birthdate);
            return age;
        }

        public static string GetTimestamp(this DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        /// <summary>
        /// Convert Hijri string to datetime.
        /// </summary>
        /// <param name="">Hijri date as string.</param>
        /// <returns></returns>
        public static DateTime ConvertHijriStringToDateTime(string dateString, string dateFormat = "dd-MM-yyyy")
        {
            DateTime output;
            CultureInfo arSA = new CultureInfo("ar-SA");
            arSA.DateTimeFormat.Calendar = new UmAlQuraCalendar();

            if (DateTime.TryParseExact(dateString, dateFormat, arSA, 0, out output))
            {
                return output;
            }
            else if (DateTime.TryParseExact(dateString, "d-M-yyyy", arSA, 0, out output))
            {
                return output;
            }
            else
            {
                return output;
            }
        }
        public static int GetCurrenthijriYear()
        {
            var today = DateTime.Today;
            UmAlQuraCalendar hijri = new UmAlQuraCalendar();
            int year = 0;
            year = hijri.GetYear(today);
            return year;
        }

    }
}
