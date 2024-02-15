namespace Tameenk.Core.Domain.Entities.Quotations
{
    public class TawuniyaProposal : BaseEntity
    {
        public int Id { get; set; }
        public string ProposalNumber { get; set; }
        public string ReferenceId { get; set; }

        public int ProposalTypeCode { get; set; }

    }
}
