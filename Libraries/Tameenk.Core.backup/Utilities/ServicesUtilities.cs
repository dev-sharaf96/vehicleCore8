using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Tameenk.Core
{
    public class ServicesUtilities
    {
      

        /// <summary>
        /// Gets the SOAP XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetSoapXml<T>(T obj)
        {
            try
            {
                if (obj == null)
                    return string.Empty;

                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, obj);
                    return textWriter.ToString().Replace("  ", "").Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", string.Empty)
                    .Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty)
                    .Replace(">    <", "><").Replace(">  <", "><").Replace(">      <", "><").Trim(); ;
                }
            }
            catch (Exception exp)
            {
                //ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the internal server ip.
        /// </summary>
        /// <returns></returns>
        public static string GetServerIP()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                return (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).FirstOrDefault();
            }
            catch (Exception exp)
            {
               // ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the credential cache.
        /// </summary>
        /// <param name="URL">The URL.</param>
        /// <returns></returns>
        public static System.Net.CredentialCache GetCredentialCache(string URL)
        {
            System.Net.NetworkCredential objNetworkCredential = new System.Net.NetworkCredential("portal", "portal");
            System.Net.CredentialCache objCredentialCache = new System.Net.CredentialCache();
            objCredentialCache.Add(new Uri(URL), "Basic", objNetworkCredential);
            return objCredentialCache;
        }

      

        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="body">The body.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns></returns>
        public static string SendRequest(string url, string request, out HttpStatusCode statusCode)
        {
            statusCode = new HttpStatusCode();
            try
            {
                string response = string.Empty;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(request);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                statusCode = httpResponse.StatusCode;
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }
                return response;
            }
            catch (Exception exp)
            {
               // ErrorLogger.LogError(exp.Message, exp, false);
                return null;
            }
        }

      

        /// <summary>
        /// Objects to XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public static Object ObjectToXML(string xml, Type objectType)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            Object obj = null;
            try
            {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
               // ErrorLogger.LogError("Error While deserialising object " + xml, exp, false);
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                if (strReader != null)
                {
                    strReader.Close();
                }
            }
            return obj;
        }

       
    }
}
