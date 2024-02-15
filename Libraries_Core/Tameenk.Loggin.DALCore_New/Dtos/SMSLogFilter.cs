namespace Tameenk.Loggin.DAL
{
    /// <summary>
    /// SMS Log Filter
    /// </summary>

    public class SMSLogFilter
    {
        /// <summary>
        /// Mobile Number
        /// </summary>
        public string MobileNumber { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Method { get; set; }
        public int? Channel { get; set; }
        public int? SMSProvider { get; set; }

    }
}
