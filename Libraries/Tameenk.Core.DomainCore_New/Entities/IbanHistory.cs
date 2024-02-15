using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tameenk.Core.Domain.Entities.Orders;
using Tameenk.Core.Domain.Entities.Payments;
using Tameenk.Core.Domain.Entities.Payments.RiyadBank;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Entities
{
    public class IbanHistory : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string IBAN { get; set; }
        public string Bic { get; set; }
        public string Branch { get; set; }
        public string Bank { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Www { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string CountryIso { get; set; }
        public string Account { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string SepaDataSCT { get; set; }
        public string SepaDataSDD { get; set; }
        public string SepaDataCOR1 { get; set; }
        public string SepaDataB2B { get; set; }
        public string SepaDataSCC { get; set; }
        public string ResponseJson { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
