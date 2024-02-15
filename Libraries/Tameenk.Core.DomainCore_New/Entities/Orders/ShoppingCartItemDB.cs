using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;namespace Tameenk.Core.Domain.Entities.Orders{    public class ShoppingCartItemDB    {
        public string UserId { get; set; }        public string ReferenceId { get; set; }        public Guid ProductId { get; set; }        public int Quantity { get; set; }
        public bool ActiveTabbyTPL { get; set; }        public bool ActiveTabbyComp { get; set; }
        public bool ActiveTabbyWafiSmart { get; set; }        public bool ActiveTabbySanadPlus { get; set; }        public List<ShoppingCartItemBenefitsList> ShoppingCartItemBenefits { get; set; }        public List<ShoppingCartItemDriversList> ShoppingCartItemDrivers { get; set; }        public List<PriceDetailsList> PriceDetails { get; set; }



        #region Product        public string ProductNameAr { get; set; }        public string ProductNameEn { get; set; }        public decimal ProductPrice { get; set; }        public short? InsuranceTypeCode { get; set; }
        public int? ProductInsuranceTypeCode { get; set; }
        public int? VehicleLimitValue { get; set; }

        #endregion
        #region InsuranceCompany        public string InsuranceCompanyKey { get; set; }        public int InsuranceCompanyID { get; set; }        public string InsuranceCompanyNameAR { get; set; }        public string InsuranceCompanyNameEN { get; set; }        public bool IsAddressValidationEnabled { get; set; }
        public bool UsePhoneCamera { get; set; }




        #endregion
        #region QuotationResponse        public long QuotationResponseId { get; set; }        public DateTime QuotationResponseCreateDateTime { get; set; }        public bool? VehicleAgencyRepair { get; set; }        public int? DeductibleValue { get; set; }        public string QuotationReferenceId { get; set; }        public int InsuranceCompanyId { get; set; }


        #endregion
    }    public class ShoppingCartItemBenefitsList    {        public long Id { get; set; }



        #region Product_Benefit        public short? BenefitId { get; set; }        public decimal? BenefitPrice { get; set; }        public string BenefitExternalId { get; set; }        public bool IsReadOnly { get; set; }




        #endregion
        #region Benefit        public short BenefitCode { get; set; }        public string BenefitEnglishDescription { get; set; }        public string BenefitArabicDescription { get; set; }


        #endregion    }    public class PriceDetailsList    {        public byte PriceTypeCode { get; set; }        public decimal PriceValue { get; set; }



        #region PriceType        public string PriceTypeEnglishDescription { get; set; }        public string PriceTypeArabicDescription { get; set; }


        #endregion    }    public class ShoppingCartItemDriversList    {        public int Id { get; set; }
        public Guid DriverId { get; set; }
        public decimal DriverPrice { get; set; }
        public string DriverExternalId { get; set; }    }}