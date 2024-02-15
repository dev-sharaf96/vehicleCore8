using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Drivers
{
   public class DriverFilter
    {
        /// <summary>
        /// National ID
        /// </summary>
       public string NIN { get; set; }

        /// <summary>
        /// FirstName
        /// </summary>

        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
       public string LastName { get; set; }


        /// <summary>
        /// Email
        /// </summary>
       public string Email { get; set; }

        /// <summary>
        /// IBAN
        /// </summary>
       public string IBAN { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        

    }
}
