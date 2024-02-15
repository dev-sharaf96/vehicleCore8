using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Services.Yakeen.Models
{
    public class CustomerNameInfoResult
    {
        public CustomerNameInfoResult()
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

        public string EnglishFirstName { get; set; }

        public string EnglishLastName { get; set; }

        public string EnglishSecondName { get; set; }

        public string EnglishThirdName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }

        public string FirstName { get; set; }

        public string ThirdName { get; set; }

        public string SubtribeName { get; set; }
    }
}
