using System;

namespace Tameenk.Cancellation.DAL.Entities
{
    public abstract class BaseEntity : IAuditableEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
    

    public interface IAuditableEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime ModifiedDate { get; set; }
        bool IsActive { get; set; }
    }
}
