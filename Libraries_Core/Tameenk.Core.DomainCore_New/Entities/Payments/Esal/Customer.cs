using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Esal
{
    public class Customer:BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReferenceId { get; set; }
        public string Code { get; set; }
        public string NameArabic { get; set; }
        public string NameEnglish { get; set; }
        public string AddressArabic1 { get; set; }
        public string AddressArabic2 { get; set; }
        public string AddressEnglish1 { get; set; }
        public string AddressEnglish2 { get; set; }
        public int VatRegisterationNumber { get; set; }
        public string Branch { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string RegionEnglish { get; set; }
        public string RegionArabic { get; set; }
        public string CityEnglish { get; set; }
        public string CityArabic { get; set; }
        public string LocalityEnglish { get; set; }
        public string LocalityArabic { get; set; }
        public string BranchArabic { get; set; }
        public string PlantEnglish { get; set; }
        public string PlantArabic { get; set; }
        public string SegmentEnglish { get; set; }
        public string SegmentArabic { get; set; }
        public string DivisionEnglish { get; set; }
        public string DivisionArabic { get; set; }
        public string ProdLineEnglish { get; set; }
        public string ProdLineArabic { get; set; }

    }
}
