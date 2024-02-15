namespace Tameenk.Loggin.DAL.Dtos
{
    public class TicketLogModel
    {
        public int? Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string MethodName { get; set; }
        public string UserAgent { get; set; }
        public string Channel { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string DriverNin { get; set; }
        public string ReferenceId { get; set; }
        public string UserId { get; set; }
        public string UserIP { get; set; }
        public string ServerIP { get; set; }
    }
}
