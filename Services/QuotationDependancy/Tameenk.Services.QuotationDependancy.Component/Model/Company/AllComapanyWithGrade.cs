namespace Tameenk.Services.QuotationDependancy.Component
{
    public class AllComapanyWithGrade
    {
        public int InsuranceCompanyID { get; set; }
        public int Grade { get; set; } = 0;
        public string Key { get; set; }
        public int? Order { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public string DescAR { get; set; }
        public string DescEN { get; set; }
        public bool IsActiveTPL { get; set; }
        public bool IsActiveComprehensive { get; set; }
        public bool ActiveTabbySanadPlus { get; set; }
        public bool ActiveTabbyWafiSmart { get; set; }
        public bool ActiveTabbyTpl { get; set; }
        public bool ActiveTabbyComp { get; set; }
        public string AppPoolName { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string HomePhone { get; set; }
        public string MobileNumber { get; set; }
        public string WebSite { get; set; }
        public string AddressEn { get; set; }
        public string AddressAr { get; set; }
    }
}
