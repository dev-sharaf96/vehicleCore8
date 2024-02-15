using System;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class PromotionProgramCodeModel
    {
        public int Id { get; set; }
        public int PromotionProgramId { get; set; }
        public string Code { get; set; }
        public int InsuranceCompanyId { get; set; }
        public bool IsDeleted { get; set; }
       

        public short InsuranceTypeCode { get; set; }

        public string CreatorId { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ModifierId { get; set; }
        public DateTime? ModificationDate { get; set; }
        public PromotionProgramModel PromotionProgram { get; set; }
        public InsuranceCompanyModel InsuranceCompany { get; set; }
        public ProductTypeModel ProductType { get; set; }
        public AspNetUserModel Modifier { get; set; }
        public AspNetUserModel Creator { get; set; }
        public bool IsComperhensive { get; set; }
    }
}