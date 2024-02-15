using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public HttpPostedFileBase ImageRight { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public HttpPostedFileBase ImageLeft { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public HttpPostedFileBase ImageFront { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public HttpPostedFileBase ImageBack { get; set; }
        [Required(ErrorMessage = "* مطلوب")]
        public HttpPostedFileBase ImageBody { get; set; }

        public string UserId { get; set; }

        public ProductModel SelectedProduct { get; set; }

        public decimal PaymentAmount { get; set; }

        public int QuotationResponseId { get; set; }

        public int TypeOfInsurance { get; set; }

        public int? PaymentMethodCode { get; set; }
        public decimal LowestComperehensiveQoutPrice { get; set; }
        public string QtRqstExtrnlId { get; set; }
        public int InsuranceCompanyId { get; set; }

        public SelectList BankCodes { get; set; }

        public string AccessToken { get; set; }

        public string InsuranceCompanyKey { get; set; }

        public string Channel { get; set; }
    }
}