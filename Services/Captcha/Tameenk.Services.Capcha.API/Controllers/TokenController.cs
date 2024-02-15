using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tameenk.Redis;
using Tameenk.Services.Capcha.API;
namespace Tameenk.Services.CaptchaApi.Controllers
{
    public class TokenController : ApiController
    {
        //private readonly IRedisCacheManager _redisCacheManager;

        private const string _Url = "http://localhost:3000/token"; // local idenetity to not calling (https://bcare.com.sa/IdentityApi) to enhance performance
        private const string _ClientId = "684C02DE-C782-4C7A-9999-70E687D73CD6";
        private const string _ClientSecret = "8776677C-CBC9-4CB9-8AD8-A135D49B1C54-8564116E-BE41-4409-B46D-5B35C74F94B9-0FB081CC-2927-4A7C-A9B7-0CF76571107F";

        //private const string base_KEY = "ToKeN_cAcH";
        //private const int cach_TiMe = 1799;

        //public TokenController(IRedisCacheManager redisCacheManager)
        //{
        //    _redisCacheManager = redisCacheManager;
        //}

        #region API

        [HttpPost]
        [Route("api/GetAccessToken")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetToken([FromBody] UserData model)
        {
            if (model == null)
                return BadRequest("Missing Model Data");
            if (string.IsNullOrEmpty(model.UserId))
                return BadRequest("Missing UserId Data");

            var Result = await GetAccessToken(model);
            if (Result == null)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\GetAccessToken_exception_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + Result.ErrorMsg);
                return BadRequest("Error happend while processing your request");
            }

            if (Result == null || Result.ErrorCode != Output<AccessTokenResult>.ErrorCodes.Success)
                return BadRequest(Result.ErrorMsg);

            return Json(Result);
        }

        #endregion

        #region Helpers
        private async Task<Output<AccessTokenResult>> GetAccessToken(UserData model)
        {
            Output<AccessTokenResult> output = new Output<AccessTokenResult>() { ErrorCode = Output<AccessTokenResult>.ErrorCodes.Success };
            output.Result = new AccessTokenResult();
            
            try
            {
                //var token = await _redisCacheManager.GetAsync<AccessTokenResult>($"{base_KEY}_{model.UserId}");
                //if (token != null)
                //{
                //    if (token != null && token.TokenExpirationDate.AddSeconds(1799) > DateTime.Now)
                //    {
                //        output.Result = token;
                //        return output;
                //    }
                //}

                var formParamters = new Dictionary<string, string>();
                formParamters.Add("grant_type", "client_credentials");
                formParamters.Add("client_Id", _ClientId);
                formParamters.Add("client_secret", _ClientSecret);
                formParamters.Add("curent_user_id", model.UserId);

                var content = new FormUrlEncodedContent(formParamters);
                HttpClient httpClient = new HttpClient();
                var postTask = httpClient.PostAsync($"{_Url}", content);
                await postTask.ConfigureAwait(false);
                var response = postTask.Result;
                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonString = response.Content.ReadAsStringAsync().Result;
                output.Result = JsonConvert.DeserializeObject<AccessTokenResult>(jsonString);
                output.Result.TokenExpirationDate = DateTime.Now.AddMinutes(30);

                //_redisCacheManager.SetAsync($"{base_KEY}_{model.UserId}", output.Result, cach_TiMe);
                output.ErrorCode = Output<AccessTokenResult>.ErrorCodes.Success;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = Output<AccessTokenResult>.ErrorCodes.Fail;
                output.ErrorMsg = ex.ToString();
                return output;
            }
        }

        #endregion

    }
}
