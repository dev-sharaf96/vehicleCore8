using System;

namespace Tameenk.Core.Domain.Entities
{
    public class NajmAccidentResponse : BaseEntity
    {
        public int Id { get; set; }
        public int NoOfAccident { get; set; }
        public string ReferenceId { get; set; }
        public string ReferenceNo { get; set; }
        public string MessageID { get; set; }
        public string VehicleId { get; set; }
        public string DriverNin { get; set; }
        public string NajmResponse { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
