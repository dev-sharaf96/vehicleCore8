namespace Tameenk.Services.Implementation.Checkouts
{
    /// <summary>
    /// Checkouts Filter
    /// </summary>
    public class CheckoutsFilter
    {
        /// <summary>
        /// NIN
        /// </summary>
        public  string? NIN { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        public  string? ReferenceId { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        public  string? SequenceNumber { get; set; }
        public  string? MerchantId { get; set; }

    }
}
