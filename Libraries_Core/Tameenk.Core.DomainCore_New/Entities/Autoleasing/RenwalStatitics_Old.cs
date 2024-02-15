using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
namespace Tameenk.Core.Domain.Entitie
{
    public class RenwalStatitics_Old : BaseEntity
    {
        public int Id { get; set; }
        public string parentExternalId { get; set; }
        public string parentReferenceId { get; set; }
        public string ReferenceId { get; set; }
        public string ExternalId { get; set; }
        public string SequenceNumber { get; set; }
        public decimal ? TotalPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserId { get; set; }
        public bool? isDeleted { get; set; }
    }
}


