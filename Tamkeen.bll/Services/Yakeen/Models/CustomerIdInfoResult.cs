using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.YakeenBCareService;

namespace Tamkeen.bll.Services.Yakeen.Models
{
    public class CustomerIdInfoResult
    {
        public CustomerIdInfoResult()
        {
            Error = new YakeenError();
        }

        public bool Success { get; set; }
        public YakeenError Error { get; set; }

        /// <summary>
        /// 1 --> citizen , 2 --> aligen
        /// </summary>
        public int IsCitizen { get; set; }
        public int LogId { get; set; }
        public string IdIssuePlace { get; set; }
        public gender Gender { get; set; }
        public short NationalityCode { get; set; }

        /// <summary>
        /// format : (dd-MM-yyyy)
        /// </summary>
        /// 
        public DateTime DateOfBirthG { get; set; }
        /// <summary>
        /// format : (dd-MM-yyyy)
        /// </summary>
        /// 
        public string DateOfBirthH { get; set; }
        /// <summary>
        /// format : (dd-MM-yyyy)
        /// </summary>
        /// 
        public string IdExpiryDate { get; set; }


        public Guid InternalIdentifier { get; set; }
    }
}
