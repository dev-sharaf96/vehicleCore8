using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class LicenseType : BaseEntity
    {
        public short Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }
        public int? WataniyaCode { get; set; }
        public int? AutoleasingWataniyaCode { get; set; }
    }
}
