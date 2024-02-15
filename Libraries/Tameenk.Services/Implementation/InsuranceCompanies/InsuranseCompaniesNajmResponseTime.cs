using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class InsuranseCompaniesNajmResponseTime
    {
        public double? AverageResponseTime { get; set; }
        public string CompanyEn { get; set; }
        public string CompanyAr { get; set; }
        public int? CompanyId { get; set; }
        public int? NajmGrade { get; set; }
        public DateTime? NajmGradeValidFrom { get; set; }
        public DateTime? NajmGradeValidTo { get; set; }

    }
}
