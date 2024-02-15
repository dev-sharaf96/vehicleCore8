using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Enums.Vehicles;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Integration.Dto.Quotation;
using Tameenk.Services.Checkout.Components;

namespace Tameenk.Models.Checkout
{

    public class CheckoutModel
    {
        [Required(ErrorMessage = "* مطلوب")]
        public String Email { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public String ReferenceId { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public String Phone { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public Nullable<int> BankCode { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public String IBAN { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public byte[] ImageRight { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public byte[] ImageLeft { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public byte[] ImageFront { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public byte[] ImageBack { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public byte[] ImageBody { get; set; }

        public string UserId { get; set; }

        public ProductModel SelectedProduct { get; set; }

        public decimal PaymentAmount { get; set; }

        public long QuotationResponseId { get; set; }

        public int TypeOfInsurance { get; set; }
        public bool IsSanadPlus { get; set; }

        public int? PaymentMethodCode { get; set; }
        public decimal LowestComperehensiveQoutPrice { get; set; }
        public string QtRqstExtrnlId { get; set; }
        public int InsuranceCompanyId { get; set; }

        public List<Lookup> BankCodes { get; set; }

        public string AccessToken { get; set; }

        public string InsuranceCompanyKey { get; set; }

        public string Channel { get; set; }

        public bool ErrorValidatingUserData { get; set; }

        public int? HyperpayRequestId { get; set; }
        public string Hashed { get; set; }
        public string ProductId { get; set; }
        public string SelectedProductBenfitId { get; set; }
        public string StcPayPhoneNo { get; set; }
        public bool IsReceivePolicyByEmailChecked { get; set; }
        public VehicleIdType? VehicleIdType { get; set; }
        public Tamkeen.bll.Model.VehicleModel Vehicle { get; set; }        public DriverData Driver { get; set; }        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyEndDate { get; set; }        public string SequenceNumber { get; set; }
        public bool IsCheckoutEmailVerified { get; set; }
        public bool IsCompany { get; set; } = false;
        public CompanyModel Company { get; set; }
        public string FirstAdditionalDriverFirstNameAr { get; set; }
        public string FirstAdditionalDriverFirstNameEn { get; set; }
        public string SecondAdditionalDriverFirstNameAr { get; set; }
        public string SecondAdditionalDriverFirstNameEn { get; set; }
        public bool ShowGallery { get; set; }
        public bool UsePhoneCamera { get; set; }
        public bool PurchaseByMobile { get; set; }
        public bool ImageRequired { get; set; }
        public int ProductInsuranceTypeCode { get; set; }
        public bool ShowWalletPaymentOption { get; set; }
        public bool IsMobileBrowser { get; set; }

        public string FirstAdditionalDriverNameAr { get; set; }
        public string FirstAdditionalDriverNameEn { get; set; }
        public string SecondAdditionalDriverNameAr { get; set; }
        public string SecondAdditionalDriverNameEn { get; set; }
        public string DiscountCode { get; set; }
        public bool IsRenewal { get; set; }
        public int? BcareCommission { get; set; }
        public QuotationResponseModel ODQuotationDetails { get; set; }
        public ODCheckoutDetails ODDetails { get; set; }
        public bool ShowTabby { get; set; }
        public string TermsAndConditions { get; set; }
        public bool CanEditPhoneNumper { get; set; }
        public string EducationLevel { get; set; }
        public string MileageExpectedAnnualName { get; set; }
        public string ParkingLocationName { get; set; }
        public int LeasingServiceId { get; set; }
        public bool IsLeasingPaymentMethod { get; set; }

        public CheckoutBCareDiscountModel BCareDiscount { get; set; }
    }
}