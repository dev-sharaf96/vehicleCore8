using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    /// <summary>
    /// Policies Status filter
    /// use class to filter policies status
    /// </summary>
    public class StatusPolicyFilter
    {
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
        /// policy No
        /// </summary>
        public string PolicyNo { get; set; }

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
    }
}
