using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class FailPolicyFilter
    {
        /// <summary>
        /// Insured Phone
        /// </summary>
        public string InsuredPhone { get; set; }

        /// <summary>
        /// insurance company id
        /// </summary>
        public int? InsuranceCompanyId { get; set; }

        /// <summary>
        /// invoice No
        /// </summary>
        public int? InvoiceNo { get; set; }

        /// <summary>
        /// National id ( NIN)
        /// </summary>
        public string NationalId { get; set; }

        /// <summary>
        /// insured first name ar
        /// </summary>
        public string InsuredFirstNameAr { get; set; }


        /// <summary>
        /// insured last name ar
        /// </summary>
        public string InsuredLastNameAr { get; set; }

        /// <summary>
        /// insured email
        /// </summary>
        public string InsuredEmail { get; set; }




        /// <summary>
        /// Sequence No to vehicle
        /// </summary>
        public string SequenceNo { get; set; }

        /// <summary>
        /// Custom no to vehicle
        /// </summary>
        public string CustomNo { get; set; }

        /// <summary>
        /// Reference No
        /// </summary>
        public string ReferenceNo { get; set; }

        /// <summary>
        /// Product Type Id such as TPL & Comperhensive
        /// </summary>
        public int? ProductTypeId { get; set; }


       

        /// <summary>
        /// end Date
        /// </summary>
        public DateTime? EndDate { get; set; }


        /// <summary>
        /// start date
        /// </summary>
        public DateTime? StartDate { get; set; }


        /// <summary>
        /// Policy Status Id
        /// </summary>
        public int? PolicyStatusId { get; set; }
    }
}
