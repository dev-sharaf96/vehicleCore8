using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class PolicyModification : BaseEntity
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; }
        public string QuotationReferenceId { get; set; }

        public string PolicyNo { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public int? InsuranceTypeCode { get; set; }
        public string Nin { get; set; }
        public string MethodName { set; get; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public string Channel { get; set; }

        public decimal? TotalAmount { set; get; }
        public decimal? TaxableAmount { set; get; }
        public decimal? VATAmount { set; get; }

        public string CustomCard { get; set; }
        public string SequenceNumber { get; set; }
        public Guid? ConvertedVehicleId { get; set; }
        public Guid? VehicleId { get; set; }
        public int InvoiceNo { set; get; }
        public Guid? DriverId { get; set; }

        public bool IsLeasing { get; set; }
        public CheckoutProviderServicesCodes? ProviderServiceId { get; set; }
        public bool IsCheckedkOut { get; set; }
        public bool IsDeleted { get; set; }
    }
}
