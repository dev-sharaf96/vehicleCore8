using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Tameenk.App_Start;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Localizations;
using TameenkDAL.Models;
using Microsoft.AspNet.Identity;
using Tameenk.Security.Services;

namespace Tameenk
{
    public class WebContext
    {
        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly ILocalizationService _localizationService;
        private readonly IAuthorizationService _authorizationService;
        private IPrincipal _currentUser;
        //private UserManager _userManager;
        private Tameenk.Services.Profile.Component.Membership.UserManager _userManager;
        private Language _language;
        // private ClientSignInManager _signInManager;
        private Tameenk.Services.Profile.Component.Membership.ClientSignInManager _signInManager;
        #endregion

        #region Ctor

        public WebContext(IAuthorizationService authorizationService, ILocalizationService localizationService, HttpContextBase httpContext)
        {
            _httpContext = httpContext;
            _localizationService = localizationService;
            _authorizationService = authorizationService;

        }

        #endregion

        #region Methods

        private void AuthenticateThisRequest()
        {
            //NOTE:  if the user is already logged in (e.g. under a different user account)
            //       then this will NOT reset the identity information.  Be aware of this if
            //       you allow already-logged in users to "re-login" as different accounts 
            //       without first logging out.
            if (_httpContext.User.Identity.IsAuthenticated) return;

            var name = FormsAuthentication.FormsCookieName;
            try
            {
                if (_httpContext.Response == null) return;
                var cookie = _httpContext.Response.Cookies[name];
                if (cookie != null)
                {
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    if (ticket != null && !ticket.Expired)
                    {
                        string[] roles = (ticket.UserData as string ?? "").Split(',');
                        _httpContext.User = new GenericPrincipal(new FormsIdentity(ticket), roles);
                    }
                }
            }
            catch {
                return;
            }
        }
        #endregion

        #region Properties

        public IPrincipal CurrentUser {
            get {
                _currentUser = _currentUser ?? _httpContext.User;
                if (_currentUser.Identity is WindowsIdentity && (_currentUser.Identity as WindowsIdentity).IsAnonymous)
                {
                    //var user = new ClientUser { UserName = model.Email, Email = model.Email };
                    string userGuid = Guid.NewGuid().ToString();
                    string userEmail = $"{userGuid}@bcare.com.sa";
                    string password = userGuid;
                    var guestRole = _authorizationService.GetRoleByName("Guest");
                    if (guestRole != null)
                    {
                        var user = new AspNetUser
                        {
                            CreatedDate = DateTime.Now,
                            LastModifiedDate = DateTime.Now,
                            LastLoginDate = DateTime.Now,
                            LockoutEndDateUtc = DateTime.UtcNow,
                            DeviceToken = "",
                            Email = userEmail,
                            EmailConfirmed = true,
                            RoleId = guestRole.ID,
                            LanguageId = CurrentLanguage.Id,
                            PhoneNumber = "",
                            PhoneNumberConfirmed = false,
                            UserName = userGuid.Replace("-", ""),
                            FullName = ""
                        };
                        var result = UserManager.CreateAsync(user, password);
                        if (result.Result != null && result.Result.Succeeded)
                        {
                            SignInManager.SignInAsync(user, true, true);
                            AuthenticateThisRequest();
                            _currentUser = _httpContext.User;
                        }
                    }
                }
                return _currentUser;
            }
            set {
                _currentUser = value;
            }
        }

        //public UserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? _httpContext.GetOwinContext().GetUserManager<UserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}


        //public ClientSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? _httpContext.GetOwinContext().Get<ClientSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}
        public Tameenk.Services.Profile.Component.Membership.UserManager UserManager
        {
            get
            {
                return _userManager ?? _httpContext.GetOwinContext().GetUserManager<Tameenk.Services.Profile.Component.Membership.UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public Tameenk.Services.Profile.Component.Membership.ClientSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? _httpContext.GetOwinContext().Get<Tameenk.Services.Profile.Component.Membership.ClientSignInManager>();

            }
            private set
            {
                _signInManager = value;
            }
        }

        private Language CurrentLanguage
        {
            get {
                _language = (_language 
                    ?? _localizationService.GetLanguageByEnglishName(System.Globalization.CultureInfo.CurrentCulture.DisplayName))
                    ?? _localizationService.GetDefaultLanguage();
                return _language;
            }
            set {
                _language = value;
            }
        }
        #endregion

    }
}