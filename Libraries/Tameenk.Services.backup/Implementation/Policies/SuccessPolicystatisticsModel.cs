using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class SuccessPolicystatisticsModel
    {
       
            public string PolicyNo { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? PolicyEffectiveDate { get; set; }
            public string Phone { get; set; }
            public string StatusAR { get; set; }
            public string StatusEn { get; set; }
            public string Email { get; set; }
            public string AddtionalDriverOne { get; set; }
            public string AddtionalDriverTwo { get; set; }
            public string ArabicDescription { get; set; }
            public string EnglishDescription { get; set; }
            public byte[] ImageBack { get; set; }
            public byte[] ImageBody { get; set; }
            public byte[] ImageFront { get; set; }
            public byte[] ImageLeft { get; set; }
            public byte[] ImageRight { get; set; }
            public string CompanyNameAr { get; set; }
            public string CompanyNameEn { get; set; }
            public string InsuredNameAr { get; set; }
            public string InsuredNameEn { get; set; }
            public string plateText { get; set; }
            public string VehicleModel { get; set; }
            public short? CarPlateNumber { get; set; }
            public string CustomCardNumber { get; set; }
            public string SequenceNumber { get; set; }
            public bool OwnerTransfer { get; set; }
            public bool VehicleAgencyRepair { get; set; }
            public string ReferenceId { get; set; }
            public string NIN { get; set; }
            public decimal? Fees { get; set; }
            public decimal? Discount { get; set; }
            public decimal? ProductPrice { get; set; }
            public decimal? ExtraPremiumPrice { get; set; }
            public decimal? TotalPrice { get; set; }
            public decimal? SubTotalPrice { get; set; }
            public decimal? Vat { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public int? InvoiceNo { get; set; }
           public List<SuccessPolicyBenefits> SuccessPolicyBenefits { get; set; }

            //PolicyNo
        
    }
}
