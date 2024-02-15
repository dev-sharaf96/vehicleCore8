using System.ComponentModel.DataAnnotations.Schema;


namespace Tameenk.Loggin.DAL
{
    [Table("PowerBIServicesLog")]
    public class PowerBIServicesLog
    {
        public int ID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ErrorCode { get; set; }
        public  String? ErrorDescription { get; set; }
        public  String? Method { get; set; }
        public double? ServiceResponseTimeInSeconds { get; set; }
        public  String? CompanyKey { get; set; }
        public int? TotalRecord { get; set; }
        public  String? ServerIP { get; set; }
        public  String? UserIP { get; set; }
        public  String? UserAgent { get; set; }
        public  String? Channel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

