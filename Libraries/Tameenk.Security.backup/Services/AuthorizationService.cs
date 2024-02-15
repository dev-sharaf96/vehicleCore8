using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Caching;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Identity;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Account;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Security.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly ISmsProvider _smsService;

        private readonly IdentityDbContext<AspNetUser> _context;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly TameenkConfig _config;
        private readonly ILogger _logger;
        private readonly IHttpClient _httpClient;
        private const string TAMEENK_ROLE_ALL_CACHE_KEY = "Tameenk.Role.All";
        private const string TAMEENK_ROLE_NAME_CACHE_KEY = "Tameenk.Role.Name.{0}";
        private readonly IRepository<AutoleasingUser> _autoleasingUserRepository;
        private readonly IRepository<CorporateUsers> _corporateUsersRepository;

        public AuthorizationService(ICacheManager cacheManager, IRepository<Role> roleRepository,
            IRepository<Client> clientRepository, IdentityDbContext<AspNetUser> context, ISmsProvider smsService
           , TameenkConfig config, ILogger logger, IHttpClient httpClient, IRepository<AutoleasingUser> autoleasingUserrepository, IRepository<CorporateUsers> corporateUsersRepository)
        {
            _cacheManager = cacheManager ?? throw new TameenkArgumentNullException(nameof(ICacheManager));
            _roleRepository = roleRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Role>));
            _clientRepository = clientRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<Client>));
            _smsService = smsService ?? throw new TameenkArgumentNullException(nameof(ISmsProvider));
            _context = context ?? throw new TameenkArgumentNullException(nameof(IdentityDbContext<AspNetUser>));
            _userManager = new UserManager<AspNetUser>(new UserStore<AspNetUser>(_context));
            _config = config;
            _logger = logger;
            _httpClient = httpClient;
            _autoleasingUserRepository = autoleasingUserrepository ?? throw new TameenkArgumentNullException(nameof(IRepository<AutoleasingUser>));
            _corporateUsersRepository = corporateUsersRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<CorporateUsers>));

            #region UserManager Config
            // Configure validation logic for usernames
            _userManager.UserValidator = new UserValidator<AspNetUser>(_userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            _userManager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = false,
                RequireUppercase = false
            };

            _userManager.UserLockoutEnabledByDefault = true;
            _userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            _userManager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            _userManager.RegisterTwoFactorProvider("EnglishPhoneCodeMessage", new PhoneNumberTokenProvider<AspNetUser>
            {
                MessageFormat = "Your #Tameenk security code is: {0}"
            });
            _userManager.RegisterTwoFactorProvider("ArabicPhoneCodeMessage", new PhoneNumberTokenProvider<AspNetUser>
            {
                MessageFormat = "رمز تحقق #تأمينك من بي كير هو: {0}"
            });
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ClientUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your Security Code is{0}"
            //});
            //manager.EmailService = new EmailService();

            _userManager.SmsService = new SmsService(EngineContext.Current.Resolve<ISmsProvider>());
            //var dataProtectionProvider = options.DataProtectionProvider;
            //if (dataProtectionProvider != null)
            //{
            //    _userManager.UserTokenProvider =
            //        new DataProtectorTokenProvider<AspNetUser>(dataProtectionProvider.Create("TameenkIdentityConfirmation"));
            //}
            #endregion


        }

        public IList<Role> GetAllRoles()
        {
            return _cacheManager.Get(TAMEENK_ROLE_ALL_CACHE_KEY, () =>
            {
                return _roleRepository.Table.ToList();
            });
        }

        public Role GetRoleByName(string roleName)
        {
            var cacheKey = string.Format(TAMEENK_ROLE_NAME_CACHE_KEY, roleName);
            return _cacheManager.Get(cacheKey, () =>
            {
                return _roleRepository.Table.FirstOrDefault(r => r.RoleNameEN.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            });
        }

        public virtual Client GetClientById(string id)
        {
            if (id.ToLower() == "684C02DE-C782-4C7A-9999-70E687D73CD6".ToLower())
            {
                Client client = new Client();
                client.Name = "TameenkApp";
                client.AllowedOrigin = "*";
                client.ApplicationType = Core.Domain.Enums.Identity.ApplicationTypes.NativeConfidential;
                client.Active = true;
                client.Id = "684C02DE-C782-4C7A-9999-70E687D73CD6";
                client.RefreshTokenLifeTime = 1800;
                client.Secret = "/jvw1mu+yF4Y0ZiDiCeJlUbSddazrSH4xkPkzQBXnuE=";
                client.AuthServerUrl = null;
                client.RedirectUrl = null;
                return client;
            }
            else
            {
                var query = _clientRepository.Table.FirstOrDefault(c => c.Id == id);
                return query;
            }
        }

        public async Task<AspNetUser> GetUser(string id)
        {
            //AspNetUser user = await _userManager.FindByIdAsync(id);
            Core.Domain.Entities.AspNetUser user = _userManager.FindById(id);
            return user;
        }

        public async Task<AspNetUser> FindUser(string userName, string password)
        {
            AspNetUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public async Task<List<string>> CreateUser(AspNetUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                return new List<string>();

            return result.Errors.ToList();
        }

        public virtual async Task<bool> SendTwoFactorCodeSmsAsync(string userId, string phoneNumber)
        {
            try
            {
                if (userId == null)
                {
                    return false;
                }
                // todo
                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);
                // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
                await _userManager.NotifyTwoFactorTokenAsync(userId, VerificationCodeResource.SmsTwoFactorCodeProviderName, token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool ChangePhoneNumber(string userId, string phoneNumber, string code, out string errors)
        {
            errors = "";
            var result = _userManager.ChangePhoneNumber(userId, phoneNumber, code);
            errors = JsonConvert.SerializeObject(result.Errors);
            return result.Succeeded;
        }


        public string GetUserId(IPrincipal user)
        {
            string userId = string.Empty;
            var claimsIdentity = user.Identity as System.Security.Claims.ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userClaim = claimsIdentity.Claims.FirstOrDefault(e => e.Type == "curent_user_id");
                if (userClaim != null)
                {
                    userId = userClaim.Value;
                }
            }
            return userId;
        }


        public AccessTokenResult GetAccessToken(string userId)
        {
           // IdentityRequestLog identityRequestLog = new IdentityRequestLog();
            try
            {
                //identityRequestLog.UserId = userId;
                //identityRequestLog.ClientId = _config.Identity.ClientId;
                //identityRequestLog.ClientSecret = _config.Identity.ClientSecret;
                //identityRequestLog.Method = "GetAccessToken";

                if (string.IsNullOrEmpty(_config.Identity.ClientId))
                {
                    //identityRequestLog.ErrorCode = 2;
                    //identityRequestLog.ErrorDescription = "ClientId is null";
                    //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);
                    return null;
                }
                if (string.IsNullOrEmpty(_config.Identity.ClientSecret))
                {
                    //identityRequestLog.ErrorCode = 3;
                    //identityRequestLog.ErrorDescription = "ClientSecret is null";
                    //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);
                    return null;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    //identityRequestLog.ErrorCode = 4;
                    //identityRequestLog.ErrorDescription = "UserId is null";
                    //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);
                    return null;
                }
                var formParamters = new Dictionary<string, string>();
                formParamters.Add("grant_type", "client_credentials");
                formParamters.Add("client_Id", _config.Identity.ClientId);
                formParamters.Add("client_secret", _config.Identity.ClientSecret);
                formParamters.Add("curent_user_id", userId);

                var content = new FormUrlEncodedContent(formParamters);
                var postTask = _httpClient.PostAsync($"{_config.Identity.Url}token", content);
                postTask.ConfigureAwait(false);
                postTask.Wait();
                var response = postTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AccessTokenResult>(jsonString);
                    result.TokenExpirationDate = DateTime.Now.AddMinutes(30);

                    //if (!string.IsNullOrEmpty(userId) && userId != Guid.Empty.ToString())
                    //{
                    //    var user = GetUserDBByID(userId);
                    //    if (user != null)
                    //        result.IsCorporateUser = (user.IsCorporateUser) ? true : false;
                    //}

                    //identityRequestLog.Response = jsonString;
                    //identityRequestLog.ErrorCode = 1;
                    //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);
                    return result;
                }
                //identityRequestLog.ErrorCode = 5;
                //identityRequestLog.ErrorDescription = "Response is not success";
                //identityRequestLog.Response = response.Content.ReadAsStringAsync().Result.ToString();
                //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);
                return null;

            }
            catch (Exception ex)
            {
                //identityRequestLog.ErrorCode = 6;
                //identityRequestLog.ErrorDescription = ex.ToString();
                //IdentityRequestLogDataAccess.AddToIdentityRequestLog(identityRequestLog);
                return null;
            }
        }

        public List<Core.Domain.Entities.AspNetUser> GetUsers(int PageIndex = 0, int PageSize = 10)
        {
            return _userManager.Users.ToList();
        }
        public int Update(UpdateCustomertModel entity)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == entity.UserId);
            user.PoliciesCount = entity.PoliciesCount;
            user.PromotionCodeCount = entity.PromotionCodeCount;

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            return _context.SaveChanges();

        }

        public Core.Domain.Entities.AspNetUser GetUserInfoByEmail(string email, string userId, string mobile)
        {
            var query = _userManager.Users.Include(x => x.Language);

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(a => a.Email.ToLower() == email.ToLower());
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(a => a.Id.ToLower() == userId.ToLower());
            }

            if (!string.IsNullOrEmpty(mobile))
            {
                query = query.Where(a => a.PhoneNumber == mobile);
            }
            return query.FirstOrDefault();
        }

        public bool DeleteUser(string mobile)
        {
            var user = _userManager.Users.Where(x => x.PhoneNumber == mobile).FirstOrDefault();
            if (user != null)
            {
                var result = _userManager.Delete(user);
                return result.Succeeded;
            }
            return false;
        }

        public bool ManageUserLock(string userId, bool toLock, string lockedby, string lockedreason)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return false;
            }

            user.LockoutEnabled = toLock;
            user.LockoutEndDateUtc = toLock ? new DateTime(3000, 12, 31) : DateTime.Now;
            user.LockedBy = lockedby;
            user.LockedReason = lockedreason;
            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            return _context.SaveChanges() > 0 ? true : false;
        }
        public AspNetUser IsEmailOrPhoneExist(string email, string mobile)
        {
            var user = _userManager.Users.Where(u => u.Email.ToLower() == email.ToLower() || u.PhoneNumber == mobile).FirstOrDefault();
            if (user != null)
            {
                if (user.EmailConfirmed && user.PhoneNumberConfirmed)
                {
                    return user;
                }
                else
                {
                    _userManager.Delete(user);
                    return null;
                }
            }
            return null;
        }

        public AspNetUser GetUserDBByID(string userId)
        {
            AspNetUser user = null;

            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            dbContext.DatabaseInstance.CommandTimeout = 60;
            var command = dbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetUserDBByID";
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
            command.Parameters.Add(userIdParam);

            dbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            user = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<AspNetUser>(reader).FirstOrDefault();

            dbContext.DatabaseInstance.Connection.Close();
            return user;
        }

        public int ConfirmUserPhoneNumberDB(string userId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            dbContext.DatabaseInstance.CommandTimeout = 60;
            var command = dbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "ConfirmUserPhoneNumberDB";
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
            command.Parameters.Add(userIdParam);

            dbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

            dbContext.DatabaseInstance.Connection.Close();
            return result;
        }

        public int ConfirmUserEmailDB(string userId)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            dbContext.DatabaseInstance.CommandTimeout = 60;
            var command = dbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "ConfirmUserEmailDB";
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
            command.Parameters.Add(userIdParam);

            dbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

            dbContext.DatabaseInstance.Connection.Close();
            return result;
        }


        public bool CheckEmailExist(string email)
        {
            bool exist = false;
            var user = _userManager.Users.FirstOrDefault(x => x.Email == email);
            if (user != null)
                exist = true;

            return exist;
        }

        public bool CheckPhoneExist(string phone)
        {
            bool exist = false;
            string validinternationalphonenumber = Utilities.ValidatePhoneNumber(phone);
            string validlocalphonenumber =  "0" + validinternationalphonenumber.Substring(Utilities.SaudiInternationalPhoneCode.Length);
            var user = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == phone||x.PhoneNumber== validinternationalphonenumber||x.PhoneNumber==validlocalphonenumber);
            if (user != null)
                exist = true;

            return exist;
        }
        public bool UpdateUserEmail(string email, string userId,string phone)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return false;
            if (!string.IsNullOrEmpty(email))
            {
                user.Email = email;
            }
            if (!string.IsNullOrEmpty(phone))
            {
                user.PhoneNumber = phone;
            }
            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            return _context.SaveChanges() > 0 ? true : false;
        }
        public bool IsUserAutoleasingAuthorized(string userId)
        {
            var user = _autoleasingUserRepository.TableNoTracking.Where(x => x.UserId == userId).FirstOrDefault();
            if (user != null)
                return true;
            return false;
        }

        public bool UpdateUserInfo(AspNetUser aspNetUser, out string exception)
        {
            exception = string.Empty;
            try
            {
                if (aspNetUser == null)
                    return false;

                _context.Users.Attach(aspNetUser);
                _context.Entry(aspNetUser).State = EntityState.Modified;
                return _context.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public List<AspNetUser> GetSuperAdmins()
        {
            return _userManager.Users.Where(x => x.IsAutoLeasingSuperAdmin == true && x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc.Value.Year.ToString() != "3000").ToList();
        }

        public List<AspNetUser> GetSuperAdminsRelated(string adminId)
        {
            return _userManager.Users.Where(x => x.AutoLeasingAdminId == adminId).ToList();
        }
        public AspNetUser GetUserByEmail(string email)
        {
            return _userManager.FindByEmail(email);
        }

        public CorporateUsers GetCorporateUseByEmail(string email)
        {
            return _corporateUsersRepository.TableNoTracking.Where(x => x.UserName == email).FirstOrDefault();
        }

        public string GenerateTokenJWT(string ID, string Email, string userName,string phone, string fullNameAr, string fullNameEn,  out string key)
        {
            var x = Guid.NewGuid().ToString();
            var claims = new[]
                       {
                          new Claim(ClaimTypes.NameIdentifier, ID),
                          new Claim(ClaimTypes.Email, Email),
                          new Claim(ClaimTypes.Name, userName),
                          new Claim(ClaimTypes.MobilePhone, phone),
                          new Claim("auth_time", DateTime.Now.ToString()),
                          new Claim("Key", x.ToString()),
                          new Claim("fullNameAr", fullNameAr),
                          new Claim("fullNameEn", fullNameEn)
                        };

            var secrectkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("t@MeEnKHa$HeKeYsEcrEt@2021"));
            var creds = new SigningCredentials(secrectkey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken("Bcare",
              "Bcare",
              claims,
              expires: DateTime.Now.AddMinutes(30), //.AddMinutes(60),
              signingCredentials: creds);
            key = x.ToString();
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedJwt;
        }

        public void GetAllUsersByPhoneAndUpdate(string phone, string currentUserId, out string exception)
        {
            try
            {
                exception = string.Empty;
                string validinternationalphonenumber = Utilities.ValidatePhoneNumber(phone);
                string validlocalphonenumber = "0" + validinternationalphonenumber.Substring(Utilities.SaudiInternationalPhoneCode.Length);
                var users = _userManager.Users.Where(x => x.PhoneNumber == phone || x.PhoneNumber == validlocalphonenumber || x.PhoneNumber == validinternationalphonenumber).ToList();
                if (users == null)
                {
                    exception = string.Empty;
                    return;
                }

                users = users.Where(x => x.Id != currentUserId).ToList();
                if (users == null)
                {
                    exception = string.Empty;
                    return;
                }

                exception = string.Empty;
                foreach (var user in users)
                {
                    user.PhoneNumber = null;
                    user.PhoneNumberConfirmed = false;
                    user.IsPhoneVerifiedByYakeen = false;
                    UpdateUserInfo(user, out exception);
                    if (!string.IsNullOrEmpty(exception))
                        return;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return;
            }
        }

        public bool CheckUserWithNationalAndDifferentEmail(string nationalId, string email)
        {
            var users = _userManager.Users.Where(x => x.NationalId == nationalId && x.Email != email && x.IsPhoneVerifiedByYakeen).FirstOrDefault();
            if (users == null)
                return false;

            return true;
        }

        public bool ManageUserMobileVerification(string email, string userId, bool isVerified, out string exception) // , string nationalId, string phone
        {
            exception = string.Empty;
            try
            {
                var user = _userManager.Users.Where(x => x.Id == userId && x.Email == email).FirstOrDefault();
                if (user == null)
                {
                    exception = $"No user found with Id: {userId} and email: {email}";
                    return false;
                }

                user.IsPhoneVerifiedByYakeen = isVerified;
                //if (isVerified == true)
                //{
                //    user.NationalId = nationalId;
                //    user.PhoneNumber = phone;
                //}

                _context.Users.Attach(user);
                _context.Entry(user).State = EntityState.Modified;
                return _context.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                exception = $"Exception error: {ex.ToString()}";
                return false;
            }
        }

        public AspNetUser GetUserInfoByNationalId(string nationalId)
        {
            return _userManager.Users.Where(x => x.NationalId == nationalId).FirstOrDefault();
        }
    }
}
