using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tameenk.bll.Najm.model;

namespace Tamkeen.bll.Services.Najm
{
    public class NajmService : INajmService
    {
        /// <summary>
        /// Gets the najm service Api...
        /// Najm for Insurance Services Company has created an efficient platform to simplify, address, and resolve accident related procedures and formalities. Since its inception in 2007, this Saudi company has committed itself to providing hassle-free and smooth operations within insurance companies. Headquartered in Riyadh, Najm operates according to the regulations set by the law of Saudi Arabia, Ministry of Interior, and Saudi Arabian Monetary Agency (SAMA).
        /// </summary>
        /// <param name="request">NajmRequestMessage</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for getNajm
        public NajmResponseMessage GetNajm(NajmRequestMessage request)
        {

            NajmServices.NCDServiceClient serviceClient = new NajmServices.NCDServiceClient();
            serviceClient.ClientCredentials.UserName.UserName = request.userName;
            serviceClient.ClientCredentials.UserName.Password = request.password;
            string responseData = serviceClient.NCDEligibility(request.policyHolderId, request.IsVehicleRegistered ? 1 : 2, request.vehicleId);
            XmlSerializer serializer = new XmlSerializer(typeof(NajmResponseMessage));
            using (StringReader reader = new StringReader(responseData))
            {
                return (NajmResponseMessage)serializer.Deserialize(reader);
            }

        }
    }
}
