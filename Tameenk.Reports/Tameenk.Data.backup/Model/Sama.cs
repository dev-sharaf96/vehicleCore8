using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Data.Model
{
    public class Sama
    {
        /// <summary>
        /// Invoice Date
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// Invoice No
        /// </summary>
        public int? BcareInvoiceNumber { get; set; }

        /// <summary>
        /// Scheme
        /// </summary>
        public string Scheme { get; set; }


        /// <summary>
        /// Main driver First Name + Second Name + Last Name
        /// </summary>
        public string PolicyHolder { get; set; }

        /// <summary>
        /// Checkout Phone
        /// </summary>
        public string Mob { get; set; }

        /// <summary>
        /// checkout Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// company NameAR
        /// </summary>
        public string Insurer { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        public string PolicyNo { get; set; }

        /// <summary>
        /// Payfort As Payment Method
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// CardNumber from PayfortPaymentResponse 
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// insurance product Type
        /// </summary>
        public string InsuranceProduct { get; set; }

        /// <summary>
        /// invoice Extra Premium Price
        /// </summary>
        public decimal? ExtraPremiumPrice { get; set; }


        /// <summary>
        /// invoice Vat
        /// </summary>
        public decimal?  Vat {get;set ;}
    }
}

