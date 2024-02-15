using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tamkeen.bll.Model;
using Tamkeen.bll.Utils;

namespace Tamkeen.bll.Lookups
{
    public class VehicleType
    {
        private Validate validate;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleType"/> class.
        /// </summary>
        public VehicleType()
        {
            this.validate = new Validate();
        }

        /// <summary>
        /// Gets the card types list.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <returns></returns>
        public VehicleTypesResponseMessage GetCardTypesList(VehicleTypesRequestMessage requestMessage)
        {
            List<ValidationResult> errorList = null;
            /// Check validation ..
            if (!validate.TryValidate(requestMessage, out errorList))
            {
                return new VehicleTypesResponseMessage { status = 0, errorMsg = errorList[0].ErrorMessage };
            }

            if (requestMessage.language == "ar")
            {
                return new VehicleTypesResponseMessage { status = 1 };
            }
            else
            {
                return new VehicleTypesResponseMessage { status = 1 };
            }
        }

    }
}
