using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Reflection;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Security.Encryption;
using Tameenk.Services.IdentityApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.IdentityApi.Controllers
{
    public class CaptchaController : BaseApiController
    {
        private const string SHARED_SECRET = "xYD_3h95?D&*&rTL";
        /// <summary>
        /// Generate new captcha image.
        /// </summary>
        /// <returns></returns>
        [Route("api/identity/captcha")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<CaptchaModel>))]
        [AllowAnonymous]
        public IActionResult Get()        {
            int num = new Random().Next(1000, 9999);            string str = GenerateBase64Captcha(num.ToString());            string img = $"data:image/jpeg;base64,{str}";            var captchaToken = new CaptchaToken()            {                Captcha = num.ToString(),                ExpiryDate = DateTime.Now.AddSeconds(605)            };            string token = AESEncryption.EncryptString(JsonConvert.SerializeObject(captchaToken), SHARED_SECRET);            return Ok(new CaptchaModel { Image = img, Token = token, ExpiredInSeconds = 600 });        }        public string GenerateBase64Captcha(string captchaValue)        {            int height = 50;            int width = 100;            Random random = new Random();            HatchStyle[] hatchStyleArray = new HatchStyle[40]            {        HatchStyle.BackwardDiagonal,        HatchStyle.Cross,        HatchStyle.DashedDownwardDiagonal,        HatchStyle.DashedHorizontal,        HatchStyle.DashedUpwardDiagonal,        HatchStyle.DashedVertical,        HatchStyle.DiagonalBrick,        HatchStyle.DiagonalCross,        HatchStyle.Divot,        HatchStyle.DottedDiamond,        HatchStyle.DottedGrid,        HatchStyle.ForwardDiagonal,        HatchStyle.Horizontal,        HatchStyle.HorizontalBrick,        HatchStyle.LargeCheckerBoard,        HatchStyle.LargeConfetti,        HatchStyle.Cross,        HatchStyle.LightDownwardDiagonal,        HatchStyle.LightHorizontal,        HatchStyle.LightUpwardDiagonal,        HatchStyle.LightVertical,        HatchStyle.Cross,        HatchStyle.Horizontal,        HatchStyle.NarrowHorizontal,        HatchStyle.NarrowVertical,        HatchStyle.OutlinedDiamond,        HatchStyle.Plaid,        HatchStyle.Shingle,        HatchStyle.SmallCheckerBoard,        HatchStyle.SmallConfetti,        HatchStyle.SmallGrid,        HatchStyle.SolidDiamond,        HatchStyle.Sphere,        HatchStyle.Trellis,        HatchStyle.Vertical,        HatchStyle.Wave,        HatchStyle.Weave,        HatchStyle.WideDownwardDiagonal,        HatchStyle.WideUpwardDiagonal,        HatchStyle.ZigZag            };            MemoryStream memoryStream = new MemoryStream();            string str = captchaValue;            Bitmap bitmap = new Bitmap(width, height);            Graphics graphics = Graphics.FromImage((Image)bitmap);            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;            graphics.SmoothingMode = SmoothingMode.AntiAlias;            RectangleF rect = new RectangleF(0, 0, width, height);            Brush brush = (Brush)new HatchBrush(hatchStyleArray[random.Next(checked(hatchStyleArray.Length - 1))], Color.FromArgb(random.Next(100, (int)byte.MaxValue), random.Next(100, (int)byte.MaxValue), random.Next(100, (int)byte.MaxValue)), Color.White);            graphics.FillRectangle(brush, rect);            for (int i = 0; i < str.Length; i++)            {                int charWidth = bitmap.Width / str.Length;                graphics.DrawString(str.Substring(i, 1),                    new Font("Tahoma", GetRandomNumber(30, 35), GetRandomFontStyle(), GraphicsUnit.Pixel),                    GetRandomBrush(),                    GetRandomNumber(-3, 3) + (charWidth * i),                    GetRandomNumber(2, 10));

                //graphics.ResetTransform();
            }            bitmap.Save((Stream)memoryStream, ImageFormat.Png);            return Convert.ToBase64String(memoryStream.ToArray());        }

        /// <summary>
        /// Validate the captcha
        /// </summary>
        /// <param name="token">The token sent with captcha image.</param>
        /// <param name="input">The user input of message shown in captcha image.</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <returns></returns>
        /// <returns></returns>
        [Route("api/identity/captcha/validate")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<bool>))]
        [AllowAnonymous]
        public IActionResult ValidateCaptcha([FromBody]ValidateCaptchaModel model)
        {
            var encryptedCaptcha = AESEncryption.DecryptString(model.Token, SHARED_SECRET);
            try
            {
                var captchaToken = JsonConvert.DeserializeObject<CaptchaToken>(encryptedCaptcha);
                if (captchaToken.ExpiryDate.CompareTo(DateTime.Now.AddSeconds(-600)) < 0)
                {
                    return Error("Expired Captcha");
                }
                if (captchaToken.Captcha.Equals(model.Input, StringComparison.Ordinal))
                {
                    return Ok(true);
                }
            }
            catch (Exception)
            {
                return Error("Invalid Captcha");
            }

            return Error("Invalid Captcha");
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

        //Function to get random number
        private static Random random;

        private static int GetRandomNumber(int min, int max)
        {
            random = random ?? new Random((int)DateTime.Now.Ticks);
            lock (random) // synchronize
            {
                return random.Next(min, max);
            }
        }


    }

    /// <summary>
    /// Captcha token class
    /// </summary>
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
}