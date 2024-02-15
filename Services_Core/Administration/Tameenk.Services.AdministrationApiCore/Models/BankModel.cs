using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class BankModel : BaseViewModel
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string IBAN { get; set; }
        public string NationalAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<string> bankNins { get; set; }
        public List<int> bankInsuranceCompanies { get; set; }
    }
}