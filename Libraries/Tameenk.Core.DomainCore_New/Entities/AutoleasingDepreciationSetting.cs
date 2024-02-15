using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Core.Domain.Entities
{
    public class AutoleasingDepreciationSetting : BaseEntity
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public int MakerCode { get; set; }
        public int ModelCode { get; set; }
        public string MakerName { get; set; }
        public string ModelName { get; set; }
        public decimal Percentage { get; set; }
        public bool IsDynamic { get; set; }
        public decimal FirstYear { get; set; }
        public decimal SecondYear { get; set; }
        public decimal ThirdYear { get; set; }
        public decimal FourthYear { get; set; }
        public decimal FifthYear { get; set; }
        public string AnnualDepreciationPercentage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
