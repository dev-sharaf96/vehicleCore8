using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tamkeen.bll.Model;
using Tamkeen.bll.Utils;

namespace Tamkeen.bll.Lookups
{
    public class Gender : IGender
    {
        Validate validate;
        /// <summary>
        /// Initializes a new instance of the <see cref="Gender"/> class.
        /// </summary>
        public Gender()
        {
            this.validate = new Validate();
        }
        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <param name="genderRequestMessage">The gender request message.</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for GetGender
        public GenderResponseMessage GetGender(GenderRequestMessage genderRequestMessage)
        {
            List<ValidationResult> errorList = null;
            /// Check validation ..
            if (validate.TryValidate(genderRequestMessage, out errorList))
            {
                return new GenderResponseMessage { status = 1, genderList = null };
            }
            else
            {
                return new GenderResponseMessage { status = 0, errorMsg = errorList[0].ErrorMessage };

            }

        }
    }
}
