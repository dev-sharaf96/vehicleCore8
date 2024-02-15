using System;

namespace Tameenk.Core.Domain.Entities
{
    public  class InsuranceCompanyGrade : BaseEntity
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public int? Grade { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
