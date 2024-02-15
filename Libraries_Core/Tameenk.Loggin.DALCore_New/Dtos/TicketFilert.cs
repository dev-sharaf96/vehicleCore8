namespace Tameenk.Loggin.DAL.Dtos
{
    public class TicketFilert
    {
        public int? Id { get; set; }

        public string UserEmail { get; set; }

        public int? StatusId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string NationalId { get; set; }

        public string PolicyNo { get; set; }

        public int? InvoiceNo { get; set; }

        public string CheckoutEmail { get; set; }

        public string Checkoutphone { get; set; }

        public string ReferenceNo { get; set; }

        public int? ChannelId { get; set; }

        public int? TicketTypeId { get; set; }
    }
}
