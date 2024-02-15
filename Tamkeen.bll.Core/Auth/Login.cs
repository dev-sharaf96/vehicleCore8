using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tamkeen.bll.Model;
using Tamkeen.bll.Utils;

namespace Tamkeen.bll.Auth
{
    public class Login : ILogin
    {
        Validate validate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class .
        /// Check User Auth..
        /// </summary>
        /// 
        /// TODO Edit XML Comment Template for #ctor

        public Login(Validate validate)
        {
            this.validate = validate;
        }


        /// <summary>
        /// Does the login.
        /// this method ...
        /// </summary>
        /// <param name="loginRequest">The login request.</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for doLogin
        public LoginResponseMessage DoLogin(LoginRequestMessage loginRequest)
        {
            List<ValidationResult> errorList = null;
            /// Check validation ..
            if (validate.TryValidate(loginRequest, out errorList))
            {
                // TODO Check DB USer Auth ..
                return new LoginResponseMessage { status = 1, name = "" };

            }
            else
            {
                return new LoginResponseMessage { status = 0, error = errorList };
            }

        }
    }

}
