using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using Tameenk.Common.Utilities;
using Tameenk.Core;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Dto.PromotionProgram;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Promotions;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Promotions;

namespace Tameenk.Services.Implementation.Promotions
{
    public class PromotionService : IPromotionService
    {
        #region Fields
        private readonly IRepository<PromotionProgram> _promotionProgramRepository;
        private readonly IRepository<PromotionProgramUser> _promotionProgramUserRepository;
        private readonly IRepository<PromotionProgramDomain> _promotionProgramDomianRepository;
        private readonly IRepository<PromotionProgramCode> _promotionProgramCodeRepository;
        private readonly IRepository<UserPurchasedPromotionPrograms> _userPurchasedPromotionPrograms;
        private readonly IHttpClient _httpClient;
        private readonly IRepository<DeservingDiscount> _deservingDiscount;
        private readonly IRepository<PromotionProgramNins> _promotionProgramNinsRepository;
        private readonly IRepository<PromotionUser> _promotionUserRepository;

        #endregion

        #region Ctor

        public PromotionService(IRepository<PromotionProgram> promotionProgramRepository,
            IRepository<PromotionProgramUser> promotionProgramUserRepository,
            IRepository<PromotionProgramDomain> promotionProgramDomianRepository,
            IRepository<PromotionProgramCode> promotionProgramCodeRepository
           , IRepository<UserPurchasedPromotionPrograms> userPurchasedPromotionPrograms,
            IRepository<DeservingDiscount> deservingDiscount,
            IRepository<PromotionProgramNins> promotionProgramNinsRepository,
            IRepository<PromotionUser> promotionUserRepository)
        {
            _promotionProgramRepository = promotionProgramRepository ?? throw new ArgumentNullException(nameof(IRepository<PromotionProgram>));
            _promotionProgramUserRepository = promotionProgramUserRepository ?? throw new ArgumentNullException(nameof(IRepository<PromotionProgramUser>));
            _promotionProgramDomianRepository = promotionProgramDomianRepository ?? throw new ArgumentNullException(nameof(IRepository<PromotionProgramDomain>));
            _promotionProgramCodeRepository = promotionProgramCodeRepository;
            _userPurchasedPromotionPrograms = userPurchasedPromotionPrograms;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();
            _deservingDiscount = deservingDiscount;
            _promotionProgramNinsRepository = promotionProgramNinsRepository;
            _promotionUserRepository = promotionUserRepository;
        }



        #endregion

        #region Methods

        #region Promotion Program


        public PromotionProgram GetPromotionProgram(int id)
        {
            return _promotionProgramRepository.Table.FirstOrDefault(e => e.Id == id);
        }

        public IPagedList<PromotionProgram> GetPromotionPrograms(bool getActiveOnly = true, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var qry = _promotionProgramRepository.TableNoTracking;
            if (getActiveOnly)
            {
                qry = qry.Where(e => e.IsActive == true &&
                                (e.EffectiveDate == null || e.EffectiveDate <= DateTime.Now) &&
                                (e.DeactivatedDate == null || e.DeactivatedDate >= DateTime.Now));
            }
            return new PagedList<PromotionProgram>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }

        public List<PromotionProgram> GetPromotionProgramsNoTracking(bool getActiveOnly = true)
        {
            var qry = _promotionProgramRepository.TableNoTracking;
            if (getActiveOnly)
            {
                qry = qry.Where(e => e.IsActive == true &&
                                (e.EffectiveDate == null || e.EffectiveDate <= DateTime.Now) &&
                                (e.DeactivatedDate == null || e.DeactivatedDate >= DateTime.Now));
            }

            return qry.ToList();
        }

        public IPagedList<PromotionProgram> GetAllPromotionPrograms(int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var qry = _promotionProgramRepository.Table;
            return new PagedList<PromotionProgram>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }
        public PromotionProgram AddPromotionProgram(PromotionProgram promotionProgram)
        {
            _promotionProgramRepository.Insert(promotionProgram);
            return promotionProgram;
        }

        public PromotionProgram UpdatePromotionProgram(PromotionProgram promotionProgram)
        {
            _promotionProgramRepository.Update(promotionProgram);
            return promotionProgram;
        }

        public PromotionProgramUser AddUserToPromotionProgram(string userId, string email, int programId)
        {
            if (programId < 1)
            {
                throw new TameenkArgumentException("Missing program identifier.", nameof(programId));
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new TameenkArgumentNullException(nameof(userId), "User id can't be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new TameenkArgumentNullException(nameof(email), "Email can't be null or empty.");
            }

            PromotionProgramUser promotionProgramUser = new PromotionProgramUser
            {
                Email = email,
                EmailVerified = false,
                CreationDate = DateTime.Now
            };
            promotionProgramUser.UserId = userId;
            promotionProgramUser.PromotionProgramId = programId;

            _promotionProgramUserRepository.Insert(promotionProgramUser);
            return promotionProgramUser;
        }
        public EnrollUserToProgramModel EnrollAndApproveUSerToPromotionProgram(string userId, int promotionProgramId, string email)
        {
            if (string.IsNullOrEmpty(userId))
                throw new TameenkArgumentNullException(nameof(userId), "User id can not be empty.");

            if (promotionProgramId < 1)
                throw new TameenkArgumentException("Promotion program id cant be less than 1", nameof(promotionProgramId));

            if (string.IsNullOrEmpty(email))
                throw new TameenkArgumentNullException(nameof(email), "Email can not be empty.");


            var response = new EnrollUserToProgramModel();

            try
            {
                var promotionProgUser = GetPromotionProgramUser(userId);

                if (promotionProgUser == null)
                {
                    //then add new user
                    ApproveUserToPromotionProgram(userId, email, promotionProgramId);
                    response.UserEndrollerd = true;
                    return response;
                }
                else
                {
                    //if  not verified yet then update the current record with the new data
                    if (!promotionProgUser.EmailVerified)
                    {
                        promotionProgUser.PromotionProgramId = promotionProgramId;
                        promotionProgUser.Email = email;
                        UpdatePromotionProgramUser(promotionProgUser);
                        response.UserEndrollerd = true;
                        return response;
                    }
                    else
                    {
                        //return error message indicate that the user already joined in a promotioin program
                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>()
                        {
                            new ErrorModel()
                            {
                                Description = "This uer already joined a promotion program. Please exist your current promotion program then try again."
                            }
                        };
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {

                response.UserEndrollerd = false;
                response.Errors = new List<ErrorModel>()
                {
                    new ErrorModel(){Description = ex.GetBaseException().Message}
                };
                return response;
            }

        }

        public PromotionProgramUser ApproveUserToPromotionProgram(string userId, string email, int programId)
        {
            if (programId < 1)
            {
                throw new TameenkArgumentException("Missing program identifier.", nameof(programId));
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new TameenkArgumentNullException(nameof(userId), "User id can't be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new TameenkArgumentNullException(nameof(email), "Email can't be null or empty.");
            }

            PromotionProgramUser promotionProgramUser = new PromotionProgramUser
            {
                Email = email,
                EmailVerified = true,
                CreationDate = DateTime.Now
            };
            promotionProgramUser.UserId = userId;
            promotionProgramUser.PromotionProgramId = programId;

            _promotionProgramUserRepository.Insert(promotionProgramUser);
            return promotionProgramUser;
        }
        public bool IsUserDomainExistInProgramDomains(string userEmail, int? programId)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new TameenkArgumentNullException(nameof(userEmail), "Email can't be null or empty.");
            }
            if (!programId.HasValue || programId <= 0)
            {
                throw new TameenkArgumentException("Promotion program id can't be less than 1.", nameof(programId));
            }

            MailAddress address = new MailAddress(userEmail);
            var programDomain = _promotionProgramDomianRepository.Table.Where(e => e.Domian == address.Host && e.PromotionProgramId == programId).FirstOrDefault();
            return programDomain == null ? false : true;
        }

        public List<PromotionProgramDomain> IsUserHasProgramDomains(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new TameenkArgumentNullException(nameof(userEmail), "Email can't be null or empty.");
            }
            MailAddress address = new MailAddress(userEmail);

            var list = _promotionProgramDomianRepository.Table.Where(e => e.Domian == address.Host&&e.IsActive==true).ToList();
            return list;
        }

        public List<PromotionProgramDomain> GetActiveDomainForUser(string userEmail,out bool isActive)
        {
            isActive = true;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new TameenkArgumentNullException(nameof(userEmail), "Email can't be null or empty.");
            }
            MailAddress address = new MailAddress(userEmail);

            var list = _promotionProgramDomianRepository.Table.Where(e => e.Domian == address.Host).ToList();
            if (list.Where(x => x.IsActive == true).ToList().Count() == 0)
                isActive = false;
            return list.Where(x=> x.PromotionProgramId != null).ToList();
        }

        public PromotionProgramDomain GetPromotionProgramBydomain(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new TameenkArgumentNullException(nameof(userEmail), "Email can't be null or empty.");
            }
            MailAddress address = new MailAddress(userEmail);
            if (string.IsNullOrWhiteSpace(address.Host))
            {
                throw new TameenkArgumentNullException(nameof(userEmail), "address.Host can't be null or empty.");
            }
            var programDomain = _promotionProgramDomianRepository.Table.FirstOrDefault(e => e.Domian == address.Host);
            if (programDomain != null)
                return programDomain;
            else
                return null;
           
        }

        public PromotionProgram GetPromotionProgramNoTracking(int id)
        {
            return _promotionProgramRepository.TableNoTracking.FirstOrDefault(e => e.Id == id);
        }
        #endregion


        #region Promotion Program User

        /// <summary>
        /// Get user promotion codes by user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetUserPromotionCodeCount(string id)
        {
            int userPromotionCodeCount = 0;
            if (_userPurchasedPromotionPrograms.TableNoTracking != null)
                userPromotionCodeCount = _userPurchasedPromotionPrograms.TableNoTracking.Where(x => x.UserId == id).Count();
            return userPromotionCodeCount;
        }
        public PromotionProgramUser GetPromotionProgramUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new TameenkArgumentNullException(nameof(userId), "User id can not be null.");
            }

            return _promotionProgramUserRepository.TableNoTracking
                .Include(e => e.PromotionProgram).OrderByDescending(p => p.Id).FirstOrDefault(e => e.UserId == userId);
        }
        public PromotionProgramUser GetPromotionProgramUserByUserIdAndEmail(string userId, string email)        {            if (string.IsNullOrWhiteSpace(userId))            {                throw new TameenkArgumentNullException(nameof(userId), "User id can not be null.");            }            return _promotionProgramUserRepository.Table                .Include(e => e.PromotionProgram).FirstOrDefault(e => e.UserId == userId && e.Email == email);        }
        /// <summary>
        /// get user programs related to it's domain.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPagedList<PromotionProgram> GetUserPromotionPrograms(string userEmail, int pageIndex = 0, int pageSize = int.MaxValue)
        {
         var result=  IsUserHasProgramDomains(userEmail).FirstOrDefault().Domian;
        var progLst= _promotionProgramDomianRepository.Table.Where(x => x.Domian == result).Include(e => e.PromotionProgram).ToList().Select(x => x.PromotionProgram);
            return new PagedList<PromotionProgram>(progLst.ToList(), pageIndex,pageSize);
        }

        public List<PromotionProgramDTO> GetUserPromotionPrograms(string userEmail)
        {
            var result = IsUserHasProgramDomains(userEmail).FirstOrDefault();
            if (result == null)
                return null;
            var progLst = _promotionProgramDomianRepository.Table.Where(x => x.Domian == result.Domian)
                .Include(e => e.PromotionProgram).ToList()
                .Select(x => x.PromotionProgram)
                .ToList();
            var newList = progLst.Select(x => new PromotionProgramDTO
            {
                CreatedBy = x.CreatedBy,
                CreationDate = x.CreationDate,
                DeactivatedDate = x.DeactivatedDate,
                Description = x.Description,
                EffectiveDate = x.EffectiveDate,
                Id = x.Id,
                IsActive = x.IsActive,
                Key = x.Key,
                ModificationDate = x.ModificationDate,
                ModifiedBy = x.ModifiedBy,
                Name = x.Name,
                ValidationMethodId = x.ValidationMethodId,
                IsPromoByEmail = x.IsPromoByEmail
            }).ToList();
            return newList;
        }


        public PromotionProgramUser UpdatePromotionProgramUser(PromotionProgramUser entity)
        {
            if (entity == null)
                throw new TameenkArgumentNullException(nameof(entity), "Entity to update can not be null.");
            _promotionProgramUserRepository.Update(entity);
            return entity;
        }

        public void ConfirmUserJoinProgram(string userId, int programId, string email)
        {
            if (string.IsNullOrEmpty(userId))
                throw new TameenkArgumentNullException(nameof(userId), "User Id can not be null.");

            if (string.IsNullOrEmpty(email))
                throw new TameenkArgumentNullException(nameof(email), "Email can not be null.");

            if (programId < 1)
                throw new TameenkArgumentException("Promotion program can not be less than 1.", nameof(email));

            var programUser = _promotionProgramUserRepository.Table
                .Where(e => e.UserId == userId && e.PromotionProgramId == programId && e.Email == email && e.EmailVerified == false)
                .OrderByDescending(x=>x.Id).FirstOrDefault();

            if (programUser == null)
                throw new TameenkEntityNotFoundException("userId", "This user didn't request to join this promotion program.");

            programUser.EmailVerified = true;
            _promotionProgramUserRepository.Update(programUser);
        }

        public EnrollUserToProgramModel EnrollUSerToPromotionProgram(string userId, int promotionProgramId, string email)
        {
            if (string.IsNullOrEmpty(userId))
                throw new TameenkArgumentNullException(nameof(userId), "User id can not be empty.");

            if (promotionProgramId < 1)
                throw new TameenkArgumentException("Promotion program id cant be less than 1", nameof(promotionProgramId));

            if (string.IsNullOrEmpty(email))
                throw new TameenkArgumentNullException(nameof(email), "Email can not be empty.");


            var response = new EnrollUserToProgramModel();

            try
            {
                var promotionProgUser = GetPromotionProgramUserByUserIdAndEmail(userId,email);

                if (promotionProgUser == null)
                {
                    //then add new user
                    AddUserToPromotionProgram(userId, email, promotionProgramId);
                    response.UserEndrollerd = true;
                    return response;
                }
                else
                {
                    //if  not verified yet then update the current record with the new data
                    if (!promotionProgUser.EmailVerified)
                    {
                        promotionProgUser.PromotionProgramId = promotionProgramId;
                        promotionProgUser.Email = email;
                        UpdatePromotionProgramUser(promotionProgUser);
                        response.UserEndrollerd = true;
                        return response;
                    }
                    else
                    {
                        //return error message indicate that the user already joined in a promotioin program
                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>()
                        {
                            new ErrorModel()
                            {
                                Description = "This uer already joined a promotion program. Please exist your current promotion program then try again."
                            }
                        };
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {

                response.UserEndrollerd = false;
                response.Errors = new List<ErrorModel>()
                {
                    new ErrorModel(){Description = ex.GetBaseException().Message}
                };
                return response;
            }

        }

        public void DisenrollUserFromPromotionProgram(string userId)
        {
            //if (string.IsNullOrWhiteSpace(userId))
            //    throw new TameenkArgumentNullException(nameof(userId), "User id can't be null.");

            //var progUser = _promotionProgramUserRepository.Table.Where(e => e.UserId == userId).ToList();
            //if (progUser == null || progUser.Count == 0)
            //    throw new TameenkEntityNotFoundException("The current loged in user is't enrolled to promotion program.");

            //foreach (var item in progUser)
            //{
            //    _promotionProgramUserRepository.Delete(item);
            //}

            if (string.IsNullOrWhiteSpace(userId))
                throw new TameenkArgumentNullException(nameof(userId), "User id can't be null.");

            var progUser = _promotionUserRepository.Table.Where(e => e.UserId == userId).ToList();
            if (progUser == null || progUser.Count == 0)
                throw new TameenkEntityNotFoundException("The current loged in user is't enrolled to promotion program.");

            foreach (var item in progUser)
            {
                item.IsDeleted = true;
            }

            _promotionUserRepository.Update(progUser);
        }

        public bool IsEmailAlreadyUsed(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new TameenkArgumentNullException(nameof(email), "Email should not be null.");

            var programUSer = _promotionProgramUserRepository.Table.FirstOrDefault(e => e.EmailVerified == true && e.Email == email);
            return programUSer == null ? false : true;
        }

        public string ValidateBeforeJoinProgram(string userInput, int? programId = null, bool isPromoByEmail = true)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                throw new TameenkArgumentNullException(nameof(userInput), "User Input can't be null or empty.");
            if (programId < 1)
                throw new TameenkArgumentException("Program id can't be less than 1.", nameof(programId));

            if (isPromoByEmail)
            {
                //validate that user's domain exist in selected program's domain
                if (programId.HasValue && !IsUserDomainExistInProgramDomains(userInput, (int)programId))
                {
                    return PromotionProgramResource.UserDomianDosntExistInProgramDomains;
                }
                //validate that given email is not already used 
                if (IsEmailAlreadyUsed(userInput))
                {
                    return PromotionProgramResource.EmailAlreadyUsedByAnotherUser;
                }
            }
            else
            {
                ServiceRequestLog log = new ServiceRequestLog();
                try
                {
                    string user_name = System.Configuration.ConfigurationManager.AppSettings["user_name"];
                    string pass_code = System.Configuration.ConfigurationManager.AppSettings["pass_code"];
                    int search_type = int.Parse(System.Configuration.ConfigurationManager.AppSettings["search_type"]);
                    string tracking_id = userInput;
                    string SCAServiceURL = System.Configuration.ConfigurationManager.AppSettings["SCAServiceURL"];
                    string queryParameters = $"?user_name={user_name}&pass_code={pass_code}&search_type={search_type}&tracking_id={tracking_id}";
                    log.ServiceURL = SCAServiceURL;
                    log.ServerIP = ServicesUtilities.GetServerIP();
                    log.Method = "ValidateBeforeJoinProgram";
                    log.Channel = Channel.Portal.ToString();
                    log.ServiceRequest = SCAServiceURL + queryParameters;
                    log.DriverNin = tracking_id;
                    DateTime dtBeforeCalling = DateTime.Now;
                    var response = _httpClient.GetAsync(SCAServiceURL + queryParameters).Result;
                    DateTime dtAfterCalling = DateTime.Now;
                   
                    log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                    if (!response.IsSuccessStatusCode)
                    {
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.ServiceException;
                        log.ErrorDescription = "service return an error and error is " + response;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return PromotionProgramResource.GeneralError;
                    }

                    var value = response.Content.ReadAsStringAsync().Result;
                    log.ServiceResponse = value;
                    if (string.IsNullOrEmpty(value))
                    {
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.NullResponse;
                        log.ErrorDescription = "service returned null";
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                        return PromotionProgramResource.GeneralError;
                    }

                    var resultList = JsonConvert.DeserializeObject<List<SCAResponse>>(value);
                    if (resultList == null || resultList.Count == 0)
                    {
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.DeserializeError;
                        log.ErrorDescription = "Failed to deserialize the returned value : " + value;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);

                        return PromotionProgramResource.GeneralError;
                    }

                    var result = resultList.First();

                    switch (result.Code)
                    {
                        case "03":
                            log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                            log.ErrorDescription = result.Message;
                            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            return PromotionProgramResource.SCAResponseCode_03;
                        case "04":
                            log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                            log.ErrorDescription = result.Message;
                            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            return PromotionProgramResource.SCAResponseCode_04;
                        case "05":
                            //This user has valid membership
                            log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.Success;
                            log.ErrorDescription = "Success";
                            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            break;
                        case "06":
                            log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                            log.ErrorDescription = result.Message;
                            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            return PromotionProgramResource.SCAResponseCode_06;
                        case "07":
                            log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                            log.ErrorDescription = result.Message;
                            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            return PromotionProgramResource.SCAResponseCode_07;
                        default:
                            log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                            log.ErrorDescription = result.Message + " " + result.Code;
                            ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                            return PromotionProgramResource.GeneralError + " " + result.Code;
                    }
                }
                catch(Exception exp)
                {
                    log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.ServiceException;
                    log.ErrorDescription = exp.ToString();
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return PromotionProgramResource.GeneralError;
                }
            }

            if (!programId.HasValue)
            {
                var list = GetActiveDomainForUser(userInput,out bool isActive);
                if (!isActive)
                    return PromotionProgramResource.thisdomainnotactive;
                if (list.Count == 0)
                    return PromotionProgramResource.UserDomianDosntExistInProgramDomains;
                else if (list.Count > 1)
                    return "multi";
                return list.FirstOrDefault().PromotionProgramId.ToString();
            }



            return string.Empty;
        }

        #endregion

        #region Promotion Codes

        public PromotionProgramCode GetPromotionCode(int id)
        {
            return _promotionProgramCodeRepository.Table.FirstOrDefault(e => e.Id == id);
        }

        public IPagedList<PromotionProgramCode> GetPromotionCodes(bool getActiveOnly = true, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var qry = _promotionProgramCodeRepository.Table;

            if (getActiveOnly)
            {
                qry = qry.Where(e => e.IsDeleted == false).Include(a => a.InsuranceCompany).Include(a => a.PromotionProgram);
            }
            return new PagedList<PromotionProgramCode>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }

        public PromotionProgramCode AddPromotionProgramCode(PromotionProgramCode promotionProgramCode)
        {
            _promotionProgramCodeRepository.Insert(promotionProgramCode);
            return promotionProgramCode;
        }

        public PromotionProgramCode UpdatePromotionProgramCode(PromotionProgramCode promotionProgramCode)
        {
            _promotionProgramCodeRepository.Update(promotionProgramCode);
            return promotionProgramCode;
        }
        #endregion


        #region Promotion Program Domain

        public PromotionProgramDomain GetPromotionProgramDomain(int id)
        {
            return _promotionProgramDomianRepository.Table.FirstOrDefault(e => e.Id == id);
        }

        public IPagedList<PromotionProgramDomain> GetPromotionProgramDomains(bool getActiveOnly = true, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var qry = _promotionProgramDomianRepository.Table;
            //if (getActiveOnly)
            //{
            //    qry = qry.Where(e => e.);
            //}
            return new PagedList<PromotionProgramDomain>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }

        public PromotionProgramDomain AddPromotionProgramDomain(PromotionProgramDomain PromotionProgramDomain)
        {
            _promotionProgramDomianRepository.Insert(PromotionProgramDomain);
            return PromotionProgramDomain;
        }

        public PromotionProgramDomain UpdatePromotionProgramDomain(PromotionProgramDomain PromotionProgramDomain)
        {
            _promotionProgramDomianRepository.Update(PromotionProgramDomain);
            return PromotionProgramDomain;
        }

        public IPagedList<PromotionProgramDomain> GetPromotionProgramDomainByProgramId(int promotionProgramId, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var qry = _promotionProgramDomianRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId);

            return new PagedList<PromotionProgramDomain>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }

        public IPagedList<PromotionProgramCode> GetPromotionProgramCodesByProgramId(int promotionProgramId, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            var qry = _promotionProgramCodeRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId).Include(a => a.InsuranceCompany);

            return new PagedList<PromotionProgramCode>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }

        public IPagedList<PromotionProgram> ChangePromotionStatus(int promotionProgramId, bool status, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            if (status == false)
            {
                // first mark all promotionUser as deleted for this programId
                var promotionUsers = _promotionUserRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId).ToList();
                if (promotionUsers != null && promotionUsers.Count > 0)
                {
                    foreach (var promotionUser in promotionUsers)
                    {
                        promotionUser.IsDeleted = true;
                    }
                    _promotionUserRepository.Update(promotionUsers);
                }

                // second mark all programCodes as deleted for this programId
                var programCodes = _promotionProgramCodeRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId).ToList();
                if (programCodes != null && programCodes.Count > 0)
                {
                    foreach (var code in programCodes)
                    {
                        code.IsDeleted = true;
                    }
                    _promotionProgramCodeRepository.Update(programCodes);
                }

                // third mark all programDomains as deleted for this programId
                var programDomains = _promotionProgramDomianRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId).ToList();
                if (programDomains != null && programDomains.Count > 0)
                {
                    foreach (var domain in programDomains)
                    {
                        domain.IsActive = false;
                    }
                    _promotionProgramDomianRepository.Update(programDomains);
                }

                // fourth mark this program as deleted
                var program = _promotionProgramRepository.Table.Where(a => a.Id == promotionProgramId).FirstOrDefault();
                if (program != null)
                {
                    program.IsActive = false;
                    _promotionProgramRepository.Update(program);
                }

                //var codes = _promotionProgramCodeRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId).ToList();
                //if (codes != null)
                //{
                //    foreach (var code in codes)
                //    {
                //        //if (status == true)
                //        //    code.IsDeleted = false;
                //        //else
                //        //    code.IsDeleted = true;
                //        _promotionProgramCodeRepository.Delete(code);
                //    }
                //}
                //PromotionProgram promotionProgram = _promotionProgramRepository.Table.FirstOrDefault(a => a.Id == promotionProgramId);
                //// promotionProgram.IsActive = status;
                //_promotionProgramRepository.Delete(promotionProgram);
            }

            else
            {
                // first mark all programCodes as deleted for this programId
                var programCodes = _promotionProgramCodeRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId).ToList();
                if (programCodes != null && programCodes.Count > 0)
                {
                    foreach (var code in programCodes)
                    {
                        code.IsDeleted = false;
                    }
                    _promotionProgramCodeRepository.Update(programCodes);
                }

                // second mark all programDomains as deleted for this programId
                var programDomains = _promotionProgramDomianRepository.Table.Where(a => a.PromotionProgramId == promotionProgramId).ToList();
                if (programDomains != null && programDomains.Count > 0)
                {
                    foreach (var domain in programDomains)
                    {
                        domain.IsActive = true;
                    }
                    _promotionProgramDomianRepository.Update(programDomains);
                }

                // third mark this program as deleted
                var program = _promotionProgramRepository.Table.Where(a => a.Id == promotionProgramId).FirstOrDefault();
                if (program != null)
                {
                    program.IsActive = true;
                    _promotionProgramRepository.Update(program);
                }
            }
            var qry = _promotionProgramRepository.TableNoTracking;
            return new PagedList<PromotionProgram>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }
            

        

        public IPagedList<PromotionProgram> UpdatePromotion(PromotionProgram promotionProgramObj, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            PromotionProgram promotionProgram = _promotionProgramRepository.Table.SingleOrDefault(a => a.Id == promotionProgramObj.Id);

            promotionProgram.Name = promotionProgramObj.Name;
            promotionProgram.Description = promotionProgramObj.Description;
            promotionProgram.EffectiveDate = (promotionProgramObj.EffectiveDate != null) ? promotionProgramObj.EffectiveDate : promotionProgram.EffectiveDate;
            promotionProgram.DeactivatedDate = (promotionProgramObj.DeactivatedDate != null) ? promotionProgramObj.DeactivatedDate : promotionProgram.DeactivatedDate;
            _promotionProgramRepository.Update(promotionProgram);
            var qry = _promotionProgramRepository.Table;
        

            return new PagedList<PromotionProgram>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }

        public IPagedList<PromotionProgramDomain> ChangePromotionDomainStatus(int promotionDomainId, bool status, int pageIndx = 0, int pageSize = int.MaxValue)
        {
            PromotionProgramDomain promotionProgramDomain = _promotionProgramDomianRepository.Table.SingleOrDefault(a => a.Id == promotionDomainId);
            promotionProgramDomain.IsActive = status;
            _promotionProgramDomianRepository.Update(promotionProgramDomain);
            var qry = _promotionProgramDomianRepository.Table.Where(a => a.PromotionProgramId == promotionProgramDomain.PromotionProgramId);


            return new PagedList<PromotionProgramDomain>(qry.OrderBy(e => e.Id), pageIndx, pageSize);
        }
        public PromotionProgramDomain UpdatePromotionDomain(PromotionProgramDomain promotionProgramObj)
        {
            PromotionProgramDomain promotionProgramDomain = _promotionProgramDomianRepository.Table.SingleOrDefault(a => a.Id == promotionProgramObj.Id);

            promotionProgramDomain.Domian = promotionProgramObj.Domian;
            promotionProgramDomain.DomainNameAr = promotionProgramObj.DomainNameAr;
            promotionProgramDomain.DomainNameEn = promotionProgramObj.DomainNameEn;


            _promotionProgramDomianRepository.Update(promotionProgramDomain);
            var qry = _promotionProgramDomianRepository.Table.Where(a => a.PromotionProgramId == promotionProgramDomain.PromotionProgramId);


            return (promotionProgramDomain);
        }

        public void AddBulkPromotionProgramDomain(List<PromotionProgramDomain> pomotionProgramDomains)
        {
            try
            {
                _promotionProgramDomianRepository.Insert(pomotionProgramDomains);
            }
            catch(Exception ex)
            {
                throw;
            }


        }

        public PromotionProgram GetPromotionProgramByKey(string promotionKey)
        {
            try
            {
                return _promotionProgramRepository.Table.FirstOrDefault( p => p.Key.ToLower() == promotionKey);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public int UpdateUserPromotionProgramWithNationalId(string userId,string promoCode, int promotionProgramId, int companyId,string nationalId,out string exception)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "UpdateUserPromotionProgramWithNationalId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter nationalIdParam = new SqlParameter() { ParameterName = "nationalId", Value = nationalId };
                SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = companyId };
                SqlParameter promoCodeParam = new SqlParameter() { ParameterName = "promoCode", Value = promoCode };
                SqlParameter promotionProgramIdParam = new SqlParameter() { ParameterName = "promotionProgramId", Value = promotionProgramId };

                command.Parameters.Add(nationalIdParam);
                command.Parameters.Add(userIdParam);
                command.Parameters.Add(insuranceCompanyIdParam);
                command.Parameters.Add(promoCodeParam);
                command.Parameters.Add(promotionProgramIdParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch(Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return -1;
            }
        }
        public PromotionProgramUserModel GetUserPromotionCodeInfo(string userId, string nationalId, int insuranceCompanyId, int insuranceTypeCode)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                if (insuranceCompanyId < 1)
                    throw new TameenkArgumentNullException(nameof(insuranceCompanyId), "Insurance company id can't be less than 1.");
                PromotionProgramUserModel promotionProgramUserInfo = null;
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetUserPromotionProgramInfo";
                command.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(userId) && userId != Guid.Empty.ToString())
                {
                    SqlParameter userIdParam = new SqlParameter() { ParameterName = "userId", Value = userId };
                    command.Parameters.Add(userIdParam);
                }
                SqlParameter nationalIdParam = new SqlParameter() { ParameterName = "nationalId", Value = nationalId };
                command.Parameters.Add(nationalIdParam);

                SqlParameter insuranceCompanyIdParam = new SqlParameter() { ParameterName = "insuranceCompanyId", Value = insuranceCompanyId };
                SqlParameter insuranceTypeCodeParam = new SqlParameter() { ParameterName = "insuranceTypeCode", Value = insuranceTypeCode };

                command.Parameters.Add(insuranceCompanyIdParam);
                command.Parameters.Add(insuranceTypeCodeParam);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                promotionProgramUserInfo = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PromotionProgramUserModel>(reader).FirstOrDefault();
                if (promotionProgramUserInfo == null)
                {
                    reader.NextResult();
                    promotionProgramUserInfo = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PromotionProgramUserModel>(reader).FirstOrDefault();
                }

                return promotionProgramUserInfo;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }


        #endregion

        #region Offers Sheet


        public List<DeservingDiscount> GetAllDeservingDiscountsFromDBWithFilter(DeservingDiscount model, int pageIndx, int pageSize, bool export, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllDeserviesOfferFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = (!string.IsNullOrWhiteSpace(model.NationalId)) ? model.NationalId : "" };
                command.Parameters.Add(NationalIdParameter);

                SqlParameter MobileParameter = new SqlParameter() { ParameterName = "Mobile", Value = (!string.IsNullOrWhiteSpace(model.Mobile)) ? model.Mobile : "" };
                command.Parameters.Add(MobileParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get listing data
                List<DeservingDiscount> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DeservingDiscount>(reader).ToList();

                //get data count
                reader.NextResult();
                totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        public bool DeleteDeservingOffersRecord(int id, out string exception)
        {
            exception = string.Empty;
            var rowData = _deservingDiscount.Table.Where(x => x.Id == id).FirstOrDefault();
            if (rowData == null)
            {
                exception = "There is no record with this id " + id;
                return false;
            }

            try
            {
                rowData.IsDeleted = true;
                _deservingDiscount.Update(rowData);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public void AddBulkOffersDataSheet(List<DeservingDiscount> data, out string exception)
        {
            exception = string.Empty;
            try
            {
                for (int i = 0; i < data.Count; i++)
                {
                    string NIN = data[i].NationalId;
                    var dbRow = _deservingDiscount.Table.FirstOrDefault(a => a.NationalId == NIN);
                    if (dbRow != null)
                    {
                         dbRow.IsDeleted = false;
                        _deservingDiscount.Update(dbRow);
                    }
                    else
                    {
                        data[i].IsDeleted = false;
                        _deservingDiscount.Insert(data[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
        }

        #endregion

        #endregion

        public List<PromotionProgramNins> GetPromotionProgramNationalIdsByProgramId(int promotionProgramId, int pageIndex, int pageSize, out string exception, out int totalCount)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            exception = string.Empty;
            totalCount = 0;
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetPromotionProgramNins";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter ProgramIdParam = new SqlParameter() { ParameterName = "programId", Value = promotionProgramId };
                command.Parameters.Add(ProgramIdParam);

                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                List<PromotionProgramNins> result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PromotionProgramNins>(reader).ToList();

                if (result != null && result.Count > 0)
                {
                    //get data count
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                dbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

        public bool DeleteNinFromPromotionProgram(int rowId, out string exception)
        {
            exception = string.Empty;
            var promotionProgramNin = _promotionProgramNinsRepository.Table.Where(x => x.Id == rowId).FirstOrDefault();
            if (promotionProgramNin == null)
                return false;

            try
            {
                promotionProgramNin.isDeleted = true;
                _promotionProgramNinsRepository.Update(promotionProgramNin);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public string SubmitServiceRequestForPromotionProgramsByNin(string nin)
        {
            ServiceRequestLog log = new ServiceRequestLog();
            try
            {
                string user_name = System.Configuration.ConfigurationManager.AppSettings["user_name"];
                string pass_code = System.Configuration.ConfigurationManager.AppSettings["pass_code"];
                int search_type = int.Parse(System.Configuration.ConfigurationManager.AppSettings["search_type"]);
                string tracking_id = nin;
                string SCAServiceURL = System.Configuration.ConfigurationManager.AppSettings["SCAServiceURL"];
                string queryParameters = $"?user_name={user_name}&pass_code={pass_code}&search_type={search_type}&tracking_id={tracking_id}";
                log.ServiceURL = SCAServiceURL;
                log.ServerIP = ServicesUtilities.GetServerIP();
                log.Method = "SubmitServiceRequestForPromotionProgramsByNin";
                log.Channel = Channel.Portal.ToString();
                log.ServiceRequest = SCAServiceURL + queryParameters;
                log.DriverNin = tracking_id;

                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.GetAsync(SCAServiceURL + queryParameters).Result;
                DateTime dtAfterCalling = DateTime.Now;

                log.ServiceResponseTimeInSeconds = dtAfterCalling.Subtract(dtBeforeCalling).TotalSeconds;
                if (!response.IsSuccessStatusCode)
                {
                    log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.ServiceException;
                    log.ErrorDescription = "service return an error and error is " + response;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return log.ErrorDescription;
                }

                var value = response.Content.ReadAsStringAsync().Result;
                log.ServiceResponse = value;
                if (string.IsNullOrEmpty(value))
                {
                    log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.NullResponse;
                    log.ErrorDescription = "service returned null";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return log.ErrorDescription;
                }

                var resultList = JsonConvert.DeserializeObject<List<SCAResponse>>(value);
                if (resultList == null || resultList.Count == 0)
                {
                    log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.DeserializeError;
                    log.ErrorDescription = "Failed to deserialize the returned value : " + value;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return log.ErrorDescription;
                }

                var result = resultList.First();
                switch (result.Code)
                {
                    case "03":
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                        log.ErrorDescription = result.Message;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return PromotionProgramResource.SCAResponseCode_03;
                    case "04":
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                        log.ErrorDescription = result.Message;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return PromotionProgramResource.SCAResponseCode_04;
                    case "05":
                        //This user has valid membership
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.Success;
                        log.ErrorDescription = "Success";
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        break;
                    case "06":
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                        log.ErrorDescription = result.Message;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return PromotionProgramResource.SCAResponseCode_06;
                    case "07":
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                        log.ErrorDescription = result.Message;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return PromotionProgramResource.SCAResponseCode_07;
                    default:
                        log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.CanNotJoin;
                        log.ErrorDescription = result.Message + " " + result.Code;
                        ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                        return PromotionProgramResource.GeneralError + " " + result.Code;
                }

                return string.Empty;
            }
            catch (Exception exp)
            {
                log.ErrorCode = (int)PromotionProgramOutput.ErrorCodes.ServiceException;
                log.ErrorDescription = exp.ToString();
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return exp.ToString();
            }
        }

        public string GetPromotionProgramUserNew(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new TameenkArgumentNullException(nameof(userId), "User id can not be null.");

            var _promotionUser = _promotionUserRepository.TableNoTracking.Where(e => e.UserId == userId && (e.EmailVerified == true || e.NinVerified == true) && (e.IsDeleted == null || e.IsDeleted == false)).OrderByDescending(a => a.Id).FirstOrDefault();
            if (_promotionUser == null)
                return string.Empty;

            var promotionProgram = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == _promotionUser.PromotionProgramId).FirstOrDefault();
            if (promotionProgram == null)
                return string.Empty;

            return promotionProgram.Name;
        }

        public List<PromotionUser> GetAllPromotionApprovalsFromDBWithFilter(PromotionProgramApprovalsFilterModel model, int pageIndx, int pageSize, out int totalCount, out string exception)
        {
            totalCount = 0;
            exception = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllApprovalsFromDBWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60;

                if (!string.IsNullOrEmpty(model.Nin))
                {
                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "NationalId", Value = model.Nin };
                    command.Parameters.Add(NationalIdParameter);
                }

                if (!string.IsNullOrEmpty(model.Email))
                {
                    SqlParameter EmailParameter = new SqlParameter() { ParameterName = "Email", Value = model.Email };
                    command.Parameters.Add(EmailParameter);
                }

                if (model.StartDate.HasValue)
                {
                    SqlParameter StartDateParameter = new SqlParameter() { ParameterName = "StartDate", Value = model.StartDate.Value.ToString("yyyy-MM-dd 00:00:00") };
                    command.Parameters.Add(StartDateParameter);
                }

                if (model.EndDate.HasValue)
                {
                    SqlParameter EndDateParameter = new SqlParameter() { ParameterName = "EndDate", Value = model.EndDate.Value.ToString("yyyy-MM-dd 23:59:59") };
                    command.Parameters.Add(EndDateParameter);
                }

                if (model.Status.HasValue)
                {
                    if (model.Status == 1)
                    {
                        SqlParameter ApprovedStatusParameter = new SqlParameter() { ParameterName = "ApprovedStatus", Value = 1 };
                        command.Parameters.Add(ApprovedStatusParameter);
                    }

                    else
                    {
                        SqlParameter PendingStatusParameter = new SqlParameter() { ParameterName = "PendingStatus", Value = 0 };
                        command.Parameters.Add(PendingStatusParameter);
                    }
                }

                SqlParameter PageNumberParameter = new SqlParameter() { ParameterName = "PageNumber", Value = pageIndx };
                command.Parameters.Add(PageNumberParameter);

                SqlParameter PageSizeParameter = new SqlParameter() { ParameterName = "PageSize", Value = pageSize };
                command.Parameters.Add(PageSizeParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                List<PromotionUser> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<PromotionUser>(reader).ToList();
                if (filteredData != null && filteredData.Count > 0)
                {
                    reader.NextResult();
                    totalCount = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                }

                dbContext.DatabaseInstance.Connection.Close();
                return filteredData;
            }
            catch (Exception exp)
            {
                dbContext.DatabaseInstance.Connection.Close();
                exception = exp.ToString();
                return null;
            }
        }

        public bool ApprovePromotionProgram(PromotionProgramApprovalActionModel model, string userId, out string exception)
        {
            exception = string.Empty;
            var promotionuser = _promotionUserRepository.Table.Where(x => x.Id == model.Id).FirstOrDefault();
            if (promotionuser == null)
                return false;

            try
            {
                //if (model.EnrolledType == "ByEmailAndNin")
                //    promotionuser.EmailVerified = true;

                //else if (model.EnrolledType == "ByAttachmentAndNin")
                //    promotionuser.NinVerified = true;

                if (model.EnrolledType == "ByEmailAndNin") // from Promotion Programs site
                {
                    promotionuser.EmailVerified = true;
                    promotionuser.NinVerified = false;
                }
                else if (model.EnrolledType == "ByAttachmentAndNin") // from Promotion Programs site
                {
                    promotionuser.EmailVerified = false;
                    promotionuser.NinVerified = true;
                }
                else // from profile
                {
                    promotionuser.EmailVerified = true;
                    promotionuser.NinVerified = false;
                    promotionuser.NationalId = null;
                }

                promotionuser.ModificationDate = DateTime.Now;
                promotionuser.ModifiedBy = userId;
                _promotionUserRepository.Update(promotionuser);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public bool DeletePromotionProgram(PromotionProgramApprovalActionModel model, string userId, out string exception)
        {
            exception = string.Empty;
            try
            {
                var promotionuser = _promotionUserRepository.Table.Where(x => x.Id == model.Id).FirstOrDefault();
                if (promotionuser == null)
                {
                    exception = "No record found in PromotionUser table with this id: " + model.Id;
                    return false;
                }

                promotionuser.IsDeleted = true;
                promotionuser.ModificationDate = DateTime.Now;
                promotionuser.ModifiedBy = userId;
                _promotionUserRepository.Update(promotionuser);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        public PromotionUser GetPromotionUserById(int id)
        {
            return _promotionUserRepository.TableNoTracking.Where(a => a.Id == id).FirstOrDefault();
        }
    }
}
