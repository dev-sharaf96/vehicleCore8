using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Enums
{
    /// <summary>
    /// The code attribute.
    /// </summary>
    public class CodeAttribute : Attribute
    {
        #region Ctor

        /// <summary>
        /// Initailize new instance of CodeAttribute class
        /// with code parameter.
        /// </summary>
        /// <param name="code">The code.</param>
        public CodeAttribute(string code)
        {
            Code = code;
        }
        #endregion

        #region Properties
        public string Code { get; private set; }

        #endregion
    }
}
