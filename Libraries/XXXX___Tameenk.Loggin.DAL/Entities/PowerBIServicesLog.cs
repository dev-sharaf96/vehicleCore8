namespace Tameenk.Loggin.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PowerBIServicesLog")]
    public class PowerBIServicesLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Method { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public string CompanyKey { get; set; }
        public int? TotalRecord { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public string Channel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

