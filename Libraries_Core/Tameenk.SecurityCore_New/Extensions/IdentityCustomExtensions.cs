using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Core.Settings;

namespace Tameenk.Security.Extensions
{
    public static class IdentityCustomExtensions
    {

        /// <summary>
       /// Check if user allowed to have new policies according to it's current policies policiescount if not zero or according to settings that admin set.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="policyService"></param>
        /// <param name="authorizationService"></param>
        /// <param name="settingService"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<bool> CheckUserAllowedGettingPoliciesAsync(string userId, IPolicyService policyService, IAuthorizationService authorizationService, ISettingService settingService )
        {
            bool allowed = false;
            AspNetUser aspNetUser= await authorizationService.GetUser(userId);
            if (aspNetUser != null)
            {
                int currentPolicies = policyService.GetUserPoliciesCount(aspNetUser.Id);
                if (aspNetUser.PoliciesCount == 0)  //if count =0 get max allowed policies from settings
                {
                     Setting setting = settingService.GetSetting();
                    if (setting.MaxNumberOfPolicies > currentPolicies) allowed = true;
                }
                else
                {
                    if (aspNetUser.PoliciesCount > currentPolicies) allowed = true;
                }
            }
            return allowed;
        }

        /// <summary>
        /// Check if user allowed to have new promtioncode according to it's current promotion code if not zero or according to settings that admin set.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="policyService"></param>
        /// <param name="authorizationService"></param>
        /// <param name="settingService"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<bool> CheckUserAllowedGettingPromotionCodeAsync(string userId, IPromotionService promotionService, IAuthorizationService authorizationService, ISettingService settingService)
        {
            bool allowed = false;           
            AspNetUser aspNetUser = await authorizationService.GetUser(userId);
            if (aspNetUser != null)
            {
                int currentPromotionCode = promotionService.GetUserPromotionCodeCount(aspNetUser.Id);
                if (aspNetUser.PromotionCodeCount == 0)  //if count =0 get max allowed promotioncode from settings
                {
                    Setting setting = settingService.GetSetting();
                    if (setting.MaxNumberOfPolicies > currentPromotionCode) allowed = true;
                }
                else
                {
                    if (aspNetUser.PromotionCodeCount > currentPromotionCode) allowed = true;
                }
            }

            return allowed;
        }
    }
}