using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Services.Policy.Components
{
    public class CheckoutDetailsInfo
    {
        public string ReferenceId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public LanguageTwoLetterIsoCode? SelectedLanguage { get; set; }


        public int? BankCodeId { get; set; }


        public string IBAN { get; set; }

        public int? PaymentMethodId { get; set; }

        public int? ImageRightId { get; set; }

        public int? ImageLeftId { get; set; }

        public int? ImageFrontId { get; set; }

        public int? ImageBackId { get; set; }

        public int? ImageBodyId { get; set; }

        public string UserId { get; set; }

        public Guid VehicleId { get; set; }

        public Guid? MainDriverId { get; set; }

        public int? PolicyStatusId { get; set; }
        public string Channel { get; set; }
        public int? InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; }
        public InsuranceCompany InsuranceCompany { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Guid? SelectedProductId { get; set; }

        public short? SelectedInsuranceTypeCode { get; set; }
        public Guid? AdditionalDriverIdOne { get; set; }
        public Guid? AdditionalDriverIdTwo { get; set; }
        public Guid? AdditionalDriverIdThree { get; set; }
        public Guid? AdditionalDriverIdFour { get; set; }
        public bool IsCancelled { get; set; } = false;
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
        public bool? IsEmailVerified { get; set; } = false;
        public int? BankId { get; set; }
        public int? CorporateAccountId { get; set; }
        public Guid? MerchantTransactionId { get; set; }

        public CheckoutDetail CheckoutDetails { get; set; }
        public int Id { get; set; }

        public string BankCode { get; set; }

        public string BankEnglishDescription { get; set; }

        public string BankArabicDescription { get; set; }

        public int? ValidationCode { get; set; }
    }
}
