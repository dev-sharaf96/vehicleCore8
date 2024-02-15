using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class YearLookup : IYearLookup
    {
        private List<Lookup> arabicYears = null;
        private List<Lookup> licenseYears = null;
        private List<Lookup> years = null;

        private YearLookup() { }
        private static YearLookup yearLookup = null;
        public static YearLookup Instance
        {
            get
            {
                if(yearLookup == null)
                {
                    yearLookup = new YearLookup();
                }

                return yearLookup;
            }
        }

        public List<Lookup> GetArabicYears()
        {
            if (arabicYears != null)
                return arabicYears;
            arabicYears = new List<Lookup>();
            //todo:  repalce GetArabicYears with DAL
            UmAlQuraCalendar hijri = new UmAlQuraCalendar();

            for (int i = 1330; i <= hijri.GetYear(DateTime.Now); i++)
            {
                arabicYears.Add(new Lookup
                {
                    Id = i.ToString(),
                    Name = i.ToString()
                });
            }

            return arabicYears;
        }
        public List<Lookup> GetlicenseYears()
        {
            if (licenseYears != null)
                return licenseYears;
            licenseYears = new List<Lookup>();
            //todo:  repalce GetArabicYears with DAL
            for (int i = 1439; i <= 1470; i++)
            {
                licenseYears.Add(new Lookup
                {
                    Id = i.ToString(),
                    Name = i.ToString()
                });
            }

            return licenseYears;
        }

        public List<Lookup> GetYears()
        {
            if (years != null)
                return years;
            years = new List<Lookup>();
            //todo: repalce GetYears with DAL
            for (int i = 1900; i <= DateTime.Now.Year; i++)
            {
                years.Add(new Lookup
                {
                    Id = i.ToString(),
                    Name = i.ToString()
                });
            }
            return years;
        }
    }
}
