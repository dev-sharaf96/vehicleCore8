using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;

using System.Net.Security;

namespace Tameenk.Cancellation.Utilities
{
    public class Utilities
    {

        /// <summary>
        /// Gets or sets the base site URL.
        /// </summary>
        /// <value>
        /// The base site URL.
        /// </value>
        public static string BaseSiteUrl
        {
            get
            {
                return GetAppSetting(Strings.BaseSiteUrl);
            }
        }

        public static string PublicSiteURL
        {
            get
            {
                return GetAppSetting(Strings.PublicSiteURL);
            }
        }
        public static string OrangeDSLPublicSiteUrl
        {
            get
            {
                return GetAppSetting(Strings.OrangeDSLPublicSiteUrl);
            }
        }
        public static string DomainURL
        {
            get
            {
                return GetAppSetting("DomainURL");
            }
        }
     
        /// <summary>
        /// Gets a value indicating whether this instance is arabic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is arabic; otherwise, <c>false</c>.
        /// </value>
        public static bool IsArabic
        {
            get
            {
                return !CultureInfo.CurrentCulture.Name.Equals("en-us", StringComparison.InvariantCultureIgnoreCase);
            }
        }
        /// <summary>
        /// Gets the site URL.
        /// </summary>
        public static string SiteURL
        {
            get
            {
                string URL = HttpContext.Current.Request.Url.Host;
                int Port = HttpContext.Current.Request.Url.Port;
                string strPort = ":" + Port;
                string Protocol = "http://";
                if (HttpContext.Current.Request.IsSecureConnection || !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["X-Forwarded-For"]) || Port == 80)
                {
                    if (HttpContext.Current.Request.IsSecureConnection || !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["X-Forwarded-For"]))
                    {
                        Protocol = "https://";
                    }
                    strPort = string.Empty;
                }
                return Protocol + URL + strPort;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is secure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is secure; otherwise, <c>false</c>.
        /// </value>
        public static bool IsSecure
        {
            get
            {
                return HttpContext.Current.Request.IsSecureConnection ||
                    HttpContext.Current.Request.ServerVariables["HTTPS"].Equals("on", StringComparison.InvariantCultureIgnoreCase);
            }
        }
        /// <summary>
        /// Gets the language URL.
        /// </summary>
        public static string LanguageURL
        {
            get
            {
                if (IsArabic)
                {
                    return "/ar/";
                }
                else
                {
                    return "/en/";
                }
            }
        }

        /// <summary>
        /// Gets the domain URL in HTTPS.
        /// </summary>
        /// <value>
        /// The domain URL in HTTPS.
        /// </value>
        public static string DomainUrlInHttps
        {
            get
            {
                string url = string.Empty;
                object value = Utilities.GetValueFromCache("___DomainUrlInHttpsCACHEKEY___");
                if (value != null)
                {
                    url = (string)value;
                    return url;
                }
                else
                {
                    url = GetAppSetting("DomainUrlInHttps");
                    if (!string.IsNullOrEmpty(url))
                        Utilities.AddValueToCache("___DomainUrlInHttpsCACHEKEY___", url, 1440);

                    return url;
                }

            }
        }

      
        /// <summary>
        /// Gets the DB cache time.
        /// </summary>
        /// <value>
        /// The eai cache time.
        /// </value>
        public static int CacheTime
        {
            get
            {
                int cacheTime = 1440;
                int cacheTimeTemp;
                if (int.TryParse(Utilities.GetAppSetting("CacheTime"), out cacheTimeTemp))
                    cacheTime = cacheTimeTemp;
                return cacheTime;
            }
        }
        /// <summary>
        /// Gets the app setting.
        /// </summary>
        /// <param name="strKey">The STR key.</param>
        /// <returns></returns>
        public static string GetAppSetting(string strKey)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strKey];
            return strValue;
        }

       
        /// <summary>
        /// clear HTML tags
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string ClearHTML(string Input)
        {
            if (!string.IsNullOrEmpty(Input))
            {
                string strOutput = String.Empty;
                strOutput = Regex.Replace(Input, Strings.RegEx1, Strings.Space);
                return strOutput;
            }
            return string.Empty;
        }
        /// <summary>
        /// Encodes the HTML.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public static string EncodeHTML(string Input)
        {
            if (!string.IsNullOrEmpty(Input))
            {
                return Microsoft.Security.Application.Encoder.HtmlEncode(Input);
            }
            return string.Empty;
        }

        /// <summary>
        /// App_s the path.
        /// </summary>
        /// <returns></returns>
        public static String App_Path()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Adds the cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="isSecure">if set to <c>true</c> [is secure].</param>
        /// <param name="expireAfterInMinutes">The expire after in minutes.</param>
        public static void AddCookie(string name, string value, bool isSecure, int expireAfterInDays, string domainName)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.HttpOnly = true;
            cookie.Value = value;
            cookie.Secure = isSecure;
            if (!expireAfterInDays.Equals(0))
            {
                cookie.Expires = DateTime.Now.AddDays(expireAfterInDays);
            }
            if (!string.IsNullOrEmpty(domainName))
            {
                cookie.Domain = domainName;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Adds the cookie.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isSecure"></param>
        /// <param name="expireAfterInDays"></param>
        public static void AddCookie(string name, string value, bool isSecure, int expireAfterInDays)
        {
            AddCookie(name, value, isSecure, expireAfterInDays, string.Empty);
        }

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetCookie(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
                return HttpContext.Current.Request.Cookies[name].Value;
            else
                return string.Empty;
        }
        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private static Byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="pObject">The p object.</param>
        /// <returns></returns>
        public static String SerializeObject(Object pObject)
        {
            String XmlizedString = null;
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(pObject.GetType());
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, pObject);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
            memoryStream.Dispose();
            return XmlizedString;
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <param name="pXmlizedString">The p xmlized string.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Object DeserializeObject(String pXmlizedString, Type type)
        {
            XmlSerializer xs = new XmlSerializer(type);
            using (MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)))
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                return xs.Deserialize(memoryStream);
            }
        }
        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <param name="pXmlizedString">The p xmlized string.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Object DeserializeObject(String pXmlizedString, string type)
        {
            XmlSerializer xs = new XmlSerializer(Type.GetType(type));
            using (MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)))
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                return xs.Deserialize(memoryStream);
            }

        }

        public static string GetBriefString(string Body, int maxLength)
        {
            string brief = Body;
            if (string.IsNullOrEmpty(brief))
            {
                return string.Empty;
            }
            string newBrief = brief;
            //cut the brief with substring
            if (brief.Length > maxLength)
            {
                if (brief.Substring(0, maxLength + 1).EndsWith(Strings.Space))
                    return brief.Substring(0, maxLength);
                for (int i = 0; i < 20; i++)
                {
                    if (maxLength - 1 - i > 0)
                        newBrief = brief.Substring(0, maxLength - i);
                    else
                        newBrief = string.Empty;

                    if (newBrief.EndsWith(Strings.Space))
                    {
                        //correct
                        break;
                    }
                }
            }
            else
            {
                newBrief = brief;
            }
            return newBrief;
        }

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <returns></returns>
        public static byte[] ResizeImage(byte[] imageBytes, int maxWidth, int maxHeight)
        {
            Stream imageStream = new MemoryStream(imageBytes);
            Bitmap tempImage = new Bitmap(imageStream);
            double Width = 0;
            double Height = 0;
            if (tempImage.Width <= maxWidth && tempImage.Height <= maxHeight)
            {
                Width = tempImage.Width;
                Height = tempImage.Height;
            }
            else
            {
                double ratioX = Convert.ToDouble(maxWidth) / Convert.ToDouble(tempImage.Width);
                double ratioY = Convert.ToDouble(maxHeight) / Convert.ToDouble(tempImage.Height);
                double ratio = 0;
                if (ratioX > ratioY)
                    ratio = ratioY;
                else
                    ratio = ratioX;
                Width = ratio * tempImage.Width;
                Height = ratio * tempImage.Height;
            }
            Bitmap imgBitMap = new Bitmap(Convert.ToInt32(Width), Convert.ToInt32(Height), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            imgBitMap.SetResolution(tempImage.HorizontalResolution, tempImage.VerticalResolution);
            Graphics g = Graphics.FromImage(imgBitMap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.DrawImage(tempImage, 0, 0, imgBitMap.Width, imgBitMap.Height);
            MemoryStream OutputStream = new MemoryStream();
            imgBitMap.Save(OutputStream, ImageFormat.Jpeg);
            // 
            imgBitMap.Dispose();
            g.Dispose();
            return OutputStream.ToArray();
        }

        /// <summary>
        /// Get Value From Cache
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetValueFromCache(string CacheKey)
        {

            if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Cache == null)
                return null;

            if (System.Web.HttpContext.Current.Cache[CacheKey] != null)
            {
                return System.Web.HttpContext.Current.Cache[CacheKey];
            }
            return null;
        }
        /// <summary>
        /// Add Value To Cache
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="obj"></param>
        public static void AddValueToCache(string CacheKey, object obj)
        {
            if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Cache == null)
                return;

            System.Web.Caching.Cache cache = System.Web.HttpContext.Current.Cache;
            lock (cache)
            {
                System.Web.HttpContext.Current.Cache.Add(CacheKey, obj, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
            }
        }
        /// <summary>
        /// Adds the value to cache.
        /// </summary>
        /// <param name="CacheKey">The cache key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="Minutes">The minutes.</param>
        public static void AddValueToCache(string CacheKey, object obj, int Minutes)
        {
            if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Cache == null)
                return;

            System.Web.Caching.Cache cache = System.Web.HttpContext.Current.Cache;
            lock (cache)
            {
                System.Web.HttpContext.Current.Cache.Add(CacheKey, obj, null, DateTime.Now.AddMinutes(Minutes), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
            }
        }

        /// <summary>
        /// Removes the cache.
        /// </summary>
        /// <param name="CacheKey">The cache key.</param>
        public static void RemoveCache(string CacheKey)
        {
            System.Web.HttpContext.Current.Cache.Remove(CacheKey);
        }

        /// <summary>
        /// Removes all cache key.
        /// </summary>
        public static void RemoveAllCacheKey()
        {
            foreach (DictionaryEntry key in System.Web.HttpContext.Current.Cache)
            {
                System.Web.HttpContext.Current.Cache.Remove(key.Key.ToString());
            }
        }

        /// <summary>
        /// Gets the current language.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLanguage()
        {
            return Thread.CurrentThread.CurrentCulture.Parent.Name;
        }

        /// <summary>
        /// Gets the current culture info.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetCurrentCultureInfo()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerCase"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        static string alphaCaps = "QWERTYUPASDFGHJKZXCVBNM";
        static string alphaLow = "qwertyupasdfghjkzxcvbnm";
        static string numerics = "23456789";
        static string special = "*$-+?_&=!%{}/";
        //create another string which is a concatenation of all above

        /// <summary>
        /// Gets a random number between zero and maxValue (exclusive)
        /// </summary>
        /// <param name="rng">Random number generator</param>
        /// <param name="maxValue">max value</param>
        /// <returns></returns>
        public static int GetRandomNumber(RandomNumberGenerator rng, int maxValue)
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            int randomNumber = BitConverter.ToInt32(randomBytes, 0);
            return Math.Abs(randomNumber) % maxValue;
        }

        /// <summary>
        /// Gets the random position.
        /// </summary>
        /// <param name="posArray">The pos array.</param>
        /// <returns></returns>
        public static int getRandomPosition(RandomNumberGenerator rng, ref string posArray)
        {
            int pos;
            string randomChar = posArray.ToCharArray()[GetRandomNumber(rng, posArray.Length)].ToString();
            pos = int.Parse(randomChar);
            posArray = posArray.Replace(randomChar, "");
            return pos;
        }

        /// <summary>
        /// Gets the random char.
        /// </summary>
        /// <param name="fullString">The full string.</param>
        /// <returns></returns>
        public static string getRandomChar(RandomNumberGenerator rng, string fullString)
        {
            return fullString.ToCharArray()[GetRandomNumber(rng, fullString.Length)].ToString();
        }

        public static string GenerateStrongPasswordForOrangeMoney(int length)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            string allChars = alphaLow + numerics;
            String generatedPassword = string.Empty;

            if (length < 4)
                throw new Exception("Number of characters should be greater than 4.");

            // Generate four repeating random numbers are postions of
            // lower, upper, numeric and special characters
            // By filling these positions with corresponding characters,
            // we can ensure the password has atleast one
            // character of those types
            int pLower, pUpper, pNumber, pSpecial;
            string posArray = "0123456789";
            if (length < posArray.Length)
                posArray = posArray.Substring(0, length);
            pLower = getRandomPosition(rng, ref posArray);
            pUpper = getRandomPosition(rng, ref posArray);
            pNumber = getRandomPosition(rng, ref posArray);
            pSpecial = getRandomPosition(rng, ref posArray);


            for (int i = 0; i < length; i++)
            {
                if (i == pLower)
                    generatedPassword += getRandomChar(rng, alphaCaps);
                if (i == pUpper)
                    generatedPassword += getRandomChar(rng, alphaLow);
                else if (i == pNumber)
                    generatedPassword += getRandomChar(rng, numerics);
                else if (i == pSpecial)
                    generatedPassword += getRandomChar(rng, special);
                else
                    generatedPassword += getRandomChar(rng, allChars);
            }
            return generatedPassword;
        }


        /// <summary>
        /// Generates the strong password.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string GenerateStrongPassword(int length)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            string allChars = alphaLow + numerics;
            String generatedPassword = "";

            if (length < 4)
                throw new Exception("Number of characters should be greater than 4.");

            // Generate four repeating random numbers are postions of
            // lower, upper, numeric and special characters
            // By filling these positions with corresponding characters,
            // we can ensure the password has atleast one
            // character of those types
            int pLower, pUpper, pNumber, pSpecial;
            string posArray = "0123456789";
            if (length < posArray.Length)
                posArray = posArray.Substring(0, length);
            pLower = getRandomPosition(rng, ref posArray);
            pUpper = getRandomPosition(rng, ref posArray);
            pNumber = getRandomPosition(rng, ref posArray);
            pSpecial = getRandomPosition(rng, ref posArray);


            for (int i = 0; i < length; i++)
            {
                //if (i == pLower)
                //    generatedPassword += getRandomChar(rng, alphaCaps);
                if (i == pUpper)
                    generatedPassword += getRandomChar(rng, alphaLow);
                else if (i == pNumber)
                    generatedPassword += getRandomChar(rng, numerics);
                //else if (i == pSpecial)
                //    generatedPassword += getRandomChar(rng, special);
                else
                    generatedPassword += getRandomChar(rng, allChars);
            }
            return generatedPassword;
        }

        #region Input Validation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserInput">Input</param>
        /// <returns>resturns the input after removing any suspicious values</returns>
        public static string CheckUserInputText(string UserInput)
        {
            if (UserInput != null)
            {
                if (!string.IsNullOrEmpty(UserInput))
                {
                    string result = FilterUsersInputURL(UserInput);
                    return System.Web.HttpContext.Current.Server.HtmlEncode(result);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static string FilterUsersInputURL(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                string quote = "\"";
                val = Regex.Replace(val, "/script", string.Empty, RegexOptions.IgnoreCase);
                val = Regex.Replace(val, "script", string.Empty, RegexOptions.IgnoreCase);
                val = Regex.Replace(val, ".exe", string.Empty, RegexOptions.IgnoreCase);
                val = Regex.Replace(val, ".dll", string.Empty, RegexOptions.IgnoreCase);
                val = val.Replace("\r\n", string.Empty);
                val = val.Replace("^", string.Empty);
                val = val.Replace("%", string.Empty);
                val = val.Replace("!", string.Empty);
                val = val.Replace(";", string.Empty);
                val = val.Replace("~", string.Empty);
                val = val.Replace("--", string.Empty);
                val = val.Replace("'", string.Empty);
                val = val.Replace(quote, string.Empty);
                val = val.Replace("/>", string.Empty);
                val = val.Replace("</", string.Empty);
                val = val.Replace("<", string.Empty);
                val = val.Replace(">", string.Empty);
                val = val.Replace("(", string.Empty);
                val = val.Replace(")", string.Empty);
                val = val.Replace("[", string.Empty);
                val = val.Replace("]", string.Empty);
                val = val.Replace("#", string.Empty);
                val = val.Replace("||", string.Empty);
                val = val.Replace("&&", string.Empty);

                return val.Trim();
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// Determines whether [is safe URL] [the specified URL].
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        ///   <c>true</c> if [is safe URL] [the specified URL]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSafeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            url = url.Trim().ToLower();

            string SafeUrls = GetAppSetting("SafeUrls");

            if (!string.IsNullOrEmpty(SafeUrls))
            {
                foreach (string s in SafeUrls.Split(';'))
                {
                    if (url.StartsWith(s.Trim().ToLower()))
                    {
                        return true;
                    }
                }
            }

            if (!url.StartsWith("/"))
                url = "/" + url;

            if (url.StartsWith("/english/") || url.StartsWith("/en/") || url.StartsWith("/arabic/") || url.StartsWith("/ar/"))
            {
                return true;
            }

            return false;
        }

        public static bool IsSafeUrl2(string url, out string normalizedUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                normalizedUrl = "";
                return false;
            }

            url = url.Trim().ToLower();

            string SafeUrls = GetAppSetting("SafeUrls");

            if (!string.IsNullOrEmpty(SafeUrls))
            {
                foreach (string s in SafeUrls.Split(';'))
                {
                    if (url.StartsWith(s.Trim().ToLower()))
                    {
                        normalizedUrl = url;
                        return true;
                    }
                }
            }

            if (!url.StartsWith("/"))
                url = "/" + url;

            if (url.StartsWith("/english/") || url.StartsWith("/en/") || url.StartsWith("/arabic/") || url.StartsWith("/ar/"))
            {
                normalizedUrl = url;
                return true;
            }

            normalizedUrl = "";
            return false;
        }

        /// <summary>
        /// Gets the message from global resource.
        /// </summary>
        /// <param name="resourceClassName">Name of the resource class.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string GetMessageFromGlobalResource(string resourceClassName, string message)
        {
            string messageLocal = String.Empty;
            object objMessage = HttpContext.GetGlobalResourceObject(resourceClassName, message, Thread.CurrentThread.CurrentCulture);
            if (objMessage != null)
            {
                messageLocal = objMessage.ToString();
            }
            return messageLocal;
        }

        /// <summary>

        public static int BankTimeout
        {
            get
            {
                int BankTimeout = 310 * 1000;
                if (int.TryParse(GetAppSetting("BankTimeout"), out BankTimeout))
                {
                    return 310 * 1000;
                }
                return BankTimeout;
            }
        }

        /// <summary>
        /// Base64s the decodein bayte.
        /// </summary>
        /// <param name="base64EncodedData">The base64 encoded data.</param>
        /// <returns></returns>
        public static byte[] Base64DecodeinBayte(string base64EncodedData)
        {
            return System.Convert.FromBase64String(base64EncodedData);
        }

        /// <summary>
        /// Gets the internal server IP.
        /// </summary>
        /// <returns></returns>
        public static string GetInternalServerIP()
        {
            try
            {
                return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the service client.
        /// </summary>
        /// <typeparam name="TClientType">The type of the client type.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TClientType GetServiceClient<TClientType, T>()
            where TClientType : System.ServiceModel.ClientBase<T>, new()
            where T : class
        {
            try
            {
                string cacheKey = "_Client_Cache_" + typeof(TClientType).FullName;
                var client = GetValueFromCache(cacheKey.Trim()) as TClientType; ;
                if (client == null)
                {
                    client = new TClientType();
                    client.Open();
                    Utilities.AddValueToCache(cacheKey, client, 1440);
                }

                if (client.State != CommunicationState.Opened)
                    client.Open();

                return client;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;
            }
        }

       
        /// <summary>
        /// Gets the user ip address.
        /// </summary>
        /// <returns></returns>
        public static string GetUserIPAddress()
        {
            try
            {
                //The X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a 
                //client connecting to a web server through an HTTP proxy or load balancer
                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip.Contains(","))
                    ip = ip.Split(',')[0];

                return ip;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

        public static bool IsMobileBrowser()
        {

            try
            {

                //GETS THE CURRENT USER CONTEXT
                HttpContext context = HttpContext.Current;

                //FIRST TRY BUILT IN ASP.NT CHECK
                if (context.Request.Browser.IsMobileDevice)
                {
                    return true;
                }
                //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
                if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
                {
                    return true;
                }
                //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
                if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
                    context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
                {
                    return true;
                }
                //AND FINALLY CHECK THE HTTP_USER_AGENT 
                //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
                if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
                {
                    //Create a list of all mobile types
                    string[] mobiles =
                        new[]
                {
                    "midp", "j2me", "avant", "docomo",
                    "novarra", "palmos", "palmsource",
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/",
                    "blackberry", "mib/", "symbian",
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio",
                    "SIE-", "SEC-", "samsung", "HTC",
                    "mot-", "mitsu", "sagem", "sony"
                    , "alcatel", "lg", "eric", "vx",
                    "NEC", "philips", "mmm", "xx",
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", "java",
                    "pt", "pg", "vox", "amoi",
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo",
                    "sgh", "gradi", "jb", "dddi",
                    "moto", "iphone"
                };

                    //Loop through each item in the list created above 
                    //and check if the header contains that text
                    foreach (string s in mobiles)
                    {
                        if (context.Request.ServerVariables["HTTP_USER_AGENT"].
                                                            ToLower().Contains(s.ToLower()))
                        {
                            return true;
                        }
                    }
                }

                ////mobile switching on red server only(not live)
                //bool isMobileForTest = false;
                //bool.TryParse(Utilities.GetAppSetting("IsMobileForTest"), out isMobileForTest);
                //if (isMobileForTest)
                //{
                //    return true;
                //}
                ////end of mobile switching

                return false;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }

      

        /// <summary>
        /// Formats the decimal in case no fractional.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static decimal FormatDecimalInCaseNoFractional(decimal value)
        {
            try
            {
                int fractional = 0;
                decimal retValue = value;
                if (int.TryParse(value.ToString().Split('.')[1], out fractional))
                {
                    if (fractional == 0)
                        retValue = (int)value;
                }
                return retValue;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return value;
            }
        }

        /// <summary>
        /// Gets the user agent.
        /// </summary>
        /// <returns></returns>
        public static string GetUserAgent()
        {
            try
            {
                return HttpContext.Current.Request.UserAgent;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

       

        public static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
            }
        }

        public static string SendRequest(string url, string request, out HttpStatusCode statusCode)
        {
            statusCode = new HttpStatusCode();
            try
            {
                string response = string.Empty;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Method = "POST";
                try
                {
                    InitiateSSLTrust();
                }
                catch
                {

                }
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
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;
            }
        }
        
        /// <summary>
        /// Validates the mail.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <returns></returns>
        public static bool IsValidMail(string mail)
        {
            try
            {
                Regex regexEmail = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                if (regexEmail.IsMatch(mail))
                {
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }

        /// <summary>
        /// Gets the delimited string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static string GetDelimitedString<T>(IEnumerable<T> items, string delimiter)
        {
            var stringBuilder = new StringBuilder();

            foreach (var item in items)
                stringBuilder.AppendFormat("{0}{1}", item, delimiter);

            var delimiterLength = delimiter.Length;
            if (stringBuilder.Length >= delimiterLength)
                stringBuilder.Remove(stringBuilder.Length - delimiterLength, delimiterLength);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deletes the excel sheet files.
        /// </summary>
        public static void DeleteExcelSheetFiles()
        {
            try
            {
                string rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                string filesToDelete = @"*.xlsx";   // Only delete DOC files containing "DeleteMe" in their filenames
                string[] fileList = System.IO.Directory.GetFiles(rootFolderPath, filesToDelete);
                foreach (string file in fileList)
                {
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
            }
        }

        /// <summary>
        /// Encodes the hex16.
        /// </summary>
        /// <param name="srcBytes">The SRC bytes.</param>
        /// <returns></returns>
        public static string EncodeHex16(Byte[] srcBytes)
        {
            if (null == srcBytes)
            {
                throw new ArgumentNullException("byteArray");
            }
            string outputString = "";

            foreach (Byte b in srcBytes)
            {
                outputString += b.ToString("X2");
            }

            return outputString;
        }

        /// <summary>
        /// Decodes the hex16.
        /// </summary>
        /// <param name="srcString">The SRC string.</param>
        /// <returns></returns>
        public static Byte[] DecodeHex16(string srcString)
        {
            if (null == srcString)
            {
                throw new ArgumentNullException("srcString");
            }

            int arrayLength = srcString.Length / 2;

            Byte[] outputBytes = new Byte[arrayLength];

            for (int index = 0; index < arrayLength; index++)
            {
                outputBytes[index] = Byte.Parse(srcString.Substring(index * 2, 2), NumberStyles.AllowHexSpecifier);
            }

            return outputBytes;
        }


        public static Channel GetChannel(string channelName)
        {
            Channel channel = Channel.Portal;
            if (channelName.ToLower() == "Mobile".ToLower())
                channel = Channel.Mobile;
            else
                Enum.TryParse(channelName, out channel);
            return channel;
        }

        public static string GetAbsoluteUrlOrSelf(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return null;

                return new Uri(new Uri(Utilities.GetAppSetting("PublicSiteURL")), url).AbsoluteUri;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return url;
            }
        }

        public static string GetAbsoluteUrlOrSelf(string url, Uri baseUri)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return null;

                if (baseUri == null)
                    return url;

                return new Uri(baseUri, url).AbsoluteUri;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return url;
            }
        }

        public static string GetPageOutput(string pageUrl)
        {
            try
            {
                var absoluteUrl = Utilities.GetAbsoluteUrlOrSelf(pageUrl);

                var webResponse = WebRequest.Create(absoluteUrl).GetResponse();
                using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                    return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }

        /// <summary>
        /// checks browser and it's version used
        /// </summary>
        /// <returns></returns>
        public static bool IsSupportedBrowser()
        {
            try
            {
                var browser = HttpContext.Current.Request.Browser;
                decimal numericBrowserVersion;
                decimal.TryParse(HttpContext.Current.Request.Browser.Version, out numericBrowserVersion);
                //check browsers and their supported versions
                if ((browser.Browser.ToLower().Contains("ie") || browser.Browser.ToLower().Contains("internetexplorer")) && numericBrowserVersion >= 10)
                {
                    return true;
                }
                else if (browser.Browser.ToLower().Contains("firefox") && numericBrowserVersion >= 43)
                {
                    return true;
                }
                // chrome for android
                else if (HttpContext.Current.Request.UserAgent.ToLower().Contains("android") && browser.Browser.ToLower().Contains("chrome") && (numericBrowserVersion >= 49))
                {
                    return true;
                }
                //android browser
                else if (HttpContext.Current.Request.UserAgent.ToLower().Contains("android") && numericBrowserVersion >= 4.4m)
                {
                    return true;
                }
                //chrome for iOS
                else if (!HttpContext.Current.Request.UserAgent.ToLower().Contains("android") && HttpContext.Current.Request.UserAgent.ToLower().Contains("crios/") && !HttpContext.Current.Request.UserAgent.ToLower().Contains("opr/") && !HttpContext.Current.Request.UserAgent.ToLower().Contains("edge/"))
                {
                    int strartIndex = HttpContext.Current.Request.UserAgent.ToLower().IndexOf("crios/") > 0 ? HttpContext.Current.Request.UserAgent.ToLower().IndexOf("crios/") : 0;
                    string chromeVersionForIOS = HttpContext.Current.Request.UserAgent.ToLower().Substring((strartIndex + 6), 4);
                    decimal.TryParse(chromeVersionForIOS, out numericBrowserVersion);
                    if (numericBrowserVersion >= 45)
                        return true;
                    return false;
                }
                //chrome in general but not android
                else if (!HttpContext.Current.Request.UserAgent.ToLower().Contains("android") && browser.Browser.ToLower().Contains("chrome") && numericBrowserVersion >= 45 && !HttpContext.Current.Request.UserAgent.ToLower().Contains("opr/") && !HttpContext.Current.Request.UserAgent.ToLower().Contains("edge/"))
                {
                    return true;
                }
                else if (HttpContext.Current.Request.UserAgent.ToLower().Contains("mac") && browser.Browser.ToLower().Contains("safari") && (numericBrowserVersion >= 8.4m))
                {
                    return true;
                }
                // for mobile devices
                else if (HttpContext.Current.Request.UserAgent.ToLower().Contains("iphone") && browser.Browser.ToLower().Contains("safari") && numericBrowserVersion >= 8.4m)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return false;
            }
        }


        /// <summary>
        /// Gets the current browserinfo.
        /// </summary>
        /// <returns></returns>
        public static BrowserInfo GetCurrentBrowserinfo()
        {
            BrowserInfo info = new BrowserInfo();
            try
            {
                var browser = HttpContext.Current.Request.Browser;
                decimal numericBrowserVersion;
                decimal.TryParse(HttpContext.Current.Request.Browser.Version, out numericBrowserVersion);

                if ((browser.Browser.ToLower().Contains("ie") || browser.Browser.ToLower().Contains("internetexplorer")))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.IE;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                if (HttpContext.Current.Request.UserAgent.ToLower().Contains("windows phone"))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.WindowsPhone;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                if (HttpContext.Current.Request.UserAgent.ToLower().Contains("android"))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.Android;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                if (HttpContext.Current.Request.UserAgent.ToLower().Contains("ipad"))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.IPAD;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                if (HttpContext.Current.Request.UserAgent.ToLower().Contains("iphone"))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.Iphone;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                if (browser.Browser.ToLower().Contains("firefox"))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.FireFox;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                if (browser.Browser.ToLower().Contains("chrome"))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.Chrome;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                if (HttpContext.Current.Request.UserAgent.ToLower().Contains("mac"))
                {
                    info.ErrorCode = BrowserInfo.ErrorCodes.Success;
                    info.ErrorDescription = "Success";
                    info.BrowserType = BrowserInfo.Type.MAC;
                    info.BrowserVersion = numericBrowserVersion;
                    return info;
                }
                info.ErrorCode = BrowserInfo.ErrorCodes.Other;
                info.ErrorDescription = "Other Devices";
                info.BrowserType = BrowserInfo.Type.None;
                info.BrowserVersion = numericBrowserVersion;
                return info;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                info.ErrorCode = BrowserInfo.ErrorCodes.ServiceException;
                info.ErrorDescription = exp.Message;
                return info;
            }
        }

        public static string English
        {
            get
            {
                return Strings.English;
            }
        }

        public static string Arabic
        {
            get
            {
                return Strings.Arabic;
            }
        }
        public static string DataUnitFeatureId
        {
            get
            {
                return GetAppSetting("DataUnitFeatureId");
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////
        ///
        public static RegistrationOutput BeginRegistration(Profile profile, string emailSubject, string emailBody, string sMSBody, Channel channel, string lang, string userAgent, string userIP, string serverIP)//lang=ar;=en
        {
            RegistrationOutput output = new RegistrationOutput();
            output.RequestID = Guid.NewGuid();
            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.RequestID = output.RequestID;
            log.Channel = channel.ToString();
            log.Method = ProfileMethods.BeginRegistratration.ToString();
            log.UserAgent = userAgent;
            log.UserIP = userIP;
            log.Email = profile.Email;
            log.Dial = profile.Dial;
            log.ServerIP = serverIP;
            try
            {
                if (string.IsNullOrEmpty(profile.Dial))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyDial;
                    output.ErrorDescription = "Dial is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;

                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(profile.Email))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyEmail;
                    output.ErrorDescription = "Email is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(emailSubject))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Registraion email subject is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(emailBody))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Registraion email body is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(sMSBody))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Registraion SMS is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (!Utilities.IsValidDial(profile.Dial))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.InvlaidDialFormat;
                    output.ErrorDescription = "Invalid dial format";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }

                Regex regexEmail = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                if (!regexEmail.IsMatch(profile.Email))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.InvalidEmailFormat;
                    output.ErrorDescription = "Invalid email format";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                // Check if dial exist
                // Check if user exist in users table
                var userInfo = Mobinil.MyAccount.Profile.Membership.Provider.GetUserInfo(profile.Dial);
                if (userInfo != null && (!userInfo.IsGuest.HasValue || !userInfo.IsGuest.Value))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.DuplicateDial;
                    output.ErrorDescription = "Dial already registered";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                // Check if dial is primary dial in user dials
                var dialData = UserDialsDataAccess.GetDialNumberInfoByDialNumber(profile.Dial);
                if (dialData != null && (!dialData.IsGuest.HasValue || !dialData.IsGuest.Value) && (dialData.IsPrimary.HasValue && dialData.IsPrimary.Value))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.DialIsPrimaryInUserDials;
                    output.ErrorDescription = "Dial is primary dial in user dials";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                // Check if email exist
                if (Mobinil.MyAccount.Profile.Membership.Provider.IsEmailExists(profile.Email))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.DuplicateEmail;
                    output.ErrorDescription = "Email already exists";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                // Get data from Mobinil backend
                // Check Dial is Mobinil or Not
                bool isNoneMobinil = false;
                DialRegisteredInfoOutput dialInfo = DialInfoRetrieval.GetDialRegisteredInfo(profile.Dial, channel, ModulesNames.BeginRegistratration, output.RequestID);
                if (dialInfo.ErrorCode != DialRegisteredInfoOutput.ErrorCodes.Success)
                {
                    if (dialInfo.ErrorCode == DialRegisteredInfoOutput.ErrorCodes.NonMobinil || dialInfo.ErrorCode == DialRegisteredInfoOutput.ErrorCodes.NoContract || dialInfo.ErrorCode == DialRegisteredInfoOutput.ErrorCodes.InvalidDial)
                    {
                        isNoneMobinil = true;
                    }
                    else
                    {
                        output.ErrorCode = RegistrationOutput.ErrorCodes.EAIServiceDown;
                        output.ErrorDescription = "Get dial registered info" + dialInfo.ErrorDescription;

                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                        return output;
                    }
                }
                log.IsMobinil = !isNoneMobinil;
                log.RatePlanID = dialInfo.TariffPlan;
                if ((dialInfo.DialStatus == DialStatus.PermentlyDeactive || dialInfo.DialStatus == DialStatus.Deactive || dialInfo.DialStatus == DialStatus.VoluntarySuspended || dialInfo.DialStatus == DialStatus.InvoluntarySuspended))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.DialIsDeactivated;
                    output.ErrorDescription = dialInfo.ErrorDescription;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }

                profile.IsMobinil = !isNoneMobinil;
                profile.RatePlanID = dialInfo.TariffPlan;

                string generatedPassword = string.Empty;
                if (channel == Channel.OrangeMoney)
                    generatedPassword = Utilities.GenerateStrongPasswordForOrangeMoney(7);
                else
                    generatedPassword = Utilities.GenerateStrongPassword(8);

                log.RandomPassword = SecurityUtilities.HashData(generatedPassword.ToLower() + ProfileUtilities.ProfileHashKey, null);

                bool passwordSent = false;
                if (isNoneMobinil)
                {
                    emailBody = emailBody.Replace("[%MobileNumber%]", profile.Dial);
                    emailBody = emailBody.Replace("[%DialNumber%]", profile.Dial);
                    emailBody = emailBody.Replace("[%password%]", generatedPassword);
                    emailBody = emailBody.Replace("{0}", generatedPassword);
                    passwordSent = MailUtilities.SendMail(emailBody, emailSubject, MailUtilities.AdminEmail, profile.Email);
                }
                else
                {
                    sMSBody = string.Format(sMSBody, generatedPassword);
                    passwordSent = SMSUtilities.SendSMSMsgEcare(profile.Dial, sMSBody, lang.ToLower() == "en" ? ProfileEn.SMSLanguage : ProfileAr.SMSLanguage, SMSUtilities.SMSTitle);
                }
                if (!passwordSent)
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.PasswordNotSent;
                    output.ErrorDescription = "The random generated password not sent";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                output.ProfileInfo = profile;
                output.ErrorCode = RegistrationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                output.ErrorCode = RegistrationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = String.Format("Service exception: {0}", exp.Message);

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                return output;
            }
        }


        public static RegistrationOutput EndRegistration(Guid requestID, string randomPassword, string thanksEmailSubject, string thanksEmailBody, string subDialRegMailSubject, string subDialRegMailBody, string subDialRegSMS, Channel channel, string lang, string userAgent, string userIP, string serverIP)
        {
            RegistrationOutput output = new RegistrationOutput();
            output.ProfileInfo = new Profile();
            output.RequestID = requestID;

            RegistrationRequestsLog log = new RegistrationRequestsLog();
            log.RequestID = output.RequestID;
            log.Channel = channel.ToString();
            log.Method = ProfileMethods.EndRegistration.ToString();
            log.UserAgent = userAgent;
            log.ServerIP = serverIP;
            log.UserIP = userIP;

            try
            {
                if (requestID == null || requestID == Guid.Empty)
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "RequestID is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(thanksEmailSubject))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Thanks email subject is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(thanksEmailBody))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Thanks email body is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(subDialRegMailSubject))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "SubDial registraion email subject is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(subDialRegMailBody))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "SubDial registraion email body is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                if (string.IsNullOrEmpty(subDialRegSMS))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "SubDial registraion SMS is empty";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                RegistrationRequestsLog request = RegistrationRequestsLogsDataAccess.GetLogsByRequestID(requestID);
                if (request == null)
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.NoDataForRequest;
                    output.ErrorDescription = "No request for this request ID " + requestID;

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                output.ProfileInfo.Dial = request.Dial;
                output.ProfileInfo.Email = request.Email;
                output.ProfileInfo.IsMobinil = request.IsMobinil;
                output.ProfileInfo.RatePlanID = request.RatePlanID;

                log.Dial = request.Dial;
                log.Email = request.Email;

                int tokenLifeTimeInSeconds = 0;
                int.TryParse(Utilities.GetAppSetting("RegistrationTokenLifeTimeInSeconds"), out tokenLifeTimeInSeconds);
                if (tokenLifeTimeInSeconds == 0)
                    tokenLifeTimeInSeconds = 180;
                if (DateTime.Now.CompareTo(request.CreatedDate.AddSeconds(tokenLifeTimeInSeconds)) > 0)
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.RequestExpired;
                    output.ErrorDescription = "Request expired it took more than " + tokenLifeTimeInSeconds + " seconds";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }


                if (!SecurityUtilities.VerifyHashedData(request.RandomPassword, randomPassword.ToLower() + ProfileUtilities.ProfileHashKey))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.InvalidRandomPassword;
                    output.ErrorDescription = "Invalid random password";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                // Check if dial exist
                // if (Mobinil.MyAccount.Profile.Membership.Provider.IsDialExists(request.Dial))
                //{
                var userInfo = Mobinil.MyAccount.Profile.Membership.Provider.GetUserInfo(request.Dial);
                if (userInfo != null && (!userInfo.IsGuest.HasValue || !userInfo.IsGuest.Value))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.DuplicateDial;
                    output.ErrorDescription = "Dial already registered";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                // Check if email exist
                if (Mobinil.MyAccount.Profile.Membership.Provider.IsEmailExists(request.Email))
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.DuplicateEmail;
                    output.ErrorDescription = "Email already exists";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                // Check if dial is primary dial in user dials


                //bool? isPrimary = UserDialsDataAccess.IsPrimaryDial(request.Dial);
                //if (isPrimary.HasValue && isPrimary.Value)
                //{
                var dialData = UserDialsDataAccess.GetDialNumberInfoByDialNumber(request.Dial);
                if (dialData != null && (!dialData.IsGuest.HasValue || !dialData.IsGuest.Value) && (dialData.IsPrimary.HasValue && dialData.IsPrimary.Value))
                {

                    output.ErrorCode = RegistrationOutput.ErrorCodes.DialIsPrimaryInUserDials;
                    output.ErrorDescription = "Dial is primary dial in user dials";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                //delete gust account
                if (dialData != null && dialData.IsGuest.HasValue && dialData.IsGuest.Value)
                {
                    bool? isDeletedFromUsers = Mobinil.MyAccount.Profile.Membership.Provider.DeleteUserWithSubDials(userInfo.UserName);

                    if (!isDeletedFromUsers.HasValue || !isDeletedFromUsers.Value)
                    {
                        output.ErrorCode = RegistrationOutput.ErrorCodes.UnableToDeleteGuestAccount;
                        output.ErrorDescription = "Can not delete dial as user exist as a guest before in users table";

                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                        return output;
                    }
                }
                // Register User
                MembershipCreateStatus status;

                int errorCode;

                if (channel != Channel.OrangeMoney)
                    randomPassword = randomPassword.ToLower();
                MembershipUser membershipUser = Mobinil.MyAccount.Profile.Membership.Provider.CreateUser(request.Dial, randomPassword, request.Email, null, null, true, null, request.IsMobinil, false, request.RatePlanID, string.Empty, string.Empty, null, userIP, channel.ToString(), serverIP, userAgent, out errorCode, out status);
                if (status != MembershipCreateStatus.Success && errorCode == 0)
                {
                    output.ErrorCode = RegistrationOutput.ErrorCodes.ProviderError;
                    output.ErrorDescription = "Error while creating membership user";

                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                    return output;
                }
                output.ProfileInfo.UserID = (Guid)membershipUser.ProviderUserKey;
                if (status == MembershipCreateStatus.Success && errorCode == 2)
                {
                    DialRegisteredInfoOutput InfoFromEai = DialInfoRetrieval.GetDialRegisteredInfo(membershipUser.UserName, channel, ModulesNames.EndRegistration, output.RequestID);
                    if (InfoFromEai.ErrorCode != DialRegisteredInfoOutput.ErrorCodes.Success)
                    {
                        string emailBody = subDialRegMailBody;
                        emailBody = emailBody.Replace("[%SiteUrl%]", Utilities.SiteURL).Replace("[%Dial%]", request.Dial);
                        MailUtilities.SendMail(emailBody, subDialRegMailSubject, MailUtilities.AdminEmail, membershipUser.Email);
                    }
                    else
                    {
                        string SMSBody = string.Format(subDialRegSMS).Replace("[%Dial%]", request.Dial);
                        SMSUtilities.SendSMSMsgEcare(membershipUser.UserName, SMSBody, lang.ToLower() == "en" ? ProfileEn.SMSLanguage : ProfileAr.SMSLanguage, SMSUtilities.SMSTitle);
                    }
                }
                //var dialInfo = UserDialsDataAccess.GetDialNumberInfoByDialNumber(request.Dial);
                //if (dialInfo != null)
                //{
                //    bool? isDeleted = UserDialsDataAccess.DeleteUserDialByDialNo(dialInfo.UserID.Value, request.Dial);
                //    if (!isDeleted.HasValue || !isDeleted.Value)
                //    {
                //        output.ErrorCode = RegistrationOutput.ErrorCodes.UnableToDeleteUserDial;
                //        output.ErrorDescription = "Can not delete sub dial";

                //        log.ErrorCode = (int)output.ErrorCode;
                //        log.ErrorDescription = output.ErrorDescription;
                //        RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);

                //        // Delete from users table
                //        Mobinil.MyAccount.Profile.Membership.Provider.DeleteUser(request.Dial, false);

                //        return output;
                //    }
                //    var primaryDialUser = Mobinil.MyAccount.Profile.Membership.Provider.GetUser(dialInfo.UserID, false);
                //    if (primaryDialUser != null)
                //    {
                //        DialRegisteredInfoOutput InfoFromEai = DialInfoRetrieval.GetDialRegisteredInfo(primaryDialUser.UserName, channel, ModulesNames.EndRegistration, output.RequestID);
                //        if (InfoFromEai.ErrorCode != DialRegisteredInfoOutput.ErrorCodes.Success)
                //        {
                //            string emailBody = subDialRegMailBody;
                //            emailBody = emailBody.Replace("[%SiteUrl%]", Utilities.SiteURL).Replace("[%Dial%]", request.Dial);
                //            MailUtilities.SendMail(emailBody, subDialRegMailSubject, MailUtilities.AdminEmail, primaryDialUser.Email);
                //        }
                //        else
                //        {
                //            string SMSBody = string.Format(subDialRegSMS).Replace("[%Dial%]", request.Dial);
                //            SMSUtilities.SendSMSMsgEcare(primaryDialUser.UserName, SMSBody, lang.ToLower() == "en" ? ProfileEn.SMSLanguage : ProfileAr.SMSLanguage, SMSUtilities.SMSTitle);
                //        }
                //    }
                //}
                //// Add User Primary Dial
                //UserDial dial = new UserDial();
                //dial.Dial = request.Dial;
                //dial.IsPrimary = true;
                //dial.IsMobinil = request.IsMobinil;
                //dial.DialStatus = true;
                //dial.Channel = channel.ToString();
                //dial.UserAgent = userAgent;
                //dial.ServerIP = serverIP;
                //dial.UserIP = userIP;
                //dial.UserID = (Guid)membershipUser.ProviderUserKey;
                //dial.IsDeleted = false;
                //if (!UserDialsDataAccess.AddUserDial(dial))
                //{
                //    output.ErrorCode = RegistrationOutput.ErrorCodes.AddDialError;
                //    output.ErrorDescription = "Error while creating user dial";

                //    log.ErrorCode = (int)output.ErrorCode;
                //    log.ErrorDescription = output.ErrorDescription;
                //    RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);

                //    // Delete from users table
                //    Mobinil.MyAccount.Profile.Membership.Provider.DeleteUser(request.Dial, false);

                //    return output;
                //}

                if (request.RatePlanID != "20") //exclude prepaid from e-bill APIs call as per mobinil team 
                {
                    EbillDataAccess.CreateUserAtEbill(request.Dial, output.ProfileInfo.UserID, channel, ModulesNames.EndRegistration, serverIP, output.ProfileInfo.Email);
                    EbillDataAccess.UpdateUserAtEbill(request.Dial, output.ProfileInfo.UserID, channel, serverIP, request.Email, string.Empty, string.Empty, Utilities.GetCurrentLanguage().ToLower() == "en" ? "eng" : "ar-ae");
                }

                InfoRegisterationDataAccess.InfoRegisteration(request.Dial, request.Dial, request.Email, string.Empty, string.Empty, channel, ModulesNames.EndRegistration, requestID, userIP, serverIP, userAgent);

                //EnrollmentDataAccess.EnrollDial(request.Dial, ModulesNames.EndRegistration, channel, serverIP, userAgent, userID, userIP);

                // Send thanxks email
                thanksEmailBody = thanksEmailBody.Replace("[%SiteUrl%]", Utilities.SiteURL).Replace("[%Dial%]", request.Dial);
                MailUtilities.SendMail(thanksEmailBody, thanksEmailSubject, MailUtilities.AdminEmail, request.Email);

                output.ErrorCode = RegistrationOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                return output;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                output.ErrorCode = RegistrationOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = String.Format("Service exception: {0}", exp.Message);

                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                RegistrationRequestsLogsDataAccess.AddToRegistrationRequestsLogs(log);
                return output;
            }
        }








    }
}
