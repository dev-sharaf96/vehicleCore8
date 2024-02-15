using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class Nationality : INationality
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Nationality"/> class.
        /// </summary>
        public Nationality()
        {

        }
        /// <summary>
        /// Gets the nationality list.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for GetNationalityList
        public NationalityResponseMessage GetNationalityList(NationalityRequestMessage requestMessage)
        {
            if (requestMessage.language == "ar")
            {
                return new NationalityResponseMessage { status = 1 };
            }
            else
            {
                return new NationalityResponseMessage { status = 1 };
            }
        }
    }

}
