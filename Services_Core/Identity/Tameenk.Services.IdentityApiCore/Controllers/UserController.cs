using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.WebResources;
using Tameenk.Security.Encryption;
using Tameenk.Security.Services;
using Tameenk.Services.Checkout.Components;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.IdentityApi.Extensions;
using Tameenk.Services.IdentityApi.Models;
using Tameenk.Services.IdentityApi.Output;
using Tameenk.Services.IdentityApi.Output.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.IdentityApi.Controllers
{
    //[Authorize]
    /// <summary>
    ///  user Control api
    /// </summary>
    public class UserController : IdentityBaseController
    {

        #region Fields
        private readonly IAuthorizationService _authorizationService;
        private readonly IPromotionService _promotionService;
        private ProfileRequestsLog profileLog;
        //private Output<UserModel> output;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IPolicyService _policyService;
        private const string Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY = "TameenkSendConfirmationEmailAfterPhoneVerificationCodeSharedKey@$";
        private readonly ICheckoutContext _checkoutContext;
        #endregion

        #region Ctor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="authorizationService">The authorization service.</param>
        /// <param name="promotionService"></param>
        public UserController(IAuthorizationService authorizationService, IPromotionService promotionService, TameenkConfig tameenkConfig, IPolicyService policyService, ICheckoutContext checkoutContext)
        {
         
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(IPromotionService));
            _tameenkConfig = tameenkConfig;
            _policyService = policyService;
            _checkoutContext = checkoutContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// get personal information to specific  user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/identity/user")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<UserModel>))]
        public async Task<IActionResult> GetUserPersonalInformation([FromBody]UserInputModel model)
        {
             Output<UserModel> output = new Output<UserModel>();
            output.Result = new UserModel();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID =new Guid(model.ID);
            profileLog.Channel = model.Channel.ToString();
            try
            {
                if (string.IsNullOrEmpty(model.ID))
                {
                    return OutputHandler<UserModel>(output, profileLog, Output<UserModel>.ErrorCodes.EmptyInputParamter, "UserIdNotExist", model.Language);
                }

                AspNetUser user = await _authorizationService.GetUser(model.ID);
                if (user == null)
                {
                    return OutputHandler<UserModel>(output, profileLog, Output<UserModel>.ErrorCodes.NotFound, "NotFound", model.Language);
                }
                UserModel res = user.ToModel();
                output.Result = res;
                output.ErrorCode = Output<UserModel>.ErrorCodes.Success;
                return Single(output);
            }
            catch (Exception ex)
            {
                return OutputHandler<UserModel>(output, profileLog, Output<UserModel>.ErrorCodes.ExceptionError, "Exception", model.Language,ex.Message);
            }

        }

        /// <summary>
        /// Add user to promotion program
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="email">Email</param>
        /// <param name="programId">Promotion program Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user/add-user-to-promotion-program")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<bool>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(CommonResponseModel<Api.Core.Models.ErrorModel>))]
        public IActionResult AddUserToPromotionProgram(UserPromotionProgramModel model)
        {
           
           Output<UserPromotionProgramModel> output = new Output<UserPromotionProgramModel>();
            output.Result = new UserPromotionProgramModel();
            profileLog = new ProfileRequestsLog();
            profileLog.UserID = new Guid(model.userId);
            profileLog.Channel = model.channel;
            try
            {
                if (string.IsNullOrEmpty(model.userId))
                {
                    return OutputHandler<UserPromotionProgramModel>(output, profileLog, Output<UserPromotionProgramModel>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", model.language);

                }
                if (string.IsNullOrEmpty(model.email))
                {
                    return OutputHandler<UserPromotionProgramModel>(output, profileLog, Output<UserPromotionProgramModel>.ErrorCodes.EmptyInputParamter, "EmptyInputParameter", model.language);
                }

                _promotionService.AddUserToPromotionProgram(model.userId,model. email, model.programId);
                output.ErrorCode = Output<UserPromotionProgramModel>.ErrorCodes.Success;
                return Single(output);
               
            }
            catch (Exception ex)
            {
                return OutputHandler<UserPromotionProgramModel>(output, profileLog, Output<UserPromotionProgramModel>.ErrorCodes.ExceptionError, "Exception", model.language, ex.Message);


            }
        }

        [HttpGet]
        [Route("api/identity/IsAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            Output<string> output=new Output<string>();
           
            var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type.ToString().ToUpper() == "curent_user_id".ToUpper())?.Value;
            if (!string.IsNullOrEmpty(UserId))
            {
                return Ok(output.ErrorCode = Output<string>.ErrorCodes.Success);
            }
            else
            {
                output.ErrorCode = Output<string>.ErrorCodes.unAuthorized;
                output.ErrorDescription = "Un Authorized";
                return Ok(output);
            }

            
        }

        [HttpGet]
        [Route("api/u/p")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Api.Core.Models.CommonResponseModel<bool>))]
        public IActionResult DownloadPolicy(string r)
        {
            IActionResult response;
            System.Net.Http.HttpResponseMessage responseMsg = new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK);
            

            if (!string.IsNullOrEmpty(r))
            {
                var policy = _policyService.GetPolicyByReferenceId(r);
                if (policy != null && policy.PolicyFileId.HasValue)
                {
                    var policyFile = _policyService.DownloadPolicyFile(policy.PolicyFileId.ToString());
                    if (policyFile != null && policyFile.PolicyFileByte != null)
                    {
                        responseMsg.Content = new System.Net.Http.ByteArrayContent(policyFile.PolicyFileByte);
                        responseMsg.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                        responseMsg.Content.Headers.ContentLength = policyFile.PolicyFileByte.Length;
                        response = ResponseMessage(responseMsg);
                        return response;
                    }
                    else if (!string.IsNullOrEmpty(policyFile.FilePath))
                    {
                        FileNetworkShare fileShare = new FileNetworkShare();
                        string exception = string.Empty;
                        if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                        {
                            var file = fileShare.GetFile(policyFile.FilePath, out exception);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(policyFile.FilePath, out exception);
                            if (file != null)
                            {
                                responseMsg.Content = new System.Net.Http.ByteArrayContent(file);
                                responseMsg.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                                responseMsg.Content.Headers.ContentLength = file.Length;
                                response = ResponseMessage(responseMsg);
                                return response;
                            }
                        }
                        else
                        {
                            exception = string.Empty;
                            var file = System.IO.File.ReadAllBytes(policyFile.FilePath);
                            if (file == null)
                                file = fileShare.GetFileFromNewServer(policyFile.FilePath, out exception);
                            //if (file == null)
                            //{
                            //    string newPath = Utilities.ConvertOldPdfPathToNewPath(policyFile.FilePath);
                            //    file = System.IO.File.ReadAllBytes(newPath);
                            //}

                            if (file != null)
                            {
                                responseMsg.Content = new System.Net.Http.ByteArrayContent(file);
                                responseMsg.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                                responseMsg.Content.Headers.ContentLength = file.Length;
                                response = ResponseMessage(responseMsg);
                                return response;
                            }
                            else
                                return Error("No file exists");


                            //responseMsg.Content = new System.Net.Http.ByteArrayContent(System.IO.File.ReadAllBytes(policyFile.FilePath));
                            //responseMsg.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                            //responseMsg.Content.Headers.ContentLength = System.IO.File.ReadAllBytes(policyFile.FilePath).Length;
                            //response = ResponseMessage(responseMsg);
                            //return response;
                        }
                    }
                }
            }
            response = ResponseMessage(responseMsg);
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/checkout/ConfirmEmailToReceivePolicy")]
        public IActionResult ConfirmEmailToReceivePolicy(string token)
        {
            Output<bool> output = new Output<bool>();
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = WebResources.ErrorGeneric;
                    return Ok(output);
                }

                var decryptedToken = AESEncryption.DecryptString(token, Send_Confirmation_Email_After_Phone_Verification_Code_SHARED_KEY);
                var model = JsonConvert.DeserializeObject<ActivationEmailToReceivePolicyModel>(decryptedToken);
                if (model == null)
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.ModelBinderError;
                    output.ErrorDescription = WebResources.ErrorGeneric;
                    return Ok(output);
                }

                string exception = string.Empty;
                var outputModel = _checkoutContext.ActivateUserEmailToReceivePolicy(model.ReferenceId, model.Email, model.UserId, Channel.Portal.ToString(), out exception);
                if ((int)outputModel.ErrorCode != (int)Output<bool>.ErrorCodes.Success || !string.IsNullOrEmpty(exception))
                {
                    output.ErrorCode = Output<bool>.ErrorCodes.ServiceDown;
                    output.ErrorDescription = WebResources.ErrorGeneric;
                    return Ok(output);
                }

                output.ErrorCode = Output<bool>.ErrorCodes.Success;
                output.ErrorDescription = WebResources.EmailActivated;
                return Ok(output);
            }
            catch (Exception)
            {
                output.ErrorCode = Output<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = WebResources.ErrorGeneric;
                return Ok(output);
            }
        }
        #endregion
    }
}
