using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Common.Utilities
{
   public class SecurityUtilities
    {


        public static string HashKey
        {
            get
            {
                return "bC@Re_2020_N0MeTecH_hAsHKEy";
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

        private const int SaltValueSize = 16;
        /// <summary>
        /// 
        /// </summary>
        private static readonly string[] HashAlgorithms = new string[] { "SHA256", "SHA1", "MD5", "SHA512" };

        /// <summary>
        /// Verifies the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static bool VerifyHashedData(string hashedText, string plainText)
        {
            try
            {
                string salt = hashedText.Substring(0, SaltValueSize * 2);
                //foreach (string hashAlgorithm in HashAlgorithms)
                {
                    string computedHash = HashData(plainText, salt);
                    if (string.Equals(computedHash, hashedText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
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
        /// Hashes the data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="hashAlgorithm">The hash algorithm.</param>
        /// <returns></returns>
        public static string HashData(string plainText, string salt)
        {
            //if (hashAlgorithm == null)
            string hashAlgorithm = HashAlgorithms[0];
            Encoding encoding = Encoding.Unicode;
            //string salt = null;
            if (salt == null)
            {
                salt = Guid.NewGuid().ToString("N").Substring(0, SaltValueSize * 2);
            }
            int saltSize = string.IsNullOrEmpty(salt) ? 0 : salt.Length / 2;
            byte[] valueToHash = new byte[saltSize + encoding.GetByteCount(plainText)];
            for (int i = 0; i < saltSize; i++)
            {
                valueToHash[i] = byte.Parse(salt.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
            }
            encoding.GetBytes(plainText, 0, plainText.Length, valueToHash, saltSize);

            using (HashAlgorithm hash = HashAlgorithm.Create(hashAlgorithm))
            {
                byte[] hashValue = hash.ComputeHash(valueToHash);
                StringBuilder hashedText = new StringBuilder((hashValue.Length + saltSize) * 2);
                if (!string.IsNullOrEmpty(salt))
                    hashedText.Append(salt);

                foreach (byte hexdigit in hashValue)
                {
                    hashedText.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "{0:X2}", hexdigit);
                }
                return hashedText.ToString();
            }
        }

        /// <summary>
        /// Hashes the data by M d5.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        public static byte[] HashDataByMD5(string plainText)
        {
            using (System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(plainText);
                bs = x.ComputeHash(bs);

                return bs;
            }
        }

        /// <summary>
        /// Encrypts the string to bytes AES.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        public static string EncryptStringToBytesAES(string plainText, string secureKey, string secureIV)
        {
            try
            {
                byte[] Key = Convert.FromBase64String(secureKey);
                byte[] IV = Convert.FromBase64String(secureIV);
                byte[] encrypted;
                // Create an AesManaged object 
                // with the specified key and IV. 
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption. 
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {

                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }


                // Return the encrypted bytes from the memory stream. 
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }

        }

        /// <summary>
        /// Decrypts the string from bytes_ aes.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        public static string DecryptStringFromBytesAES(byte[] cipherText, string secureKey, string secureIV)
        {
            try
            {
                byte[] Key = Convert.FromBase64String(secureKey);
                byte[] IV = Convert.FromBase64String(secureIV);

                // Declare the string used to hold 
                // the decrypted text. 
                string plaintext = null;

                // Create an AesManaged object 
                // with the specified key and IV. 
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption. 
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream 
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return string.Empty;
            }

        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Formats the credit card number.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string FormatCreditCardNumber(string cardNumber, string key, string IV)
        {
            string formattedNumber = string.Empty;
            if (!string.IsNullOrEmpty(cardNumber))
                formattedNumber = cardNumber.Substring(0, 6) + "XXXXXXX" + cardNumber.Substring(cardNumber.Length - 4);
            return EncryptStringToBytesAES(formattedNumber, key, IV);
        }

        /// <summary>
        /// Formats the credit card number.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <returns></returns>
        public static string FormatCreditCardNumber(string cardNumber)
        {
            string formattedNumber = string.Empty;
            if (!string.IsNullOrEmpty(cardNumber))
                formattedNumber = "XXXXXXXX" + cardNumber.Substring(cardNumber.Length - 4);
            return formattedNumber;
        }

        /// <summary>
        /// Creates the SH a256 signature. 
        /// </summary>
        /// <param name="useRequest">if set to <c>true</c> [use request].</param>
        /// <returns></returns>
        public static string CreateSHA256Signature(bool useRequest, SortedList<String, String> _requestFields, SortedList<String, String> _responseFields, string secureHashKey)
        {

            string _secureSecret = secureHashKey;

            // Hex Decode the Secure Secret for use in using the HMACSHA256 hasher
            // hex decoding eliminates this source of error as it is independent of the character encoding
            // hex decoding is precise in converting to a byte array and is the preferred form for representing binary values as hex strings. 
            byte[] convertedHash = new byte[_secureSecret.Length / 2];
            for (int i = 0; i < _secureSecret.Length / 2; i++)
            {
                convertedHash[i] = (byte)Int32.Parse(_secureSecret.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            // Build string from collection in preperation to be hashed
            StringBuilder sb = new StringBuilder();
            SortedList<String, String> list = (useRequest ? _requestFields : _responseFields);
            foreach (KeyValuePair<string, string> kvp in list)
            {
                if (kvp.Key.StartsWith("vpc_") || kvp.Key.StartsWith("user_"))
                    sb.Append(kvp.Key + "=" + kvp.Value + "&");
            }
            // remove trailing & from string
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            // Create secureHash on string
            string hexHash = "";
            using (HMACSHA256 hasher = new HMACSHA256(convertedHash))
            {
                //byte[] hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

                byte[] utf8bytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);
                byte[] hashValue = hasher.ComputeHash(iso8859bytes);



                foreach (byte b in hashValue)
                {
                    hexHash += b.ToString("X2");
                }
            }
            return hexHash;
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

        /// <summary>
        /// Encrypts the string to bytes_ AES.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        public static byte[] encryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {

            // Declare the streams used
            // to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                swEncrypt = new StreamWriter(csEncrypt);

                //Write all data to the stream.
                swEncrypt.Write(plainText);

            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (swEncrypt != null)
                    swEncrypt.Close();
                if (csEncrypt != null)
                    csEncrypt.Close();
                if (msEncrypt != null)
                    msEncrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();

        }

        /// <summary>
        /// Decrypts the string from bytes_ AES.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        public static string decryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // TDeclare the streams used
            // to decrypt to an in memory
            // array of bytes.
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                msDecrypt = new MemoryStream(cipherText);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (srDecrypt != null)
                    srDecrypt.Close();
                if (csDecrypt != null)
                    csDecrypt.Close();
                if (msDecrypt != null)
                    msDecrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;

        }

        

        /// <summary>
        /// Encrypts the string to bytes_ AES.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        public static byte[] EncryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {

            // Declare the streams used
            // to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                swEncrypt = new StreamWriter(csEncrypt);

                //Write all data to the stream.
                swEncrypt.Write(plainText);

            }
            catch (Exception exp)
            {
                //ErrorLogger.LogError(exp.Message, exp, false);
            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (swEncrypt != null)
                    swEncrypt.Close();
                if (csEncrypt != null)
                    csEncrypt.Close();
                if (msEncrypt != null)
                    msEncrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();

        }

        /// <summary>
        /// Decrypts the string from bytes_ AES.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        public static string DecryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // TDeclare the streams used
            // to decrypt to an in memory
            // array of bytes.
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                msDecrypt = new MemoryStream(cipherText);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            catch (Exception exp)
            {
                //ErrorLogger.LogError(exp.Message, exp, false);
            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (srDecrypt != null)
                    srDecrypt.Close();
                if (csDecrypt != null)
                    csDecrypt.Close();
                if (msDecrypt != null)
                    msDecrypt.Close();

                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;

        }


       

        /// <summary>
        /// Converts the IP address to intger.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        public static long ConvertIPAddressToIntger(string ip)
        {
            try
            {
                string[] ipValue = ip.Split('.');
                int firstOctet = 0;
                int secondOctet = 0;
                int thirdOctet = 0;
                int fourthOctet = 0;
                int.TryParse(ipValue[0], out firstOctet);
                int.TryParse(ipValue[1], out secondOctet);
                int.TryParse(ipValue[2], out thirdOctet);
                int.TryParse(ipValue[3], out fourthOctet);
                long IpNumber = (firstOctet * 16777216) + (secondOctet * 65536) + (thirdOctet * 256) + (fourthOctet);

                return IpNumber;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return -1;
            }
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="encryptedStr">The encrypted STR.</param>
        /// <returns></returns>
        public static string DecryptStringAES(string encryptedStr, string key, string Iv)
        {
            string decryptedString = string.Empty;

            byte[] bytes = DecodeHex16(encryptedStr);

            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.Key = Encoding.ASCII.GetBytes(key);
                myRijndael.IV = Encoding.ASCII.GetBytes(Iv);
                decryptedString = DecryptStringFromBytes_AES(bytes, myRijndael.Key, myRijndael.IV);
            }

            return decryptedString;
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string EncryptStringAES(string str, string key, string Iv)
        {
            string encryptedString = string.Empty;

            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.Key = Encoding.ASCII.GetBytes(key);
                myRijndael.IV = Encoding.ASCII.GetBytes(Iv);
                byte[] encrypted = EncryptStringToBytes_AES(str, myRijndael.Key, myRijndael.IV);
                encryptedString = EncodeHex16(encrypted);
            }

            return encryptedString;
        }

        public static string HyperpayDecryption(string httpBody, string ivFromHttpHeader, string authTagFromHttpHeader, out string exceptiopn)
        {
            exceptiopn = string.Empty;
            try
            {
                string keyFromConfiguration = Utilities.GetAppSetting("HyperpaykeyFromConfiguration");// "26EE3FDF5B1BEEE23A150BCA85D1898DFC34748824CF62B096481C777851F164";

                byte[] key = ToByteArray(keyFromConfiguration);
                byte[] iv = ToByteArray(ivFromHttpHeader);
                byte[] authTag = ToByteArray(authTagFromHttpHeader);
                byte[] encryptedText = ToByteArray(httpBody);
                byte[] cipherText = encryptedText.Concat(authTag).ToArray();

                // Prepare decryption
                GcmBlockCipher cipher = new GcmBlockCipher(new AesFastEngine());
                AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv);
                cipher.Init(false, parameters);

                // Decrypt
                var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
                var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
                cipher.DoFinal(plainText, len);
                return Encoding.ASCII.GetString(plainText).ToString();
            }
            catch (Exception ex)
            {
                exceptiopn = ex.ToString();
                return ex.ToString();
            }
        }
        public static byte[] ToByteArray(string HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

    }
}
