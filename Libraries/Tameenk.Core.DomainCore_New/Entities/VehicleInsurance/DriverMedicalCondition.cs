namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class DriverMedicalCondition : BaseEntity
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
    }
}
