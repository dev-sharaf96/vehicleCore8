//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Web;
//using System.Web.Script.Serialization;
//using System.Xml.Serialization;
//using Tameenk.Core.Configuration;
//using Tameenk.Core.Domain.Entities;
//using Tameenk.Integration.Dto.Najm;
//using Tameenk.Loggin.DAL;
//using Tameenk.Services.InquiryGateway.Services.Core;
//using Tameenk.Services.Logging;

//namespace Tameenk.Services.InquiryGateway.Services.Implementation
//{
//    public class FakeNajmService : INajmService
//    {
//        /// <summary>
//        /// Gets the najm service Api...
//        /// Najm for Insurance Services Company has created an efficient platform to simplify, address, and resolve accident related procedures and formalities. Since its inception in 2007, this Saudi company has committed itself to providing hassle-free and smooth operations within insurance companies. Headquartered in Riyadh, Najm operates according to the regulations set by the law of Saudi Arabia, Ministry of Interior, and Saudi Arabian Monetary Agency (SAMA).
//        /// </summary>
//        /// <param name="request">NajmRequestMessage</param>
//        /// <returns></returns>
//        /// TODO Edit XML Comment Template for getNajm
//        public NajmResponse GetNajm(NajmRequest request, ServiceRequestLog predefinedLogInfo)
//        {
//            try
//            {
//                string responseData = ReadResource("Tameenk.Services.InquiryGateway.FakeResponses.NCDEligibility.xml");

//                XmlSerializer serializer = new XmlSerializer(typeof(NajmResponse));
//                using (StringReader reader = new StringReader(responseData))
//                {
//                    return (NajmResponse)serializer.Deserialize(reader);
//                }
//            }
//            catch (Exception)
//            {
                
//            }
//            return null;
//        }

//        private string ReadResource(string resourceName)
//        {
//            var assembly = Assembly.GetExecutingAssembly();
//            //var resourceName = "MyCompany.MyProduct.MyFile.txt";
//            string result = "";
//            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
//            using (StreamReader reader = new StreamReader(stream))
//            {
//                result = reader.ReadToEnd();
//            }
//            return result;

//        }
//    }
//}