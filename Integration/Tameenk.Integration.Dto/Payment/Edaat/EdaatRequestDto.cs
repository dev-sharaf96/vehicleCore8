using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Payment.Edaat
{
    public class EdaatRequestDto
    {
        public bool IsClientEnterpise { set; get; }
        public string RegistrationNo { get; set; }
        public string NationalID { get; set; }
        public string InternalCode { set; get; }
        public string IssueDate { set; get; }
        public string DueDate { set; get; }
        public decimal TotalAmount { set; get; }
        public string Conditions { set; get; }
        public string SubBillerRegistrationNo { set; get; }
        public List<int> TaxIds { set; get; }
        public List<int> DiscountIDs { set; get; }
        public bool HasValidityPeriod { set; get; }
        public string FromDurationTime { set; get; }
        public string ToDurationTime { set; get; }
        public bool ExportToSadad { set; get; }
        public string ExpiryDate { set; get; }
        public decimal? SubBillerShareAmount { set; get; }
        public decimal? SubBillerSharePercentage { set; get; }
        public virtual List<EdaatProductDto> Products { set; get; }
        public virtual EdaatCustomerDto Customer { set; get; }
        public virtual EdaatCompanyDto Company { set; get; }
    }
    public class EdaatProductDto
    {
        public int ProductId { set; get; }
        public double Price { set; get; }
        public double Qty { set; get; }
    }
    public class EdaatCustomerDto
    {
        public string NationalID { set; get; }
        public string FirstNameAr { set; get; }
        public string FatherNameAr { set; get; }
        public string GrandFatherNameAr { set; get; }
        public string LastNameAr { set; get; }
        public string FirstNameEn { set; get; }
        public string FatherNameEn { set; get; }
        public string GrandFatherNameEn { set; get; }
        public string LastNameEn { set; get; }
        public string Email { set; get; }
        public string MobileNo { set; get; }
        public string DateOfBirth { set; get; }
        public string DateOfBirthHijri { set; get; }
        public string CustomerRefNumber { set; get; }
    }
    public class EdaatCompanyDto
    {
        public string RegistrationNo { set; get; }
        public string NameAr { set; get; }
        public string NameEn { set; get; }
        public string CommissionerNationalId { set; get; }
        public string CommissionerName { set; get; }
        public string CommissionerEmail { set; get; }
        public string CommissionerMobileNo { set; get; }
        public string CustomerRefNumber { set; get; }
    }
}
