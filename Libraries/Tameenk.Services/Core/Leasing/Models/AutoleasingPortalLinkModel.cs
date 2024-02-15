using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class AutoleasingPortalLinkModel
    {
        public int Id { get; set; }
        public string CheckOutUserId { get; set; }
        public string Phone { get; set; }
        public string ReferenceId { get; set; }
        public Guid? MainDriverId { get; set; }
        public Guid VehicleId { get; set; }
        public LanguageTwoLetterIsoCode CheckoutSelectedlang { get; set; }
        public string DriverNin { get; set; }
        public string VehicleSequenceOrCustom { get; set; }
        public int BankId { get; set; }
        public string BankKey { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int? InsuranceTypeCode { get; set; }
    }
}
