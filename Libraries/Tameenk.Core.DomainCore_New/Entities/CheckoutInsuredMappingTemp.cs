
namespace Tameenk.Core.Domain.Entities
{
    public class CheckoutInsuredMappingTemp : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public int InsuredId { get; set; }
        public string ExternalId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDone { get; set; }
        public int ProcessingTries { get; set; }
        public string ErrorDescription { get; set; }
    }
}
