using System;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Tameenk.Security.CustomAttributes;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Profile.Component;

namespace Tameenk.Services.IdentityApi.Controllers
{
    [SingleSessionAuthorizeAttribute]
    public class PromotionController : IdentityBaseController
    {
       
        private readonly IPromotionContext _promotionContext;
        private readonly IAuthorizationService _authorizationService;

        #region The Ctro
        public PromotionController(IPromotionContext promotionContext,
            IAuthorizationService authorizationService)
        {
            _promotionContext = promotionContext;
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }
        #endregion

        //[TameenkAuthorizeAttribute]
        [SingleSessionAuthorizeAttribute]
        [HttpPost]
        [Route("api/promotion/joinbyemail")]
        public IHttpActionResult JoinProgramByEmail(JoinProgramModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            var output = _promotionContext.JoinProgramByEmail(model, User.Identity.GetUserName(), userId);
            if (output.ErrorCode == PromotionOutput.ErrorCodes.Success)
            {
                return Ok(output);
            }
            else
            {
                return Error(output);
            }
        }

        //[TameenkAuthorizeAttribute]
        [SingleSessionAuthorizeAttribute]
        [HttpPost]
        [Route("api/promotion/joinbynin")]
        public IHttpActionResult JoinProgramByNin(JoinProgramModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            var output = _promotionContext.JoinProgramByNin(model, User.Identity.GetUserName(), userId);
            if (output.ErrorCode == PromotionOutput.ErrorCodes.Success)
            {
                return Ok(output);
            }
            else
            {
                return Error(output);
            }
        }

        [HttpPost]
        [Route("api/promotion/confirmjoinprogram")]
        public IHttpActionResult ConfirmJoinProgram(ConfirmPromotionModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            var output = _promotionContext.ConfirmJoinProgram(model, User.Identity.GetUserName(), userId);
            if (output.ErrorCode == PromotionOutput.ErrorCodes.Success)
            {
                return Ok(output);
            }
            else
            {
                return Error(output);
            }
        }

        [HttpPost]
        [Route("api/promotion/promotion-program-checkEnrolled")]
        public IHttpActionResult GetEnrolledPromotionProgramBy(CheckPromotionProgramEnrolledModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            var output = _promotionContext.CheckUserEnrolled(model, User.Identity.GetUserName(), userId);
            if (output.ErrorCode == PromotionOutput.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }

        [HttpPost]
        [Route("api/promotion/joinbyEmailAndNin")]
        public IHttpActionResult JoinProgramByEmailAndNin(JoinProgramModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            var output = _promotionContext.JoinProgramByEmailAndNin(model, User.Identity.GetUserName(), userId);
            if (output.ErrorCode == PromotionOutput.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }

        [HttpPost]
        [Route("api/promotion/joinbyAttachment")]
        public IHttpActionResult JoinProgramByAttachment()
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();

            try
            {
                JoinProgramModel model = JsonConvert.DeserializeObject<JoinProgramModel>(HttpContext.Current.Request.Form[0]);
                HttpPostedFileBase file = null;
                if (HttpContext.Current.Request.Files.Count > 0)
                    file = new HttpPostedFileWrapper(HttpContext.Current.Request.Files["file"]);

                var output = _promotionContext.JoinProgramByAttachment(model, file, User.Identity.GetUserName(), userId);
                if (output.ErrorCode == PromotionOutput.ErrorCodes.Success)
                    return Ok(output);
                else
                    return Error(output);
            }
            catch (Exception ex)
            {
                return Error(ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/promotion/exit-promotion")]
        public IHttpActionResult ExitPromotionProgram(PromotionProgramApprovalActionModel model)
        {
            string userId = _authorizationService.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                userId = User.Identity.GetUserId();
            var output = _promotionContext.ExitPromotionProgram(model, User.Identity.GetUserName(), userId);
            if (output.ErrorCode == PromotionOutput.ErrorCodes.Success)
                return Ok(output);
            else
                return Error(output);
        }
    }
}




