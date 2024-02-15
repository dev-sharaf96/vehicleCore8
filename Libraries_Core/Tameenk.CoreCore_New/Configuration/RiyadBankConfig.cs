using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tameenk.Core.Extensions;

namespace Tameenk.Core.Configuration
{
    /// <summary>
    /// Riydbank MIGS payment configuration.
    /// </summary>
    public class RiyadBankConfig
    {
        /// <summary>
        /// Create new instnance of riydbank configuration class
        /// </summary>
        /// <param name="section">The parent configuration section to resolve the configurations.</param>
        public RiyadBankConfig(XmlNode section)
        {
            var riyadBankConfigSection = section.SelectSingleNode("RiyadBank");
            Url = riyadBankConfigSection.GetString("Url");
            MerchantId = riyadBankConfigSection.GetString("MerchantId");
            AccessCode = riyadBankConfigSection.GetString("AccessCode");
            SecureHashSecret = riyadBankConfigSection.GetString("SecureHashSecret");
        }
        /// <summary>
        /// The riydbank url to call for payment request process. 
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// The merchant identifier.
        /// </summary>
        public string MerchantId { get; private set; }
        /// <summary>
        /// The access code to autorizee the call with MIGS payment gateway.
        /// </summary>
        public string AccessCode { get; private set; }
        /// <summary>
        /// The secure hash secret used to generate signature for validation.
        /// </summary>
        public string SecureHashSecret { get; private set; }

    }
}
