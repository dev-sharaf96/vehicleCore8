using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Caching; //By Atheer
using System.Xml;
using System.Xml.Serialization;
using DeviceDetectorNET;
using System.Data;
//using System.Management;         //By Atheer
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Tameenk.Common.Utilities
{
    public class Utilities
    {
        public const string InternationalPhoneCode = "00";
        public const string InternationalPhoneSymbol = "+";
        public const string Zero = "0";
        public const string SaudiInternationalPhoneCode = "966";
        //public static string SiteURL
        //{
        //    get
        //    {
        //        try
        //        {
        //            string URL = HttpContext.Current.Request.Url.Host;
        //            int Port = HttpContext.Current.Request.Url.Port;
        //            string strPort = ":" + Port;
        //            string Protocol = "http://";
        //            if (HttpContext.Current.Request.IsSecureConnection || !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["X-Forwarded-For"]) || Port == 80)
        //            {
        //                if (HttpContext.Current.Request.IsSecureConnection || !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["X-Forwarded-For"]))
        //                {
        //                    Protocol = "https://";
        //                }
        //                strPort = string.Empty;
        //            }
        //            return Protocol + URL + strPort;
        //        }
        //        catch
        //        {
        //            return "https://www.bcare.com.sa";
        //        }
        //    }
        //}
        //By Atheer


        /// <summary>
        /// Gets a value indicating whether this instance is secure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is secure; otherwise, <c>false</c>.
        /// </value>
        //public static bool IsSecure
        //{
        //    get
        //    {
        //        return HttpContext.Current.Request.IsSecureConnection ||
        //            HttpContext.Current.Request.ServerVariables["HTTPS"].Equals("on", StringComparison.InvariantCultureIgnoreCase);
        //    }
        //}
        //By Atheer
        //public static string AuthorizationHeader
        //{
        //    get
        //    {
        //        return HttpContext.Current.Request.Headers["Authorization"];
        //    }
        //}
        //By Atheer

        /// <summary>
        /// Gets the app setting.
        /// </summary>
        /// <param name="strKey">The STR key.</param>
        /// <returns></returns>
        //public static string GetAppSetting(string strKey)
        //{
        //    try
        //    {
        //        string strValue =(string)GetValueFromCache(strKey);
        //        if (string.IsNullOrEmpty(strValue))
        //        {
        //            strValue = System.Configuration.ConfigurationManager.AppSettings[strKey];
        //            if (!string.IsNullOrEmpty(strValue))
        //            {
        //                AddValueToCache(strKey, strValue, 300);
        //            }
        //        }
        //        return strValue;
        //    }
        //    catch(Exception exp)
        //    {
        //        ErrorLogger.LogError(exp.Message, exp, false);
        //        return string.Empty;
        //    }
        //}
        //By Atheer

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
                strOutput = Regex.Replace(Input, "<(.|\n)+?>", string.Empty);
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
                return HtmlEncoder.Default.Encode(Input);
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
        //public static void AddCookie(string name, string value, bool isSecure, int expireAfterInDays, string domainName)
        //{
        //    HttpCookie cookie = new HttpCookie(name);
        //    cookie.HttpOnly = true;
        //    cookie.Value = value;
        //    cookie.Secure = isSecure;
        //    if (!expireAfterInDays.Equals(0))
        //    {
        //        cookie.Expires = DateTime.Now.AddDays(expireAfterInDays);
        //    }
        //    if (!string.IsNullOrEmpty(domainName))
        //    {
        //        cookie.Domain = domainName;
        //    }
        //    HttpContext.Current.Response.Cookies.Add(cookie);
        //}
        //By Atheer

        /// <summary>
        /// Adds the cookie.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isSecure"></param>
        /// <param name="expireAfterInDays"></param>
        //public static void AddCookie(string name, string value, bool isSecure, int expireAfterInDays)
        //{
        //    AddCookie(name, value, isSecure, expireAfterInDays, string.Empty);
        //}
        //By Atheer

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        //public static string GetCookie(string name)
        //{
        //    if (HttpContext.Current.Request.Cookies[name] != null)
        //        return HttpContext.Current.Request.Cookies[name].Value;
        //    else
        //        return string.Empty;
        //}
        //By Atheer

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
                if (brief.Substring(0, maxLength + 1).EndsWith(string.Empty))
                    return brief.Substring(0, maxLength);
                for (int i = 0; i < 20; i++)
                {
                    if (maxLength - 1 - i > 0)
                        newBrief = brief.Substring(0, maxLength - i);
                    else
                        newBrief = string.Empty;

                    if (newBrief.EndsWith(string.Empty))
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
        //public static object GetValueFromCache(string CacheKey)
        //{

        //    if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Cache == null)
        //        return null;

        //    if (System.Web.HttpContext.Current.Cache[CacheKey] != null)
        //    {
        //        return System.Web.HttpContext.Current.Cache[CacheKey];
        //    }
        //    return null;
        //}

        /// <summary>
        /// Add Value To Cache
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="obj"></param>
        //public static void AddValueToCache(string CacheKey, object obj)
        //{
        //    if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Cache == null)
        //        return;

        //    System.Web.Caching.Cache cache = System.Web.HttpContext.Current.Cache;
        //    lock (cache)
        //    {
        //        System.Web.HttpContext.Current.Cache.Add(CacheKey, obj, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        //    }
        //}
        //By Atheer

        /// <summary>
        /// Adds the value to cache.
        /// </summary>
        /// <param name="CacheKey">The cache key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="Minutes">The minutes.</param>
        //public static void AddValueToCache(string CacheKey, object obj, int Minutes)
        //{
        //    if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Cache == null)
        //        return;

        //    System.Web.Caching.Cache cache = System.Web.HttpContext.Current.Cache;
        //    lock (cache)
        //    {
        //        System.Web.HttpContext.Current.Cache.Add(CacheKey, obj, null, DateTime.Now.AddMinutes(Minutes), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        //    }
        //}
        //By Atheer

        /// <summary>
        /// Removes the cache.
        /// </summary>
        /// <param name="CacheKey">The cache key.</param>
        //public static void RemoveCache(string CacheKey)
        //{
        //    System.Web.HttpContext.Current.Cache.Remove(CacheKey);
        //}
        //By Atheer

        /// <summary>
        /// Removes all cache key.
        /// </summary>
        //public static void RemoveAllCacheKey()
        //{
        //    foreach (DictionaryEntry key in System.Web.HttpContext.Current.Cache)
        //    {
        //        System.Web.HttpContext.Current.Cache.Remove(key.Key.ToString());
        //    }
        //}
        //By Atheer

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
                    return HtmlEncoder.Default.Encode(result);
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
        //public static bool IsSafeUrl(string url)
        //{
        //    if (string.IsNullOrEmpty(url))
        //    {
        //        return false;
        //    }

        //    url = url.Trim().ToLower();

        //    string SafeUrls = GetAppSetting("SafeUrls");

        //    if (!string.IsNullOrEmpty(SafeUrls))
        //    {
        //        foreach (string s in SafeUrls.Split(';'))
        //        {
        //            if (url.StartsWith(s.Trim().ToLower()))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //
        //
        //    if (!url.StartsWith("/"))
        //        url = "/" + url;
        //
        //    if (url.StartsWith("/english/") || url.StartsWith("/en/") || url.StartsWith("/arabic/") || url.StartsWith("/ar/"))
        //    {
        //        return true;
        //    }
        //
        //    return false;
        //}
        //By Atheerl
        public static bool IsSafeUrl2(string url, out string normalizedUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                normalizedUrl = "";
                return false;
            }

            url = url.Trim().ToLower();

            string SafeUrls = "SafeUrls"; //GetAppSetting("SafeUrls");//By Atheer

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
            object objMessage = null;// HttpContext.GetGlobalResourceObject(resourceClassName, message, Thread.CurrentThread.CurrentCulture);//By Atheer
            if (objMessage != null)
            {
                messageLocal = objMessage.ToString();
            }
            return messageLocal;
        }

        /// <summary>

        //public static int BankTimeout
        //{
        //    get
        //    {
        //        int BankTimeout = 310 * 1000;
        //        if (int.TryParse(GetAppSetting("BankTimeout"), out BankTimeout))
        //        {
        //            return 310 * 1000;
        //        }
        //        return BankTimeout;
        //    }
        //}
        //By Atheer

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
                return "LOCAL_ADDR";//HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"]; //By Atheer
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// Removes the zero from dial.
        /// </summary>
        /// <param name="dial">The dial.</param>
        /// <returns></returns>
        public static string RemoveZeroFromDial(string dial)
        {
            return dial.StartsWith("0") ? dial.Remove(0, 1).Trim().ToString() : dial;
        }

        public static string AddTwoToDial(string dial)
        {
            return dial.Insert(0, "2").Trim();
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
                string ip = "HTTP_X_FORWARDED_FOR";// HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];//By Atheer
                if (string.IsNullOrEmpty(ip))
                {
                    ip = "REMOTE_ADDR";// HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];//By Atheer
                }
                if (ip.Contains(","))
                    ip = ip.Split(',')[0];

                return ip;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //public static bool IsMobileBrowser()
        //{

        //    try
        //    {

        //        //GETS THE CURRENT USER CONTEXT
        //        HttpContext context = HttpContext.Current;

        //        //FIRST TRY BUILT IN ASP.NT CHECK
        //        if (context.Request.Browser.IsMobileDevice)
        //        {
        //            return true;
        //        }
        //        //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
        //        if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
        //        {
        //            return true;
        //        }
        //        //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
        //        if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
        //            context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
        //        {
        //            return true;
        //        }
        //        //AND FINALLY CHECK THE HTTP_USER_AGENT 
        //        //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
        //        if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
        //        {
        //            //Create a list of all mobile types
        //            string[] mobiles =
        //                new[]
        //        {
        //            "midp", "j2me", "avant", "docomo",
        //            "novarra", "palmos", "palmsource",
        //            "240x320", "opwv", "chtml",
        //            "pda", "windows ce", "mmp/",
        //            "blackberry", "mib/", "symbian",
        //            "wireless", "nokia", "hand", "mobi",
        //            "phone", "cdm", "up.b", "audio",
        //            "SIE-", "SEC-", "samsung", "HTC",
        //            "mot-", "mitsu", "sagem", "sony"
        //            , "alcatel", "lg", "eric", "vx",
        //            "NEC", "philips", "mmm", "xx",
        //            "panasonic", "sharp", "wap", "sch",
        //            "rover", "pocket", "benq", "java",
        //            "pt", "pg", "vox", "amoi",
        //            "bird", "compal", "kg", "voda",
        //            "sany", "kdd", "dbt", "sendo",
        //            "sgh", "gradi", "jb", "dddi",
        //            "moto", "iphone"
        //        };

        //            //Loop through each item in the list created above 
        //            //and check if the header contains that text
        //            foreach (string s in mobiles)
        //            {
        //                if (context.Request.ServerVariables["HTTP_USER_AGENT"].
        //                                                    ToLower().Contains(s.ToLower()))
        //                {
        //                    return true;
        //                }
        //            }
        //        }

        //        ////mobile switching on red server only(not live)
        //        //bool isMobileForTest = false;
        //        //bool.TryParse(Utilities.GetAppSetting("IsMobileForTest"), out isMobileForTest);
        //        //if (isMobileForTest)
        //        //{
        //        //    return true;
        //        //}
        //        ////end of mobile switching

        //        return false;
        //    }
        //    catch (Exception exp)
        //    {
        //        ErrorLogger.LogError(exp.Message, exp, false);
        //        return false;
        //    }
        //}

        //By Atheer 

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
                return "User-Agent";// HttpContext.Current.Request.Headers["User-Agent"].ToString();//By Atheer 
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

        public static bool IsValidPhoneNo(string dial)
        {

            Regex regexDial = new Regex(@"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$");
            return regexDial.IsMatch(dial);
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
        public static string SendComVivaRequest(string url, string request, out HttpStatusCode statusCode)
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
                Regex regexEmail = new Regex(@"\w+([-+.']\w+)*[.,-]?@\w+([-.]\w+)*\.\w+([-.]\w+)*");
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

        public static string GetAbsoluteUrlOrSelf(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return null;

                return "PublicSiteURL";// new Uri(new Uri(Utilities.GetAppSetting("PublicSiteURL")), url).AbsoluteUri; //By Atheer 
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
        //public static bool IsSupportedBrowser()
        //{
        //    try
        //    {
        //        var browser = HttpContext.Current.Request.Browser;
        //        decimal numericBrowserVersion;
        //        decimal.TryParse(HttpContext.Current.Request.Browser.Version, out numericBrowserVersion);
        //        //check browsers and their supported versions
        //        if ((browser.Browser.ToLower().Contains("ie") || browser.Browser.ToLower().Contains("internetexplorer")) && numericBrowserVersion >= 10)
        //        {
        //            return true;
        //        }
        //        else if (browser.Browser.ToLower().Contains("firefox") && numericBrowserVersion >= 43)
        //        {
        //            return true;
        //        }
        //        // chrome for android
        //        else if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("android") && browser.Browser.ToLower().Contains("chrome") && (numericBrowserVersion >= 49))
        //        {
        //            return true;
        //        }
        //        //android browser
        //        else if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("android") && numericBrowserVersion >= 4.4m)
        //        {
        //            return true;
        //        }
        //        //chrome for iOS
        //        else if (!HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("android") && HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("crios/") && !HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("opr/") && !HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("edge/"))
        //        {
        //            int strartIndex = HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().IndexOf("crios/") > 0 ? HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().IndexOf("crios/") : 0;
        //            string chromeVersionForIOS = HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Substring((strartIndex + 6), 4);
        //            decimal.TryParse(chromeVersionForIOS, out numericBrowserVersion);
        //            if (numericBrowserVersion >= 45)
        //                return true;
        //            return false;
        //        }
        //        //chrome in general but not android
        //        else if (!HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("android") && browser.Browser.ToLower().Contains("chrome") && numericBrowserVersion >= 45 && !HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("opr/") && !HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("edge/"))
        //        {
        //            return true;
        //        }
        //        else if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("mac") && browser.Browser.ToLower().Contains("safari") && (numericBrowserVersion >= 8.4m))
        //        {
        //            return true;
        //        }
        //        // for mobile devices
        //        else if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("iphone") && browser.Browser.ToLower().Contains("safari") && numericBrowserVersion >= 8.4m)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.Message, ex, false);
        //        return false;
        //    }
        //}
        //By Atheer 

        public static bool IsValidPin(string pin)
        {
            try
            {
                Regex regexDial = new Regex(@"^\d{6}$", RegexOptions.Compiled);
                return regexDial.IsMatch(pin);
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }

        /// <summary>
        /// Gets the current browserinfo.
        /// </summary>
        /// <returns></returns>
        //public static BrowserInfo GetCurrentBrowserinfo()
        //{
        //    BrowserInfo info = new BrowserInfo();
        //    try
        //    {
        //        var browser = HttpContext.Current.Request.Browser;
        //        decimal numericBrowserVersion;
        //        decimal.TryParse(HttpContext.Current.Request.Browser.Version, out numericBrowserVersion);
        //
        //        if ((browser.Browser.ToLower().Contains("ie") || browser.Browser.ToLower().Contains("internetexplorer")))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.IE;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("windows phone"))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.WindowsPhone;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("android"))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.Android;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("ipad"))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.IPAD;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("iphone"))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.Iphone;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        if (browser.Browser.ToLower().Contains("firefox"))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.FireFox;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        if (browser.Browser.ToLower().Contains("chrome"))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.Chrome;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        if (HttpContext.Current.Request.Headers["User-Agent"].ToString().ToLower().Contains("mac"))
        //        {
        //            info.ErrorCode = BrowserInfo.ErrorCodes.Success;
        //            info.ErrorDescription = "Success";
        //            info.BrowserType = BrowserInfo.Type.MAC;
        //            info.BrowserVersion = numericBrowserVersion;
        //            return info;
        //        }
        //        info.ErrorCode = BrowserInfo.ErrorCodes.Other;
        //        info.ErrorDescription = "Other Devices";
        //        info.BrowserType = BrowserInfo.Type.None;
        //        info.BrowserVersion = numericBrowserVersion;
        //        return info;
        //    }
        //    catch (Exception exp)
        //    {
        //        ErrorLogger.LogError(exp.Message, exp, false);
        //        info.ErrorCode = BrowserInfo.ErrorCodes.ServiceException;
        //        info.ErrorDescription = exp.Message;
        //        return info;
        //    }
        //}
        //By Atheer 

        public static DateTime? ConvertStringToDateTimeFromAllianz(string strValue)
        {
            DateTime dt;
            var value = strValue;
            var dateComponents = strValue.Split('-');
            if (dateComponents.Length > 2 && dateComponents[2].Length >= 4)
            {
                string year = strValue.Substring(0, 4);
                string month = strValue.Substring(5, 2);
                string day = strValue.Substring(8, 2);
                string hour = strValue.Substring(11, 2);
                string mintues = strValue.Substring(14, 2);
                string seconds = strValue.Substring(17, 2);
                value = year + "-" + month + "-" + day + " " + hour + ":" + mintues + ":" + seconds;

            }

            if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            else if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            else if (DateTime.TryParse(strValue, out dt))
            {
                return dt;
            }
            return null;
        }

        public static DateTime? ConvertStringToDateTimeFromMedGulf(string strValue)
        {
            DateTime dt;
            var value = strValue;
            var dateComponents = strValue.Split('-');
            if (dateComponents.Length > 2 && dateComponents[2].Length >= 4)
            {
                string year = strValue.Substring(0, 4);
                string month = strValue.Substring(5, 2);
                string day = strValue.Substring(8, 2);
                string hour = strValue.Substring(10, 2);
                string mintues = strValue.Substring(13, 2);
                string seconds = strValue.Substring(16, 2);
                value = year + "-" + month + "-" + day + " " + hour + ":" + mintues + ":" + seconds;

            }

            if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            else if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            else if (DateTime.TryParse(strValue,  out dt))
            {
                return dt;
            }
            return null;
        }
        public static string Remove966FromMobileNumber(string mobile)
        {
            mobile = mobile.Replace("+", "00");
            if (mobile.StartsWith("966"))
            {
                Regex rex = new Regex("966", RegexOptions.IgnoreCase);
                mobile = rex.Replace(mobile, "0", 1);
            }
            if (mobile.StartsWith("00966"))
            {
                Regex rex = new Regex("00966", RegexOptions.IgnoreCase);
                mobile = rex.Replace(mobile, "0", 1);
            }
            return mobile;
        }

        public static string SaveCompanyFile(string referenceId, byte[] file, string companyName, bool isPolicy, bool isAutoleasingPolicy)
        {
            try
            {
                #region Old Struncture

                //string generatedReportFileName = string.Empty;

                //int referenceLength = referenceId.Length;

                //generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                //  referenceId.Replace("-", "").Substring(0, referenceLength), companyName,
                //   DateTime.Now.ToString("HHmmss"), "pdf");

                //string generatedReportDirPath = Utilities.GetAppSetting("PdfCompaniesFilesBaseFolder");// @"D:\BcarePdf";
                //if (isPolicy)
                //    generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "Policies", DateTime.Now.Date.ToString("dd-MM-yyyy"), DateTime.Now.Hour.ToString());
                //else
                //    generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "Invoices", DateTime.Now.Date.ToString("dd-MM-yyyy"), DateTime.Now.Hour.ToString());

                //string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);
                //if (!Directory.Exists(generatedReportDirPath))
                //    Directory.CreateDirectory(generatedReportDirPath);

                //File.WriteAllBytes(generatedReportFilePath, file);
                //return generatedReportFilePath;

                #endregion

                string generatedReportDirPath = "PdfCompaniesFilesBaseFolder";// Utilities.GetAppSetting("PdfCompaniesFilesBaseFolder");//By Atheer 
                var subProductPath = isAutoleasingPolicy ? "Autolease" : "Individual";
                var dateNow = DateTime.Now.Date;

                if (isPolicy)
                    generatedReportDirPath = Path.Combine(generatedReportDirPath, subProductPath, companyName, "Policies", dateNow.Year.ToString(), dateNow.Month.ToString(), dateNow.Day.ToString(), DateTime.Now.Hour.ToString());
                else
                    generatedReportDirPath = Path.Combine(generatedReportDirPath, subProductPath, companyName, "Invoices", dateNow.Year.ToString(), dateNow.Month.ToString(), dateNow.Day.ToString(), DateTime.Now.Hour.ToString());

                int referenceLength = referenceId.Length;
                string generatedReportFileName = string.Format("{0}_{1}_{2}.{3}", referenceId.Replace("-", "").Substring(0, referenceLength), companyName, DateTime.Now.ToString("HHmmss"), "pdf");
                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);
                if (!Directory.Exists(generatedReportDirPath))
                    Directory.CreateDirectory(generatedReportDirPath);

                File.WriteAllBytes(generatedReportFilePath, file);
                return generatedReportFilePath;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

        public static string SaveCompanyFileFromDashboard(string referenceId, byte[] file, string companyName, bool isPolicy, bool isPdfServer, string domain, string serverIP, string username, string password, bool isAutoleasingPolicy, out string exception)
        {
            try
            {
                FileNetworkShare fileShare = new FileNetworkShare();

                exception = string.Empty;
                string generatedReportFileName = string.Empty;
                if (referenceId.Length == 10)
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                      referenceId.Replace("-", "").Substring(0, 10), companyName,
                       DateTime.Now.ToString("HHmmss"), "pdf");
                }
                else if (referenceId.Length == 13)
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                      referenceId.Replace("-", "").Substring(0, 13), companyName,
                       DateTime.Now.ToString("HHmmss"), "pdf");
                }
                else if (referenceId.Length == 8)
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                      referenceId.Replace("-", "").Substring(0, 8), companyName,
                       DateTime.Now.ToString("HHmmss"), "pdf");
                }
                else
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                     referenceId.Replace("-", "").Substring(0, 15), companyName,
                      DateTime.Now.ToString("HHmmss"), "pdf");
                }

                string generatedReportDirPath = "PdfCompaniesFilesBaseFolder";// Utilities.GetAppSetting("PdfCompaniesFilesBaseFolder");//By Atheer 
                var subProductPath = isAutoleasingPolicy ? "Autolease" : "Individual";
                var dateNow = DateTime.Now.Date;

                if (isPolicy)
                    generatedReportDirPath = Path.Combine(generatedReportDirPath, subProductPath, companyName, "Policies", dateNow.Year.ToString(), dateNow.Month.ToString(), dateNow.Day.ToString(), DateTime.Now.Hour.ToString());
                else
                    generatedReportDirPath = Path.Combine(generatedReportDirPath, subProductPath, companyName, "Invoices", dateNow.Year.ToString(), dateNow.Month.ToString(), dateNow.Day.ToString(), DateTime.Now.Hour.ToString());

                //string generatedReportDirPath = Utilities.GetAppSetting("PdfCompaniesFilesBaseFolder");// @"D:\BcarePdf";
                //if (isPolicy)
                //    generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "Policies", DateTime.Now.Date.ToString("dd-MM-yyyy"), DateTime.Now.Hour.ToString());
                //else
                //    generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "Invoices", DateTime.Now.Date.ToString("dd-MM-yyyy"), DateTime.Now.Hour.ToString());

                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);

                if (isPdfServer)
                {
                    string reportFilePath = generatedReportFilePath;
                    //generatedReportFilePath = serverIP + "\\" + generatedReportFilePath;
                    //generatedReportDirPath = serverIP + "\\" + generatedReportDirPath;
                    if (fileShare.UploadFileToShare(domain, username, password, generatedReportDirPath, generatedReportFilePath, file, serverIP, out exception))
                    {
                        return reportFilePath;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (!Directory.Exists(generatedReportDirPath))
                        Directory.CreateDirectory(generatedReportDirPath);

                    File.WriteAllBytes(generatedReportFilePath, file);
                    return generatedReportFilePath;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return string.Empty;
            }
        }

        public static string SaveQuotationFormFile(string externalId, byte[] file, string bankName, bool isPdfServer, string domain, string serverIP, string username, string password, out string exception)
        {
            try
            {
                FileNetworkShare fileShare = new FileNetworkShare();

                exception = string.Empty;
                string generatedReportFileName = string.Format("{0}_{1}_{2}.{3}", externalId, bankName, DateTime.Now.ToString("HHmmss"), "pdf"); ;
               
                string generatedReportDirPath = "QuotationFormFilesBaseFolder";// Utilities.GetAppSetting("QuotationFormFilesBaseFolder");// @"D:\BcarePdf"; //By Atheer 
                generatedReportDirPath = Path.Combine(generatedReportDirPath, bankName, "QuotationForm", DateTime.Now.Date.ToString("dd-MM-yyyy", new CultureInfo("en-US")), DateTime.Now.Hour.ToString());
                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);

                if (isPdfServer)
                {
                    string reportFilePath = generatedReportFilePath;
                    generatedReportFilePath = serverIP + "\\" + generatedReportFilePath;
                    generatedReportDirPath = serverIP + "\\" + generatedReportDirPath;
                    if (fileShare.SaveFileToShare(domain, username, password, generatedReportDirPath, generatedReportFilePath, file, serverIP, out exception))
                    {
                        return reportFilePath;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (!Directory.Exists(generatedReportDirPath))
                        Directory.CreateDirectory(generatedReportDirPath);

                    File.WriteAllBytes(generatedReportFilePath, file);
                    return generatedReportFilePath;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return string.Empty;
            }
        }
        public static string RemoveWhiteSpaces(string s)
        {
            return string.Join(" ", s.Split(new char[] { ' ' },
                   StringSplitOptions.RemoveEmptyEntries));
        }

        public static int GetSocialStatusId(string socialStatus)
        {
           if(socialStatus== "مطلقة" || socialStatus== "Divorced Female")
            {
                return 5;
            }
            if (socialStatus == "متزوجة" || socialStatus == "Married Female")
            {
                return 4;
            }
            if (socialStatus == "متزوج" || socialStatus == "Married Male")
            {
                return 2;
            }
            if (socialStatus == "غير متاح" || socialStatus == "Not Available")
            {
                return 0;
            }
            if (socialStatus == "غير ذلك" || socialStatus == "Other")
            {
                return 7;
            }
            if (socialStatus == "غير متزوجة" || socialStatus == "Single Female")
            {
                return 3;
            }
            if (socialStatus == "أعزب" || socialStatus == "Single Male")
            {
                return 1;
            }
            if (socialStatus == "ارملة" || socialStatus == "Widowed Female")
            {
                return 6;
            }
            return 1;
        }

        public static string Removemultiplespaces(string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }
        public static bool ValidPolicyEffectiveDate(DateTime policyEffectiveDate)
        {

            if (policyEffectiveDate < DateTime.Now.Date.AddDays(1) || policyEffectiveDate > DateTime.Now.AddDays(14))
            {
                return false;
            }
            return true;
        }

        public static string GoogleUrlShortener(string longUrl)
        {
            string key = "AIzaSyBhDtJotdkh3WB2QtZpyjYmtXh7Eoc2NKI";
            string finalURL = "";
            string post = "{\"longUrl\": \"" + longUrl + "\"}";
            string shortUrl = longUrl;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + key);
            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            string json = responseReader.ReadToEnd();

                            JObject jsonResult = JObject.Parse(json);
                            finalURL = jsonResult["id"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message,ex,false);
            }
            return finalURL;
        }
        public static string GetShortUrl(string i_sLongUrl)
        {
            string finalURL = string.Empty;
            try
            {
                string i_sBitlyUserName = "bcare2019";
                string i_sBitlyAPIKey = "R_40b626d0a62049099177ffc8a415b887";
                //Construct a valid URL and parameters to connect to Bitly Server
                StringBuilder sbURL = new StringBuilder("http://api.bitly.com/v3/shorten?");
                sbURL.Append("&format=xml");
                sbURL.Append("&longUrl=");
                sbURL.Append(HttpUtility.UrlEncode(i_sLongUrl));
                sbURL.Append("&login=");
                sbURL.Append(System.Web.HttpUtility.UrlEncode(i_sBitlyUserName));
                sbURL.Append("&apiKey=");
                sbURL.Append(System.Web.HttpUtility.UrlEncode(i_sBitlyAPIKey));

                HttpWebRequest objRequest = WebRequest.Create(sbURL.ToString()) as HttpWebRequest;
                objRequest.Method = "GET";
                objRequest.ContentType = "application/x-www-form-urlencoded";
                objRequest.ServicePoint.Expect100Continue = false;
                objRequest.ContentLength = 0;


                //Send the Request and Get the Response. The Response will have the status of operation and the bitlyURL
                WebResponse objResponse = objRequest.GetResponse();
                StreamReader myXML = new StreamReader(objResponse.GetResponseStream());
                dynamic xr = XmlReader.Create(myXML);

                //Retrieve the Status and URL from the Response
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(xr);

                string sStat = xdoc.ChildNodes[1].ChildNodes[1].ChildNodes[0].Value;
                if (sStat == "OK")
                {
                    finalURL= xdoc.ChildNodes[1].ChildNodes[2].ChildNodes[0].ChildNodes[0].Value;
                }
                //else
                //{
                //    return sStat;
                //}

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
            }
            return  finalURL;
        }

        public static string ValidatePhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith(InternationalPhoneCode))
                phoneNumber = phoneNumber.Substring(InternationalPhoneCode.Length);
            else if (phoneNumber.StartsWith(InternationalPhoneSymbol))
                phoneNumber = phoneNumber.Substring(InternationalPhoneSymbol.Length);

            if (!phoneNumber.StartsWith(SaudiInternationalPhoneCode))
            {
                if (phoneNumber.StartsWith(Zero))
                    phoneNumber = phoneNumber.Substring(Zero.Length);

                phoneNumber = SaudiInternationalPhoneCode + phoneNumber;
            }

            return phoneNumber;
        }
        public static bool IsValidIBAN(string iban)
        {
            try
            {
                iban = iban.ToLower().Replace("sa", "");

                if (iban.Length < 22)
                {
                    return false;
                }
                return true;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }

        //public static string SendRequestJson(string url, string request, out HttpStatusCode statusCode, string token = null)
        //{
        //    statusCode = new HttpStatusCode();
        //    try
        //    {
        //        string response = string.Empty;
        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //        httpWebRequest.ContentType = "application/json";
        //        httpWebRequest.Method = "POST";
        //        if (token != null)
        //            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
        //        try
        //        {
        //            InitiateSSLTrust();
        //        }
        //        catch
        //        {

        //        }
        //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            streamWriter.Write(request);
        //            streamWriter.Flush();
        //        }

        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        statusCode = httpResponse.StatusCode;
        //       // if (httpResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //            {
        //                response = streamReader.ReadToEnd();
        //            }
        //        }
        //        return response;
        //    }
        //    catch (Exception exp)
        //    {
        //        ErrorLogger.LogError(exp.Message, exp, false);
        //        return null;
        //    }
        //}
        public static string SendRequestJson(string url, string request, out HttpStatusCode statusCode, string token = null, string Method = "POST")
        {
            statusCode = new HttpStatusCode();
            try
            {
                string response = string.Empty;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = Method;
                if (token != null)
                    httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                try
                {
                    InitiateSSLTrust();
                }
                catch
                {

                }
                if (Method.ToLower() == "post")
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(request);
                        streamWriter.Flush();
                    }
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                statusCode = httpResponse.StatusCode;
                // if (httpResponse.StatusCode == HttpStatusCode.OK)
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
        public static string GetDecodeUrl(string url)
        {
            if (HttpUtility.UrlDecode(url).Replace(" ", "+") != url)
            {
                return HttpUtility.UrlDecode(url).Replace(" ", "+");
            }
            else
            {
                return url;
            }
        }

        public static string SaveComprehensiveImageFile(string referenceId, byte[] file, string companyName, int imageId, string imageSide)
        {
            var dateNow = DateTime.Now;
            var generatedImageFileName = string.Format("{0}_{1}.{2}", imageId, imageSide, "png");

            string generatedImageDirBasePath = "ComprehensiveImageFilesBaseFolder";//Utilities.GetAppSetting("ComprehensiveImageFilesBaseFolder");//By Atheer
            string relativePath = "ComprehensiveImageFilesRelativePath";// Utilities.GetAppSetting("ComprehensiveImageFilesRelativePath");//By Atheer
            string imageSecondryPath = Path.Combine(companyName, dateNow.Date.ToString("dd-MM-yyyy"), dateNow.Hour.ToString(), referenceId);
            generatedImageDirBasePath = Path.Combine(generatedImageDirBasePath + relativePath, imageSecondryPath);

            string generatedImageFilePath = Path.Combine(generatedImageDirBasePath, generatedImageFileName);
            if (!Directory.Exists(generatedImageDirBasePath))
                Directory.CreateDirectory(generatedImageDirBasePath);

            File.WriteAllBytes(generatedImageFilePath, file);

            return Path.Combine(relativePath, imageSecondryPath, generatedImageFileName).Replace("\\", "/");
        }
        public static string HandleHijriDate(string dateH)
        {
            string resultDateH = dateH;
            if (!string.IsNullOrEmpty(dateH))
            {
                var convertedDateOfBirthH = dateH.Split('-');
                if (convertedDateOfBirthH == null || convertedDateOfBirthH.Count() != 3)
                { return dateH; }

                int day = 0;
                Int32.TryParse(convertedDateOfBirthH[0], out day);
                int month = 0;
                Int32.TryParse(convertedDateOfBirthH[1], out month);
                int year = 0;
                Int32.TryParse(convertedDateOfBirthH[2], out year);

                if (day == 0 || month == 0 || year == 0)
                { return dateH; }

                if (day > 30 && (month == 1 || month == 3 || month == 5 || month == 7 || month == 9 || month == 11))
                {
                    resultDateH = String.Format("30-{0, 0:D2}-{1}", month, year);
                }
                else if (day > 29 && (month == 4 || month == 6 || month == 8 || month == 10 || month == 12))
                {
                    resultDateH = String.Format("29-{0, 0:D2}-{1}", month, year);
                }
                else if (day > 28 && month == 2)
                {
                    resultDateH = String.Format("28-{0, 0:D2}-{1}", month, year);
                }
            }
            return resultDateH;
        }

        //public static string GetCurrentURL
        //{
        //    get
        //    {
        //        return HttpContext.Current.Request.Url.AbsoluteUri;
        //    }
        //}//By Atheer
        public static string GetCurrentUrl(HttpContext httpContext)
        {
            try
            {
                if (httpContext != null)
                {
                    return httpContext.Request.GetDisplayUrl();
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
            public static bool IsBlockedUser(string userName,string userID,string userAgent)
        {
            if (string.IsNullOrEmpty(userName)||userName.ToLower()== "anonymous")
                return true;
            if (string.IsNullOrEmpty(userID))
                return true;
            if (string.IsNullOrEmpty(userAgent))
                return true;

            return false;
        }

        public static byte[] GetFileBytes(IFormFile file)
        {
            byte[] data;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                data = memoryStream.ToArray();
            }
            return data;
        }
        public static string SaveUserTicketAttachmentFile(int userTicketId, byte[] file, string extension, string prefix, bool isRemoteServer, string domain, string serverIP, string username, string password, out string exception)
        {
            try
            {
                FileNetworkShare fileShare = new FileNetworkShare();

                exception = string.Empty;

                var currentDate = DateTime.Now;
                var generatedUserTicketFileName = $"{userTicketId}_{currentDate.ToString("HHmmss", new CultureInfo("en-US"))}_{prefix}.{extension}";
                string userTicketDirPath = "UserTicketFilesBaseFolder";// Path.Combine(Utilities.GetAppSetting("UserTicketFilesBaseFolder"), currentDate.Date.ToString("dd-MM-yyyy", new CultureInfo("en-US")), currentDate.Hour.ToString(new CultureInfo("en-US")));

                string generatedUserTicketFilePath = Path.Combine(userTicketDirPath, generatedUserTicketFileName);

                if (isRemoteServer)
                {
                    //string reportFilePath = generatedUserTicketFilePath;
                    //generatedUserTicketFilePath = serverIP + "\\" + generatedUserTicketFilePath;
                    //userTicketDirPath = serverIP + "\\" + userTicketDirPath;
                    if (fileShare.UploadFileToShare(domain, username, password, userTicketDirPath, generatedUserTicketFilePath, file, serverIP, out exception))
                    {
                        return generatedUserTicketFilePath;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {

                    if (!Directory.Exists(userTicketDirPath))
                        Directory.CreateDirectory(userTicketDirPath);

                    File.WriteAllBytes(generatedUserTicketFilePath, file);
                    return generatedUserTicketFilePath;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return string.Empty;
            }
        }
        public static Channel GetChannel(string channel)
        {
            Channel channelValue = Channel.Portal;
            if (channel.ToLower() == "ios")
                channelValue = Channel.ios;
            if (channel.ToLower() == "android")
                channelValue = Channel.android;
            if (channel.ToLower() == "Mobile")
                channelValue = Channel.Mobile;
            if (channel.ToLower() == "dashboard")
                channelValue = Channel.Dashboard;
            return channelValue;
        }

        public static string FormatDateString(string dateString)
        {
            string formatedResult = string.Empty;
            try
            {
                if (dateString?.Length < 10 && dateString.Contains("-"))
                {
                    var day = dateString.Split('-')[0];
                    var month = dateString.Split('-')[1];
                    var year = dateString.Split('-')[2];
                    int d = 0;
                    int m = 0;
                    if (int.TryParse(dateString.Split('-')[0], out d))
                    {
                        if (d < 10 && d > 0)
                        {
                            day = "0" + day;
                        }
                        else if (d == 0)
                        {
                            day = "01";
                        }
                    }
                    if (int.TryParse(dateString.Split('-')[1], out m))
                    {
                        if (m < 10 && m > 0)
                        {
                            month = "0" + month;
                        }
                        else if (m == 0)
                        {
                            month = "01";
                        }
                    }
                    formatedResult = day + "-" + month + "-" + year;
                }
                else
                {
                    formatedResult = dateString;
                }
            }
            catch
            {

            }

            if (string.IsNullOrEmpty(dateString))
            {
                try
                {
                    System.Globalization.DateTimeFormatInfo HijriDTFI;
                    HijriDTFI = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
                    HijriDTFI.Calendar = new System.Globalization.UmAlQuraCalendar();
                    HijriDTFI.ShortDatePattern = "dd-MM-yyyy";
                    DateTime dt = DateTime.Now;
                    formatedResult = dt.ToString("dd-MM-yyyy", HijriDTFI);
                }
                catch
                {

                }
            }

            return formatedResult;
        }

        public static string SaveQuotationFormFile(string externalId, byte[] file, string companyName)
        {
            try
            {
                var generatedReportFileName = string.Format("{0}_{1}_{2}.{3}", externalId, companyName, DateTime.Now.ToString("HHmmss"), "pdf");
                string generatedReportDirPath = "UserTicketFilesBaseFolder";// Utilities.GetAppSetting("QuotationFormFilesBaseFolder");// @"D:\BcarePdf";//By Atheer
                generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "QuotationForm", DateTime.Now.Date.ToString("dd-MM-yyyy", new CultureInfo("en-US")), DateTime.Now.Hour.ToString());
                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);
                if (!Directory.Exists(generatedReportDirPath))
                    Directory.CreateDirectory(generatedReportDirPath);
                File.WriteAllBytes(generatedReportFilePath, file);
                return generatedReportFilePath;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }
        }

        public static string GetUrlReferrer(HttpContext httpContext)
        {
            try
            {
                Uri referrer = httpContext?.Request?.GetTypedHeaders()?.Referer;

                if (referrer != null)
                {
                    return referrer.Host;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SendRequestToQueue()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string response1= client.DownloadString("http://10.201.204.42:8000");
                    string response2 = client.DownloadString("http://10.201.11.173:8000");
                    string response3 = client.DownloadString("http://10.201.11.41:12000");//medical
                    string response4 = client.DownloadString("http://10.201.11.41:5000");//general
                    string response5 = client.DownloadString("http://10.201.11.41:10000");//general
                }
            }
            catch
            {

            }
        }
        //public static bool RemoveCookie(string name)
        //{
        //    try
        //    {
        //        HttpCookie cookieToRemove = new HttpCookie(name);
        //        if (cookieToRemove != null)
        //        {
        //            cookieToRemove.Value = string.Empty;
        //            cookieToRemove.Expires = DateTime.Now.AddDays(-1);
        //            HttpContext.Current.Response.Cookies.Add(cookieToRemove);
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        //By Atheer


        //public static string GetFullUrlReferrer()
        //{
        //    try
        //    {
        //        if (HttpContext.Current != null && HttpContext.Current.Request.UrlReferrer != null)
        //            return HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
        //
        //        return string.Empty;
        //    }
        //    catch
        //    {
        //        return string.Empty;
        //    }
        //}
        //By Atheer

        public static DeviceInfo GetDeviceInfo()
        {
            try
            {
                var deviceInfo = new DeviceDetector(Utilities.GetUserAgent());
                deviceInfo.Parse();
                if (deviceInfo == null)
                {
                    return null;
                }
                DeviceInfo deviceInfoModel = new DeviceInfo();
                if (deviceInfo.GetOs().Success)
                {
                    deviceInfoModel.OS = deviceInfo.GetOs().Match.Name;
                }
                if (deviceInfo.GetBrowserClient().Success)
                {
                    deviceInfoModel.Client = deviceInfo.GetBrowserClient().Match.Name;
                }
                return deviceInfoModel;
            }
            catch(Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;
            }
        }

        public static byte[] AddWaterMarkOverImage(byte[] image,string waterMark)
        {
            Stream stream = new MemoryStream(image);
            using (Bitmap bitmap = new Bitmap(stream, false))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Brush brush = new SolidBrush(Color.Red);
                    Font font = new Font("Arial", 90, FontStyle.Italic, GraphicsUnit.Pixel);
                    SizeF textSize = new SizeF();
                    textSize = graphics.MeasureString(waterMark, font);
                    Point position = new Point(bitmap.Width - ((int)textSize.Width + 10), bitmap.Height - ((int)textSize.Height + 10));
                    graphics.DrawString(waterMark, font, brush, position);
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        bitmap.Save(mStream, ImageFormat.Png);
                        mStream.Position = 0;
                        return mStream.ToArray();
                    }
                }
            }
        }
        //for portrait
        public static byte[] AddWaterMarkOverImage(byte[] image, string date, Bitmap bitmaplogo, string Time)
        {
            #region Handling Portrait img
            //ImageConverter converter = new ImageConverter();
            //Image x = (Bitmap)((new ImageConverter()).ConvertFrom(image));

            ////  portrait
            //// bool flage = true;
            //if (x.Height > x.Width)
            //{
            //    x.RotateFlip(RotateFlipType.Rotate90FlipNone);
            //    image = (byte[])converter.ConvertTo(x, typeof(byte[]));
            //    // flage = false;
            //}
            #endregion

            Stream stream = new MemoryStream(image);
            using (Bitmap bitmap = new Bitmap(stream, false))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Brush brush = new SolidBrush(Color.White);
                    Font font = new Font("Arial", 60, FontStyle.Regular, GraphicsUnit.Pixel);
                    SizeF textSize = new SizeF();
                    textSize = graphics.MeasureString(date, font);
                    //Point Dateposition = new Point(bitmap.Width - ((int)textSize.Width + 10), bitmap.Height / 10 - ((int)textSize.Height + 10));
                    //Point Timeposition = new Point(bitmap.Width - ((int)textSize.Width + 20), bitmap.Height / 15 - ((int)textSize.Height + 10));
                    Point Dateposition = new Point(bitmap.Width - (int)textSize.Width/*bitmap.Width - ((int)textSize.Width + 10)*/, bitmap.Height / 15 - ((int)textSize.Height + 10)/*bitmap.Height / 10 - ((int)textSize.Height + 10)*/);
                    Point Timeposition = new Point(bitmap.Width - (int)textSize.Width, 0);

                    Point logoposition = new Point(bitmap.Width - (bitmaplogo.Width), bitmap.Height - (bitmaplogo.Height));
                    graphics.DrawString(Time, font, brush, Timeposition);//timewatermark
                    graphics.DrawString(date, font, brush, Dateposition);//datewatermark
                                                                         //workaround to set opcity
                    ColorMatrix colormatrix = new ColorMatrix();
                    colormatrix.Matrix33 = .5f;//Opcity of Logo
                    ImageAttributes bitmaplogoAttribute = new ImageAttributes();
                    bitmaplogoAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    ////
                    // To Add Logo to the image
                    ///
                    graphics.DrawImage(bitmaplogo, new Rectangle(bitmap.Width - (bitmaplogo.Width + 5), bitmap.Height - (bitmaplogo.Height), bitmaplogo.Width, bitmaplogo.Height), 0, 0, bitmaplogo.Width, bitmaplogo.Height, GraphicsUnit.Pixel, bitmaplogoAttribute);
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        ////x.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        bitmap.Save(mStream, ImageFormat.Jpeg);
                        mStream.Position = 0;
                        return mStream.ToArray();
                    }
                }
            }
        }

        public static string SavePdfFile(string referenceId, byte[] file, string companyName, bool isPolicy, bool isPdfServer, string domain, string serverIP, string username, string password,DateTime folderDate, bool isAutoleasingPolicy, out string exception)
        {
            try
            {
                FileNetworkShare fileShare = new FileNetworkShare();

                exception = string.Empty;
                string generatedReportFileName = string.Empty;
                if (referenceId.Length == 10)
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                      referenceId.Replace("-", "").Substring(0, 10), companyName,
                      folderDate.ToString("HHmmss"), "pdf");
                }
                else if (referenceId.Length == 13)
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                      referenceId.Replace("-", "").Substring(0, 13), companyName,
                       folderDate.ToString("HHmmss"), "pdf");
                }
                else if (referenceId.Length == 8)
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                      referenceId.Replace("-", "").Substring(0, 8), companyName,
                      folderDate.ToString("HHmmss"), "pdf");
                }
                else if (referenceId.Length == 9)
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                      referenceId.Replace("-", "").Substring(0, 9), companyName,
                      folderDate.ToString("HHmmss"), "pdf");
                }
                else
                {
                    generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                     referenceId.Replace("-", "").Substring(0, 15), companyName,
                      folderDate.ToString("HHmmss"), "pdf");
                }

                //string generatedReportDirPath = Utilities.GetAppSetting("PdfCompaniesFilesBaseFolder");// @"D:\BcarePdf";
                //if (isPolicy)
                //    generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "Policies", folderDate.Date.ToString("dd-MM-yyyy", new CultureInfo("en-US")), folderDate.Hour.ToString());
                //else
                //    generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "Invoices", folderDate.Date.ToString("dd-MM-yyyy",new CultureInfo("en-US")), folderDate.Hour.ToString());

                string generatedReportDirPath = "PdfCompaniesFilesBaseFolder";// Utilities.GetAppSetting("PdfCompaniesFilesBaseFolder");//By Atheer
                var subProductPath = isAutoleasingPolicy ? "Autolease" : "Individual";
                var dateNow = DateTime.Now.Date;

                if (isPolicy)
                    generatedReportDirPath = Path.Combine(generatedReportDirPath, subProductPath, companyName, "Policies", dateNow.Year.ToString(), dateNow.Month.ToString(), dateNow.Day.ToString(), DateTime.Now.Hour.ToString());
                else
                    generatedReportDirPath = Path.Combine(generatedReportDirPath, subProductPath, companyName, "Invoices", dateNow.Year.ToString(), dateNow.Month.ToString(), dateNow.Day.ToString(), DateTime.Now.Hour.ToString());

                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);

                if (isPdfServer)
                {

                    if (fileShare.UploadFileToShare(domain, username, password, generatedReportDirPath, generatedReportFilePath, file, serverIP, out exception))
                    {
                        return generatedReportFilePath;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (!Directory.Exists(generatedReportDirPath))
                        Directory.CreateDirectory(generatedReportDirPath);

                    File.WriteAllBytes(generatedReportFilePath, file);
                    return generatedReportFilePath;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return string.Empty;
            }
        }
        /// <summary>
        /// For Save Cancelled Policy File
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="file"></param>
        /// <param name="companyName"></param>
        /// <param name="isPdfServer"></param>
        /// <param name="domain"></param>
        /// <param name="serverIP"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="exception"></param>
        /// <returns>FilePath</returns>
        public static string SaveCancelPolicyRequestAttachmentFromDashboard(string referenceId, byte[] file, string companyName, bool isPdfServer, string domain, string serverIP, string username, string password, out string exception)
        {
            try
            {
                exception = string.Empty;
                //updated by a.hussein 8-7-2021
                if (file == null)
                {
                    return "there is no file";
                }
                FileNetworkShare fileShare = new FileNetworkShare();
                string generatedReportFileName = string.Format("{0}_{1}_{2}.{3}",
                referenceId.Replace("-", ""), companyName,
                DateTime.Now.ToString("HHmmss"), "pdf");
                string generatedReportDirPath = "CancelPolicyRequestAttachments";// Utilities.GetAppSetting("CancelPolicyRequestAttachments");// @"c:\CancelPolicyRequestAttachment";//By Atheer
                generatedReportDirPath = Path.Combine(generatedReportDirPath, companyName, "CancelPolicyRequestAttachment", DateTime.Now.Date.ToString("dd-MM-yyyy"), DateTime.Now.Hour.ToString());
                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);
                if (isPdfServer)
                {
                    string reportFilePath = generatedReportFilePath;
                    generatedReportFilePath = serverIP + "\\" + generatedReportFilePath;
                    generatedReportDirPath = serverIP + "\\" + generatedReportDirPath;
                    if (fileShare.UploadFileToShare(domain, username, password, generatedReportDirPath, generatedReportFilePath, file, serverIP, out exception))
                    {
                        return reportFilePath;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (!Directory.Exists(generatedReportDirPath))
                        Directory.CreateDirectory(generatedReportDirPath);
                    File.WriteAllBytes(generatedReportFilePath, file);
                    return generatedReportFilePath;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return exp.Message;
            }
        }

        public static string SaveCompanyInvoicePdfFile(string referenceId, byte[] file, string generatedReportFileName, string generatedReportDirPath,  bool isPdfServer, string domain, string serverIP, string username, string password, out string exception)
        {
            try
            {
                FileNetworkShare fileShare = new FileNetworkShare();

                exception = string.Empty;
                string generatedReportFilePath = Path.Combine(generatedReportDirPath, generatedReportFileName);
                if (isPdfServer)
                {
                    if (fileShare.UploadFileToShare(domain, username, password, generatedReportDirPath, generatedReportFilePath, file, serverIP, out exception))
                    {
                        return generatedReportFilePath;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (!Directory.Exists(generatedReportDirPath))
                        Directory.CreateDirectory(generatedReportDirPath);

                    File.WriteAllBytes(generatedReportFilePath, file);
                    return generatedReportFilePath;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                exception = exp.ToString();
                return string.Empty;
            }
        }
        public static void SendRequestToMotorQueue()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string response1 = client.DownloadString("http://10.201.204.42:8000");
                    string response2 = client.DownloadString("http://10.201.204.158:8000");
                }
            }
            catch
            {

            }
        }
        public static string ConvertOldPdfPathToNewPath(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && filePath.Split('\\').Count() == 7)
                {
                    string[] results = filePath.Split('\\');
                    string day = results[4].Split('-')[0];
                    string month = results[4].Split('-')[1];
                    string year = results[4].Split('-')[2];

                    return results[0] + "\\" + results[1] + "\\" + results[2] + "\\" + results[3] + "\\" + year + "\\" + month + "\\" + day + "\\" + results[6];
                }
                return filePath;
            }
            catch
            {
                return filePath;
            }
        }
        public static bool CheckImage(Stream imageStream,out string exception)
        {
            exception = string.Empty;
            try
            {
                using (var image = Image.FromStream(imageStream))
                {
                    if (image.Height < 2 && image.Width < 2)
                        return false;
                }
                using (var bitmap = new System.Drawing.Bitmap(imageStream))
                {
                }
                return true;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public static bool IsValidPdf(byte[] file, out string exception)
        {
            exception = string.Empty;
            try
            {
                new iTextSharp.text.pdf.PdfReader(file);
                return true;
            }
            catch (iTextSharp.text.exceptions.InvalidPdfException exp)
            {
                exception = exp.ToString();
                return false;
            }
        }
        public static bool IsValidImage(IFormFile postedFile, out string exception)
        {
            exception = string.Empty;
            if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            try
            {
                //if (!postedFile.InputStream.CanRead)
                //{
                //    return false;
                //}
                //By Atheer
                if (postedFile.Length < 256)
                {
                    return false;
                }

                byte[] buffer = new byte[256];
                //postedFile.InputStream.Read(buffer, 0, 256);   //By Atheer
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return false;
            }
            finally
            {
                //postedFile.InputStream.Position = 0;        //By Atheer
            }
            return true;
        }

        public static string ValidateInternalPhoneNumber(string phoneNumber)
        {
            phoneNumber = FormatePhoneBeforeValidateInternalPhoneNumber(phoneNumber);
            while (!phoneNumber.StartsWith("5"))
                phoneNumber = FormatePhoneBeforeValidateInternalPhoneNumber(phoneNumber);

            return $"{Zero}{phoneNumber}";
        }

        public static string FormatePhoneBeforeValidateInternalPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith(InternationalPhoneSymbol))
                phoneNumber = phoneNumber.Substring(InternationalPhoneSymbol.Length);
            if (phoneNumber.StartsWith(InternationalPhoneCode))
                phoneNumber = phoneNumber.Substring(InternationalPhoneCode.Length);
            if (phoneNumber.StartsWith(SaudiInternationalPhoneCode))
                phoneNumber = phoneNumber.Substring(SaudiInternationalPhoneCode.Length);
            if (phoneNumber.StartsWith(Zero))
                phoneNumber = phoneNumber.Substring(Zero.Length);

            return phoneNumber;
        }

        public static DeviceInfo GetFullDeviceInfo()
        {
            var deviceInfo = new DeviceDetector(Utilities.GetUserAgent());
            deviceInfo.Parse();
            if (deviceInfo == null)
                return null;

            DeviceInfo deviceInfoModel = new DeviceInfo();
            deviceInfoModel.DeviceType = $"{deviceInfo.GetDeviceName()}";
            deviceInfoModel.DeviceName = $"{deviceInfo.GetBrandName()}-{deviceInfo.GetModel()}";
            if (deviceInfo.GetOs().Success)
                deviceInfoModel.OS = $"{deviceInfo.GetOs().Match.Name}-{deviceInfo.GetOs().Match.Platform}-{deviceInfo.GetOs().Match.Version}";

            if (deviceInfo.GetBrowserClient().Success)
                deviceInfoModel.Client = $"{deviceInfo.GetBrowserClient().Match.Name}-{deviceInfo.GetBrowserClient().Match.Version}";

            return deviceInfoModel;
        }

        public static string GetPurePhoneNumber(string phoneNumber)
        {
            phoneNumber = FormatePhoneBeforeValidateInternalPhoneNumber(phoneNumber);
            while (!phoneNumber.StartsWith("5"))
                phoneNumber = FormatePhoneBeforeValidateInternalPhoneNumber(phoneNumber);

            return $"{phoneNumber}";
        }

        //public static string GetMachineUniqueUUID()
        //{
        //    try
        //    {
        //        //string cpuInfo = string.Empty;
        //        //ManagementClass mc = new ManagementClass("win32_processor");
        //        //ManagementObjectCollection moc = mc.GetInstances();
        //        //foreach (ManagementObject mo in moc)
        //        //{
        //        //    cpuInfo = mo.Properties["processorID"].Value.ToString();
        //        //    break;
        //        //}

        //        //string drive = "C";
        //        //ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
        //        //dsk.Get();
        //        //string volumeSerial = dsk["VolumeSerialNumber"].ToString();

        //        //return $"{cpuInfo}{volumeSerial}";

        //        ManagementObject os = new ManagementObject("Win32_OperatingSystem=@");
        //        //var serialNumber = os["SerialNumber"];
        //        return $"{Environment.MachineName}_{os["SerialNumber"]}";
        //    }
        //    catch (Exception exp)
        //    {
        //        return string.Empty;
        //    }
        //}
    }
}
