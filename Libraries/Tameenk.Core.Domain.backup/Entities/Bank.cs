using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.Core.Domain.Entities
{
    public class Bank : BaseEntity
    {

        public int Id { get; set; }

        public string NameEn { get; set; }
        public string NameAr { get; set; }

        public string IBAN { get; set; }
        public string NationalAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsAcitveWallet { get; set; }
        public bool HasWallet { get; set; }
        public bool PurchaseByNegative { get; set; }
        public decimal Balance { get; set; }
        public string BankKey { get; set; }
    }
}
