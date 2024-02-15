using System;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class PromotionProgramDomainModel
    {
        public int Id { get; set; }
        public int PromotionProgramId { get; set; }
        public string Domian { get; set; }
        public string DomainNameAr { get; set; }
        public string DomainNameEn { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public PromotionProgramModel PromotionProgram { get; set; }
        public bool? IsActive { get; set; }

    }
}