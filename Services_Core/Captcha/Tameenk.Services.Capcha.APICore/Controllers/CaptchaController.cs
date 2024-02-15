using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.Capcha.API
{
    public class CapchaController : BaseApiController
    {
        private const string SHARED_SECRET = "xYD_3h95?D&*&rTL";
        private static byte[] _salt = Encoding.UTF8.GetBytes("tameenk-sec-key-pass-word#$&");

        [HttpGet]
        [Route("api/GetCaptcha")]
        [AllowAnonymous]
        public IActionResult GetCapcha()
        {
            int num = new Random().Next(1000, 9999);
            string str = GenerateBase64Captcha(num.ToString());

            string img = $"data:image/jpeg;base64,{str}";
            var captchaToken = new CaptchaToken()
            {
                Captcha = num.ToString(),
                ExpiryDate = DateTime.Now.AddSeconds(605)
            };
            string token = EncryptString(JsonConvert.SerializeObject(captchaToken), SHARED_SECRET);

            return Json(new Tokenresponse { Data = new CaptchaModel { Image = img, Token = token, ExpiredInSeconds = 600 }, Errors = null, TotalCount = 0 });
        }

        [HttpPost]
        [Route("api/ValidateCaptcha")]
        [AllowAnonymous]
        public IActionResult ValidateCaptcha([FromBody] ValidateCaptchaModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Missing Model Data");
                if (string.IsNullOrEmpty(model.Token))
                    return BadRequest("Missing Token Data");

                var encryptedCaptcha = DecryptString(model.Token, SHARED_SECRET);
                var captchaToken = JsonConvert.DeserializeObject<CaptchaToken>(encryptedCaptcha);
                if (captchaToken.ExpiryDate.CompareTo(DateTime.Now.AddSeconds(-600)) < 0)
                    return BadRequest("Expired Captcha");

                var isValid = captchaToken.Captcha.Equals(model.Input, StringComparison.Ordinal);
                if (!isValid)
                    return BadRequest("Invalid Captcha");

                return Json(new ValidateCaptchaResponse { Data = true, Errors = null, TotalCount = 0 });
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\CaptchaValidationException_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
                return BadRequest("Captcha Exception");
            }
        }

        public string GenerateBase64Captcha(string captchaValue)
        {
            int height = 50;
            int width = 100;
            Random random = new Random();
            HatchStyle[] hatchStyleArray = new HatchStyle[40]
            {
        HatchStyle.BackwardDiagonal,
        HatchStyle.Cross,
        HatchStyle.DashedDownwardDiagonal,
        HatchStyle.DashedHorizontal,
        HatchStyle.DashedUpwardDiagonal,
        HatchStyle.DashedVertical,
        HatchStyle.DiagonalBrick,
        HatchStyle.DiagonalCross,
        HatchStyle.Divot,
        HatchStyle.DottedDiamond,
        HatchStyle.DottedGrid,
        HatchStyle.ForwardDiagonal,
        HatchStyle.Horizontal,
        HatchStyle.HorizontalBrick,
        HatchStyle.LargeCheckerBoard,
        HatchStyle.LargeConfetti,
        HatchStyle.Cross,
        HatchStyle.LightDownwardDiagonal,
        HatchStyle.LightHorizontal,
        HatchStyle.LightUpwardDiagonal,
        HatchStyle.LightVertical,
        HatchStyle.Cross,
        HatchStyle.Horizontal,
        HatchStyle.NarrowHorizontal,
        HatchStyle.NarrowVertical,
        HatchStyle.OutlinedDiamond,
        HatchStyle.Plaid,
        HatchStyle.Shingle,
        HatchStyle.SmallCheckerBoard,
        HatchStyle.SmallConfetti,
        HatchStyle.SmallGrid,
        HatchStyle.SolidDiamond,
        HatchStyle.Sphere,
        HatchStyle.Trellis,
        HatchStyle.Vertical,
        HatchStyle.Wave,
        HatchStyle.Weave,
        HatchStyle.WideDownwardDiagonal,
        HatchStyle.WideUpwardDiagonal,
        HatchStyle.ZigZag
            };
            MemoryStream memoryStream = new MemoryStream();

            string str = captchaValue;
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage((Image)bitmap);
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF rect = new RectangleF(0, 0, width, height);
            Brush brush = (Brush)new HatchBrush(hatchStyleArray[random.Next(checked(hatchStyleArray.Length - 1))], Color.FromArgb(random.Next(100, (int)byte.MaxValue), random.Next(100, (int)byte.MaxValue), random.Next(100, (int)byte.MaxValue)), Color.White);
            graphics.FillRectangle(brush, rect);
            for (int i = 0; i < str.Length; i++)
            {
                int charWidth = bitmap.Width / str.Length;
                graphics.DrawString(str.Substring(i, 1),
                    new Font("Tahoma", GetRandomNumber(30, 35), GetRandomFontStyle(), GraphicsUnit.Pixel),
                    GetRandomBrush(),
                    GetRandomNumber(-3, 3) + (charWidth * i),
                    GetRandomNumber(2, 10));

                //graphics.ResetTransform();
            }

            bitmap.Save((Stream)memoryStream, ImageFormat.Png);
            return Convert.ToBase64String(memoryStream.ToArray());
        }
        private Brush GetRandomBrush()
        {

            Dictionary<int, Brush> brushes = new Dictionary<int, Brush>();
            brushes.Add(1, Brushes.Black);
            brushes.Add(2, Brushes.Blue);
            brushes.Add(3, Brushes.Gray);
            brushes.Add(4, Brushes.Brown);
            brushes.Add(5, Brushes.Chocolate);
            brushes.Add(6, Brushes.Indigo);
            brushes.Add(7, Brushes.BlueViolet);
            return brushes[GetRandomNumber(1, 7)];
        }
        private FontStyle GetRandomFontStyle()
        {
            Dictionary<int, FontStyle> fontStyles = new Dictionary<int, FontStyle>();
            fontStyles.Add(0, FontStyle.Bold);
            fontStyles.Add(1, FontStyle.Italic);
            fontStyles.Add(2, FontStyle.Regular);
            fontStyles.Add(3, FontStyle.Underline);
            fontStyles.Add(4, FontStyle.Bold | FontStyle.Italic);
            fontStyles.Add(5, FontStyle.Italic | FontStyle.Underline);
            fontStyles.Add(6, FontStyle.Italic | FontStyle.Underline | FontStyle.Bold);
            fontStyles.Add(7, FontStyle.Bold | FontStyle.Regular);
            fontStyles.Add(8, FontStyle.Underline | FontStyle.Bold);
            return fontStyles[GetRandomNumber(1, 8)];
        }
        private static Random random;

        private static int GetRandomNumber(int min, int max)
        {
            random = random ?? new Random((int)DateTime.Now.Ticks);
            lock (random) // synchronize
            {
                return random.Next(min, max);
            }
        }
        [JsonObject]
        public class CaptchaToken
        {
            /// <summary>
            /// Captcha value.
            /// </summary>
            [JsonProperty("captcha")]
            public string Captcha { get; set; }

            /// <summary>
            /// Captcha expiration date.
            /// </summary>
            [JsonProperty("expiryDate")]
            public DateTime ExpiryDate { get; set; }
        }
        //[JsonObject("captcha")]
        //public class CaptchaModel
        //{
        //    /// <summary>
        //    /// The captcha image.
        //    /// </summary>
        //    [JsonProperty("image")]
        //    public string Image { get; set; }

        //    /// <summary>
        //    /// Captcha token.
        //    /// </summary>
        //    [JsonProperty("token")]
        //    public string Token { get; set; }

        //    /// <summary>
        //    /// Captcah expiration in seconds.
        //    /// </summary>
        //    [JsonProperty("expiredInSeconds")]
        //    public int ExpiredInSeconds { get; set; }
        //}

        public static string EncryptString(string plainText, string sharedSecret)
        {


            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }

            catch (Exception ex)
            {

            }

            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        public static string DecryptString(string cipherText, string sharedSecret)
        {
            RijndaelManaged aesAlg = null;
            string plaintext = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {

                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}