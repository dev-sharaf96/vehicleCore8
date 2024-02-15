using System;
namespace Tameenk.Core.Domain.Entities
{
  public  class WathqInfo : BaseEntity
    {
        public int Id { get; set; }
        public string CrName { get; set; }
        public long CrNumber { get; set; }
        public string CrEntityNumber { get; set; }
        public bool IsMain { get; set; }
        public DateTime? CreatedDate { get; set; } 
        public DateTime? ModifiedDate { get; set; }
        public string InsuredNIN { get; set; }

    }
}
