using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.PromotionPrograms;
using Tameenk.Core.Exceptions;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.Promotions;
using Tameenk.Security.Encryption;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Core.Promotions;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Profile.Component
{
    enum PromotionType
    {
        Email = 0,
        Nin = 1,
        Attacjment = 2
    }

    public class PromotionContext : IPromotionContext
    {
        private const string JOIN_PROMOTION_PROGRAM_SHARED_KEY = "TameenkJoinPromotionProgramSharedKey@$";
        private readonly IRepository<PromotionUser> _promotionProgramUserRepository;
        private readonly IRepository<PromotionProgramDomain> _promotionProgramDomainRepository;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IRepository<PromotionProgram> _promotionProgramRepository;
        private readonly IRepository<PromotionProgramNins> _promotionProgramNinsRepository;
        private readonly IRepository<PromotionProgramAttachments> _PromotionProgramAttachmentsNinsRepository;
        private readonly IPromotionService _promotionService;
        private readonly IRepository<PromotionProgramUser> _promotionUserRepository;
        private readonly INotificationService _notificationService;

        public PromotionContext(IRepository<PromotionUser> promotionProgramUserRepository, IRepository<PromotionProgramDomain> promotionProgramDomainRepository,
            TameenkConfig tameenkConfig, IRepository<PromotionProgram> promotionProgramRepository, IRepository<PromotionProgramNins> promotionProgramNinsRepository,
            IRepository<PromotionProgramAttachments> PromotionProgramAttachmentsNinsRepository, IPromotionService promotionService, IRepository<PromotionProgramUser> promotionUserRepository,
            INotificationService notificationService)
        {
            _promotionProgramUserRepository = promotionProgramUserRepository;
            _promotionProgramDomainRepository = promotionProgramDomainRepository;
            _tameenkConfig = tameenkConfig;
            _promotionProgramRepository = promotionProgramRepository;
            _promotionProgramNinsRepository = promotionProgramNinsRepository;
            _PromotionProgramAttachmentsNinsRepository = PromotionProgramAttachmentsNinsRepository;
            _promotionService = promotionService;
            _promotionUserRepository = promotionUserRepository;
            _notificationService = notificationService;
        }

        // in mail only
        public EnrollUserToProgramModel EnrollUserToProgramByEmail(JoinProgramModel model, string userId)
        {
            EnrollUserToProgramModel outPut = new EnrollUserToProgramModel();

            int programId;
            var validationError = ValidateBeforeJoinProgramByEmail(model.UserEmail, userId, model.ProgramId, out programId);
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                outPut.UserEndrollerd = false;
                outPut.Errors = new List<ErrorModel>() { new ErrorModel() { Description = validationError } };
                return outPut;
            }

            var enrollmentResponse = EnrollUSerToPromotionProgram(model.UserEmail, programId, userId, model.Nin, "By Email", model.Lang);
            //if user joined the program then send confirmation email 
            if (enrollmentResponse.UserEndrollerd)
            {
                var sendEmail = SendJoinProgramConfirmationEmailByUserIdOrNin(model.UserEmail, enrollmentResponse.Key, model.Lang, model.Channel);
                if (string.IsNullOrEmpty(sendEmail))
                    outPut.UserEndrollerd = true;
                else
                {
                    outPut.UserEndrollerd = false;
                    outPut.Errors = new List<ErrorModel>()
                    {
                        new ErrorModel()
                        {
                            Description = PromotionProgramResource.ResourceManager.GetString("SendingConfirmationEmail", CultureInfo.GetCultureInfo(model.Lang))
                        }
                    };
                }

                return outPut;
            }
            else
            {
                outPut.UserEndrollerd = false;
                outPut.Errors = new List<ErrorModel>() { new ErrorModel(){ Description = enrollmentResponse.Errors.FirstOrDefault().Description } };
                return outPut;
            }
        }

        // in mail only
        private string ValidateBeforeJoinProgramByEmail(string userEmail, string userId, int promotionProgId, out int programId)
        {
            programId = 0;
            MailAddress address = new MailAddress(userEmail);
            if (address != null && string.IsNullOrEmpty(address.Host))
                throw new TameenkArgumentNullException(nameof(userEmail), "address.Host can't be null or empty.");

            if (IsEmailAlreadyUsed(userEmail, userId))
                return PromotionProgramResource.EmailAlreadyUsedByAnotherUser;

            var programDomain = _promotionProgramDomainRepository.TableNoTracking.Where(e => e.Domian == address.Host && e.PromotionProgramId == promotionProgId).OrderByDescending(a => a.Id).FirstOrDefault();
            if (programDomain == null)
                return PromotionProgramResource.UserDomianDosntExistInProgramDomains;

            programId = programDomain.PromotionProgramId;
            return string.Empty;
        }

        // in main only
        private bool IsEmailAlreadyUsed(string email, string userId)
        {
            var programUSer = _promotionProgramUserRepository.TableNoTracking.FirstOrDefault(e => e.Email == email && e.UserId == userId && (e.IsDeleted == null || e.IsDeleted == false));
            return programUSer == null ? false : true;
        }

        // in mail only
        private EnrollUserToProgramModel EnrollUSerToPromotionProgram(string email, int promotionProgramId, string userId, string nin, string enrolledType, string lang)
        {
            var response = new EnrollUserToProgramModel();
            try
            {
                var promotionProgUser = _promotionProgramUserRepository.TableNoTracking.Where(e => e.Email == email && e.PromotionProgramId == promotionProgramId && (e.IsDeleted == null || e.IsDeleted == false)).FirstOrDefault();
                if (promotionProgUser == null)
                {
                    //then add new user
                    string _key = string.Empty;
                    var result = AddUserToPromotionProgram(email, promotionProgramId, userId, nin, enrolledType, out _key);
                    if (!string.IsNullOrEmpty(result))
                    {
                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>() { new ErrorModel(){ Description = result } };
                        return response;
                    }

                    response.UserEndrollerd = true;
                    response.Key = _key;
                    return response;
                }

                var program = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == promotionProgUser.PromotionProgramId).FirstOrDefault();
                //if  not verified yet then update the current record with the new data
                if (!promotionProgUser.EmailVerified)
                {
                    response.UserEndrollerd = true;
                    response.Key = promotionProgUser.Key.ToString();
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
                            Description = string.Format(PromotionProgramResource.ResourceManager.GetString("UserAlreasdyEnrolled", CultureInfo.GetCultureInfo(lang)), program.Name)
                        }
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.UserEndrollerd = false;
                response.Errors = new List<ErrorModel>() { new ErrorModel(){ Description = ex.ToString() } };
                return response;
            }
        }

        private string AddUserToPromotionProgram(string email, int programId, string userId, string nin, string enrolledType, out string _key)
        {
            Guid key = Guid.NewGuid();
            _key = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(nin))
                    return "Email and National Id can't be null or empty.";

                if (programId < 1)
                    return "Missing program identifier.";

                PromotionUser promotionProgramUser = new PromotionUser();
                promotionProgramUser.Email = email;
                promotionProgramUser.EmailVerified = false;
                promotionProgramUser.CreationDate = DateTime.Now;
                promotionProgramUser.UserId = userId;
                promotionProgramUser.NationalId = nin;
                promotionProgramUser.PromotionProgramId = programId;
                promotionProgramUser.EnrolledType = enrolledType;
                promotionProgramUser.Key = key;

                _promotionProgramUserRepository.Insert(promotionProgramUser);

                _key =key.ToString();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string UpdatePromotionProgramUser(PromotionUser entity)
        {
            try
            {
                if (entity == null)
                    throw new TameenkArgumentNullException(nameof(entity), "Entity to update can not be null.");

                _promotionProgramUserRepository.Update(entity);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string SendJoinProgramConfirmationEmailByUserIdOrNin(string userEmail, string key, string lang, string channel)
        {
            try
            {
                string hashed = SecurityUtilities.HashData(key, null);
                var emailSubject = PromotionProgramResource.JoinProgramConfirmationSubject;
                string url = Utilities.SiteURL + "/promotionPrograms/confirmJoinProgram/?key=" + key + "&hashed=" + hashed;
                string emailBody = string.Format(PromotionProgramResource.JoinProgramConfirmationBody, url);
                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/assets/imgs/PromoActivation.png";
                messageBodyModel.Language = lang;
                messageBodyModel.MessageBody = emailBody;

                EmailModel emailModel = new EmailModel();
                emailModel.To = new List<string>();
                emailModel.To.Add(userEmail);
                emailModel.Subject = emailSubject;
                emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                emailModel.Module = "Vehicle";
                emailModel.Method = "Promotions";
                emailModel.Channel = channel;
                var sendMail = _notificationService.SendEmail(emailModel);
                if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                {
                    return sendMail.ErrorDescription;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public EnrollUserToProgramModel ConfirmJoinProgram(Guid key, string userId, string lang)
        {
            var response = new EnrollUserToProgramModel();
            try
            {
                var programUser = _promotionProgramUserRepository.Table.Where(a => a.Key == key).FirstOrDefault();
                if (programUser == null)
                {
                    response.UserEndrollerd = false;
                    response.Errors = new List<ErrorModel>()
                    {
                        new ErrorModel() { Description = PromotionProgramResource.ResourceManager.GetString("ConfirmJoinProgram_EmailDoesNotHaveRequest", CultureInfo.GetCultureInfo(lang)) }
                    };
                    return response;
                }
                if (DateTime.Now.Subtract(programUser.CreationDate.Value).TotalMinutes > 15)
                {
                    response.UserEndrollerd = false;
                    response.Errors = new List<ErrorModel>()
                    {
                        new ErrorModel() { Description = PromotionProgramResource.ResourceManager.GetString("ConfirmJoinProgramRequestExpired", CultureInfo.GetCultureInfo(lang)) }
                    };
                    return response;
                }

                programUser.EmailVerified = true;
                _promotionProgramUserRepository.Update(programUser);

                response.UserEndrollerd = true;
                return response;
            }
            catch (Exception ex)
            {
                response.UserEndrollerd = false;
                response.Errors = new List<ErrorModel>() { new ErrorModel(){ Description = ex.ToString() } };
                return response;
            }
        }

        public List<IdNameModel> GetPromotionProgramsByTypeId(int typeId, string lang)
        {
            List<IdNameModel> programs = new List<IdNameModel>();
            var programsQuery = _promotionProgramRepository.TableNoTracking.Where(e => e.IsActive == true &&
                                (e.EffectiveDate == null || e.EffectiveDate <= DateTime.Now) &&
                                (e.DeactivatedDate == null || e.DeactivatedDate >= DateTime.Now));

            if (typeId == (int)PromotionType.Email)
                programsQuery = programsQuery.Where(e => e.IsPromoByEmail);

            else if (typeId == (int)PromotionType.Nin)
                programsQuery = programsQuery.Where(e => e.IsPromoByNin);

            else if (typeId == (int)PromotionType.Attacjment)
                programsQuery = programsQuery.Where(e => e.IsPromoByAttachment);

            foreach (var program in programsQuery)
            {
                var model = new IdNameModel();
                model.Id = program.Id;
                model.Name = (lang.ToLower() == "ar") ? program.Name : program.Name;

                programs.Add(model);
            }

            return programs;
        }

        public EnrollUserToProgramModel EnrollUserToProgramByNin(JoinProgramModel model, string userId)
        {
            EnrollUserToProgramModel outPut = new EnrollUserToProgramModel();

            var enrollmentResponse = EnrollUSerToPromotionProgramByNin(model.Nin, model.ProgramId, userId, model.Lang);
            //if user joined the program then send confirmation email 
            if (enrollmentResponse.UserEndrollerd)
            {
                outPut.UserEndrollerd = true;
                return outPut;
            }
            else
            {
                //show error that userd couldn't be added to this program.
                var errorMsg = string.Join(Environment.NewLine, enrollmentResponse.Errors.Select(e => e.Description));
                outPut.UserEndrollerd = false;
                outPut.Errors = new List<ErrorModel>()
                {
                    new ErrorModel(){ Description = enrollmentResponse.Errors.FirstOrDefault().Description }
                };

                return outPut;
            }
        }

        private EnrollUserToProgramModel EnrollUSerToPromotionProgramByNin(string nin, int promotionProgramId, string userId, string lang)
        {
            var response = new EnrollUserToProgramModel();
            try
            {
                var promotionProgram = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == promotionProgramId && a.IsActive == true).FirstOrDefault();
                if (promotionProgram == null)
                {
                    response.UserEndrollerd = false;
                    response.Errors = new List<ErrorModel>()
                    {
                        new ErrorModel() { Description = string.Format(PromotionProgramResource.ResourceManager.GetString("ProgramNotFound", CultureInfo.GetCultureInfo(lang)), promotionProgramId) }
                    };
                    return response;
                }
                else if (promotionProgram.EnableService.HasValue && promotionProgram.EnableService.Value)
                {
                    var result = _promotionService.SubmitServiceRequestForPromotionProgramsByNin(nin);
                    if (!string.IsNullOrEmpty(result))
                    {
                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>() { new ErrorModel() { Description = result } };
                        return response;
                    }

                    //then add new user
                    string _key = string.Empty;
                    var addResult = AddUserToPromotionProgram(null, promotionProgramId, userId, nin, "By Nin, and service is enabled", out _key);
                    if (!string.IsNullOrEmpty(addResult))
                    {
                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>() { new ErrorModel(){ Description = addResult } };
                        return response;
                    }

                    response.UserEndrollerd = true;
                    response.Key = _key;
                    return response;
                }
                else
                {
                    var promotionProgUser = _promotionProgramNinsRepository.TableNoTracking.Where(e => e.NationalId == nin && e.isDeleted == false).FirstOrDefault();
                    if (promotionProgUser == null)
                    {
                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>()
                        {
                            new ErrorModel() { Description = string.Format(PromotionProgramResource.ResourceManager.GetString("CanNotJoinProgramBuNin", CultureInfo.GetCultureInfo(lang)), promotionProgramId) }
                        };
                        return response;
                    }
                    else if (promotionProgUser.PromotionProgramId != promotionProgramId)
                    {
                        var program = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == promotionProgUser.PromotionProgramId).FirstOrDefault();

                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>()
                        {
                            new ErrorModel() { Description = string.Format(PromotionProgramResource.ResourceManager.GetString("NinAlreadyJoinedToProgram_error", CultureInfo.GetCultureInfo(lang)), program.Name) }
                        };
                        return response;
                    }
                    else
                    {
                        var promotionProgUser_Nin = _promotionProgramUserRepository.TableNoTracking.Where(e => e.NationalId == nin && e.IsDeleted == false).FirstOrDefault();
                        if (promotionProgUser_Nin == null)
                        {
                            //then add new user
                            string _key = string.Empty;
                            var result = AddUserToPromotionProgram(null, promotionProgramId, userId, nin, "By Nin", out _key);
                            if (!string.IsNullOrEmpty(result))
                            {
                                response.UserEndrollerd = false;
                                response.Errors = new List<ErrorModel>()
                                {
                                    new ErrorModel(){ Description = result }
                                };
                                return response;
                            }

                            response.UserEndrollerd = true;
                            response.Key = _key;
                            return response;
                        }
                        else
                        {
                            //return error message indicate that the user already joined in a promotioin program
                            var program = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == promotionProgUser.PromotionProgramId).FirstOrDefault();
                            response.UserEndrollerd = false;
                            response.Errors = new List<ErrorModel>()
                            {
                                new ErrorModel() { Description = string.Format(PromotionProgramResource.ResourceManager.GetString("NinAlreadyJoinedToProgram", CultureInfo.GetCultureInfo(lang)), program.Name) }
                            };
                            return response;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.UserEndrollerd = false;
                response.Errors = new List<ErrorModel>()
                {
                    new ErrorModel(){ Description = ex.ToString() }
                };
                return response;
            }
        }

        public EnrollUserToProgramModel EnrollUserToProgramByUploaAttachment(JoinProgramModel model, string userId)
        {
            var response = new EnrollUserToProgramModel();
            try
            {
                FileNetworkShare fileShare = new FileNetworkShare();

                string generatedReportDirPath = Utilities.GetAppSetting("JoinProgramByUploadAttachmentFilesBaseFolder");
                generatedReportDirPath = Path.Combine(generatedReportDirPath, model.ProgramName, DateTime.Now.Date.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Hour.ToString());
                string generatedReportFilePath = Path.Combine(generatedReportDirPath, model.FileName + "." + model.FileExt);

                var serverIp = _tameenkConfig.RemoteServerInfo.ServerIP;
                if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                {
                    string reportFilePath = generatedReportFilePath;
                    generatedReportFilePath = serverIp + "\\" + generatedReportFilePath;
                    generatedReportDirPath = serverIp + "\\" + generatedReportDirPath;

                    string exception = string.Empty;
                    if (fileShare.SaveFileToShare(_tameenkConfig.RemoteServerInfo.DomainName, _tameenkConfig.RemoteServerInfo.ServerUserName, _tameenkConfig.RemoteServerInfo.ServerPassword, generatedReportDirPath, generatedReportFilePath, model.FileBytes, serverIp, out exception))
                    {
                        response.UserEndrollerd = true;
                        response.Errors = new List<ErrorModel>()
                        {
                            new ErrorModel() {
                                Description = PromotionProgramResource.ResourceManager.GetString("Attachment_Success", CultureInfo.GetCultureInfo(model.Lang))
                            }
                        };
                        return response;
                    }
                    else
                    {
                        response.UserEndrollerd = false;
                        response.Errors = new List<ErrorModel>()
                        {
                            new ErrorModel() {
                                //Description = "Error happend while save file to shared server, and the error is: "  + exception
                                Description = string.Format(PromotionProgramResource.ResourceManager.GetString("Attachment_serviceException", CultureInfo.GetCultureInfo(model.Lang)), exception)
                            }
                        };
                        return response;
                    }
                }
                else
                {
                    if (!Directory.Exists(generatedReportDirPath))
                        Directory.CreateDirectory(generatedReportDirPath);

                    File.WriteAllBytes(generatedReportFilePath, model.FileBytes);
                }

                var promotionProgramAttachmentModel = new PromotionProgramAttachments() {
                    CreatedDate = DateTime.Now,
                    ProgramId = model.ProgramId,
                    UserId = userId,
                    Approved = false,
                    FilePath = generatedReportFilePath
                };
                _PromotionProgramAttachmentsNinsRepository.Insert(promotionProgramAttachmentModel);

                response.UserEndrollerd = true;
                return response;
            }
            catch (Exception ex)
            {
                response.UserEndrollerd = false;
                response.Errors = new List<ErrorModel>()
                {
                    new ErrorModel(){ Description = ex.ToString() }
                };
                return response;
            }
        }

        public PromotionProgramEnrolledModel GetEnrolledPromotionProgram(CheckPromotionProgramEnrolledModel model, out string exception)
        {
            exception = string.Empty;
            PromotionProgramEnrolledModel enrolledProgram = null;

            try
            {
                var userProgram = _promotionProgramNinsRepository.TableNoTracking.Include(x => x.PromotionProgram).Where(a => a.NationalId == model.Nin && !a.isDeleted).FirstOrDefault();
                if (userProgram != null)
                {
                    enrolledProgram = new PromotionProgramEnrolledModel() {
                        ProgramId = userProgram.PromotionProgramId,
                        ProgramNameAr = userProgram.PromotionProgram.Name,
                        ProgramNameEn = userProgram.PromotionProgram.Name
                    };
                }

                return enrolledProgram;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return enrolledProgram;
            }
        }
        public PromotionOutput JoinProgramByEmail(JoinProgramModel model, string userName, string userId)
        {
            PromotionOutput output = new PromotionOutput();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserName = userName;
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = userId;
            log.MethodName = "JoinProgramByEmail";
            try
            {
                if (model == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ModelIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (model.ProgramId < 1)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ProgramIdInvalid", CultureInfo.GetCultureInfo(model.Lang)); //"Program Id can't be less than 1";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.ProgramId is not valid";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.UserEmail))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmailEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.UserEmail is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (!Utilities.IsValidMail(model.UserEmail))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidEmail;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Email_Format_Erorr", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.UserEmail is not valid as we recived " + model.UserEmail;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                MailAddress address = new MailAddress(model.UserEmail);
                if (address != null && string.IsNullOrEmpty(address.Host))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidEmail;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Email_Format_Erorr", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "address.Host can't be null or empty " + model.UserEmail;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                var programUSer = _promotionProgramUserRepository.TableNoTracking.FirstOrDefault(e => e.Email == model.UserEmail && e.UserId == userId&&e.EmailVerified==true && (e.IsDeleted == null || e.IsDeleted == false));
                if (programUSer != null)
                {
                    var programInfo = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == programUSer.PromotionProgramId).FirstOrDefault();

                    output.ErrorCode = PromotionOutput.ErrorCodes.UserAlreasdyEnrolled;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("UserAlreasdyEnrolled", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", programInfo.Name); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "address.Host can't be null or empty " + model.UserEmail;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                var programDomain = _promotionProgramDomainRepository.TableNoTracking.Where(e => e.Domian == address.Host && e.PromotionProgramId == model.ProgramId).OrderByDescending(a => a.Id).FirstOrDefault();
                if (programDomain == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ProgramDomainIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("UserDomianDosntExistInProgramDomains", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Program Domain Is Null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                PromotionUser promotionProgramUser = new PromotionUser();
                promotionProgramUser.Email = model.UserEmail;
                promotionProgramUser.EmailVerified = false;
                promotionProgramUser.CreationDate = DateTime.Now;
                promotionProgramUser.ModificationDate = DateTime.Now;
                promotionProgramUser.UserId = userId;
                //promotionProgramUser.NationalId = nin;
                promotionProgramUser.PromotionProgramId = programDomain.PromotionProgramId;
                promotionProgramUser.EnrolledType = "ByEmail";
                promotionProgramUser.Key = Guid.NewGuid();
                promotionProgramUser.IsDeleted = false;
                _promotionProgramUserRepository.Insert(promotionProgramUser);

                string hashed = SecurityUtilities.HashData(promotionProgramUser.Key.ToString(), null);
                var emailSubject = PromotionProgramResource.JoinProgramConfirmationSubject;
                string url = Utilities.SiteURL + "/promotionPrograms/confirmJoinProgram/?key=" + promotionProgramUser.Key + "&hashed=" + hashed + "&jt=" + (int)PromotionProgramTypeEnum.Email;
                string emailBody = string.Format(PromotionProgramResource.JoinProgramConfirmationBody, url);
                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/assets/imgs/PromoActivation.png";
                messageBodyModel.Language = model.Lang;
                messageBodyModel.MessageBody = emailBody;

                EmailModel emailModel = new EmailModel();
                emailModel.To = new List<string>();
                emailModel.To.Add(model.UserEmail);
                emailModel.Subject = emailSubject;
                emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                emailModel.Module = "Vehicle";
                emailModel.Method = "Promotions";
                emailModel.Channel = model.Channel;
                var sendMail = _notificationService.SendEmail(emailModel);
                if (sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.FailedToSendEamil;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("SendingConfirmationEmail", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to send email " + sendMail.ErrorDescription;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("VerificationMailAlreadySent", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}",model.UserEmail);
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
        }
        public PromotionOutput JoinProgramByNin(JoinProgramModel model, string userName, string userId)
        {
            PromotionOutput output = new PromotionOutput();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserName = userName;
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = userId;
            log.MethodName = "JoinProgramByNin";
            try
            {
                if (model == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ModelIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (model.ProgramId < 1)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ProgramIdInvalid", CultureInfo.GetCultureInfo(model.Lang)); //"Program Id can't be less than 1";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.ProgramId is not valid";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Nin))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.Nin is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                
                var programUSer = _promotionProgramUserRepository.TableNoTracking.FirstOrDefault(e => e.Email == model.UserEmail && e.UserId == userId &&e.NinVerified==true && (e.IsDeleted == null || e.IsDeleted == false));
                if (programUSer != null)
                {
                    var programInfo = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == programUSer.PromotionProgramId).FirstOrDefault();
                    output.ErrorCode = PromotionOutput.ErrorCodes.UserAlreasdyEnrolled;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("UserAlreasdyEnrolled", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", programInfo.Name); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "address.Host can't be null or empty " + model.UserEmail;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                var program = _promotionProgramRepository.TableNoTracking.Where(e => e.Id == model.ProgramId&&e.IsActive==true).OrderByDescending(a => a.Id).FirstOrDefault();
                if (program == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ProgramDomainIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ProgramNotFound", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Program Domain Is Null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (program.EnableService.HasValue && program.EnableService.Value)
                {
                    var result = _promotionService.SubmitServiceRequestForPromotionProgramsByNin(model.Nin);
                    if (!string.IsNullOrEmpty(result))
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.ProgramDomainIsNull;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("CanNotJoinProgramBuNin", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Failed due to "+ result;
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                }
                else
                {
                    var promotionProgUser = _promotionProgramNinsRepository.TableNoTracking.Where(e => e.NationalId == model.Nin && e.isDeleted == false).FirstOrDefault();
                    if (promotionProgUser == null)
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.ProgramDomainIsNull;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("CanNotJoinProgramBuNin", CultureInfo.GetCultureInfo(model.Lang)); 
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "promotionProgUser is null ";
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                    var programInfo = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == promotionProgUser.PromotionProgramId).FirstOrDefault();
                    if (promotionProgUser.PromotionProgramId != model.ProgramId)
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.ProgramDomainIsNull;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NinAlreadyJoinedToProgram_error", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", programInfo.Name);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "promotionProgUser.PromotionProgramId and model.ProgramId not equal";
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                       
                    }
                    var promotionProgUser_Nin = _promotionProgramUserRepository.TableNoTracking.Where(e => e.NationalId == model.Nin && e.IsDeleted == false).FirstOrDefault();
                    if (promotionProgUser_Nin != null)
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.ProgramDomainIsNull;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NinAlreadyJoinedToProgram_error", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", programInfo.Name);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "user already subscribed on another offer "+ programInfo.Name;
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                }
                PromotionUser promotionProgramUser = new PromotionUser();
                //promotionProgramUser.Email = model.UserEmail;
                promotionProgramUser.EmailVerified = false;
                promotionProgramUser.CreationDate = DateTime.Now;
                promotionProgramUser.UserId = userId;
                promotionProgramUser.NationalId = model.Nin;
                promotionProgramUser.PromotionProgramId = program.Id;
                promotionProgramUser.EnrolledType = "ByNin";
                promotionProgramUser.Key = Guid.NewGuid();
                _promotionProgramUserRepository.Insert(promotionProgramUser);
                
                output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("JoinProgramIsConfirmed", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", model.UserEmail); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
        }
        public PromotionOutput ConfirmJoinProgram(ConfirmPromotionModel model,string userName,string userId)
        {
            PromotionOutput output = new PromotionOutput();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserName = userName;
            log.MethodName = "ConfirmJoinProgram";
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = userId;
            try
            {
                if (string.IsNullOrWhiteSpace(model.Key))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "key value is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrWhiteSpace(model.Hashed))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "hashed value is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (!SecurityUtilities.VerifyHashedData(model.Hashed.Trim(), model.Key.Trim()))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidHashing;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "hash not matched Key: { " + model.Key + " } and hashed: " + model.Hashed;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                Guid key = Guid.Empty;
                if (!Guid.TryParse(model.Key, out key))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidKeyFormat;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "failed to parse key:"+model.Key;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                var programUser = _promotionProgramUserRepository.Table.Where(a => a.Key == key).FirstOrDefault();
                if (programUser == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidKeyFormat;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ConfirmJoinProgram_EmailDoesNotHaveRequest", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "programUser is null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (DateTime.Now.Subtract(programUser.ModificationDate.Value).TotalMinutes > 15)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidKeyFormat;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ConfirmJoinProgramRequestExpired", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Expired request";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                if (model.JoinTypeId == (int)PromotionProgramTypeEnum.Email)
                    programUser.EmailVerified = true;
                else if (model.JoinTypeId == (int)PromotionProgramTypeEnum.EmailAndNin)
                    programUser.NinVerified = true;

                programUser.ModifiedBy = userId;
                programUser.ModificationDate = DateTime.Now;
                _promotionProgramUserRepository.Update(programUser);

                output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("JoinProgramIsConfirmed", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
        }

        public PromotionOutput CheckUserEnrolled(CheckPromotionProgramEnrolledModel model, string userName, string userId)
        {
            PromotionOutput output = new PromotionOutput();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserName = userName;
            log.MethodName = "GetEnrolledPromotionProgram";
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = userId;

            try
            {
                if (model == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ModelIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Nin))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "nin is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                var userProgram = _promotionProgramUserRepository.TableNoTracking.Where(a => a.NationalId == model.Nin && a.NinVerified == true && (a.IsDeleted == null || a.IsDeleted == false)).OrderByDescending(a => a.Id).FirstOrDefault();
                if (userProgram == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.NullResult;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "This national id does not enrolled in any promotion program";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                var promotionProgram = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == userProgram.PromotionProgramId).FirstOrDefault();
                if (promotionProgram == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.NullResult;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "No program with this id " + userProgram.PromotionProgramId;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                output.Data = new ProgramProgramDataModel();
                output.Data.PromotionUserRequestId = userProgram.Id;
                output.Data.ProgramId = promotionProgram.Id;
                output.Data.ProgramName = (model.Lang == "en") ? promotionProgram.Name : promotionProgram.Name;

                output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "success";
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
        }

        public PromotionOutput JoinProgramByEmailAndNin(JoinProgramModel model, string userName, string userId)
        {
            PromotionOutput output = new PromotionOutput();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserName = userName;
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = userId;
            log.MethodName = "JoinProgramByEmailAndNin";
            string exception = string.Empty;

            try
            {
                if (model == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ModelIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.Nin))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NationalIdEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.Nin is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.UserEmail))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("EmailEmpty", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.UserEmail is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (!Utilities.IsValidMail(model.UserEmail))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidEmail;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Email_Format_Erorr", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.UserEmail is not valid as we recived " + model.UserEmail;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                MailAddress address = new MailAddress(model.UserEmail);
                if (address != null && string.IsNullOrEmpty(address.Host))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidEmail;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Email_Format_Erorr", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "address.Host can't be null or empty " + model.UserEmail;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                PromotionUser programUSer = _promotionProgramUserRepository.Table.Where(e => e.Email == model.UserEmail && (e.IsDeleted == null || e.IsDeleted == false)).OrderByDescending(a => a.Id).FirstOrDefault();
                if (programUSer != null)
                {
                    // this is an old user from old table (PromotionProgramUser) && user is verivied
                    if (string.IsNullOrEmpty(programUSer.Key.ToString()) && programUSer.EnrolledType == "ByEmail" && programUSer.EmailVerified)
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.UserAlreasdyEnrolled;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("emailJoinedFromProfile", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "email is already joined to an program from old system";
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                    // new & profile & verified
                    else if (!string.IsNullOrEmpty(programUSer.Key.ToString()) && programUSer.EnrolledType == "ByEmail" && programUSer.EmailVerified)
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.UserAlreasdyEnrolled;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("emailJoinedFromProfile", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "email is already joined to an program from profile new system";
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                    // new & profile & not verified --> send another email
                    else if(!string.IsNullOrEmpty(programUSer.Key.ToString()) && programUSer.EnrolledType == "ByEmail" && !programUSer.EmailVerified)
                    {
                        var sendAnotherMail = SendPromotionProgramEmail(programUSer, model, (int)PromotionProgramTypeEnum.Email, out exception);
                        if (!string.IsNullOrEmpty(exception) || sendAnotherMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                        {
                            output.ErrorCode = PromotionOutput.ErrorCodes.FailedToSendEamil;
                            output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("SendingConfirmationEmail", CultureInfo.GetCultureInfo(model.Lang));
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Failed to send email " + sendAnotherMail.ErrorDescription;
                            PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                            return output;
                        }

                        output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("VerificationMailAlreadySent", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", model.UserEmail); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Success";
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                    // new & site & nin verified
                    else if (!string.IsNullOrEmpty(programUSer.Key.ToString()) && programUSer.EnrolledType == "ByEmailAndNin" && programUSer.NinVerified)
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.UserAlreasdyEnrolled;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NinAlreadyJoinedToProgram", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "email and nin is already joined to an program from profile new system";
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                    // new & site & nin not verified --> send another email
                    else if (!string.IsNullOrEmpty(programUSer.Key.ToString()) && programUSer.EnrolledType == "ByEmailAndNin" && !programUSer.NinVerified)
                    {
                        var sendAnotherMail = SendPromotionProgramEmail(programUSer, model, (int)PromotionProgramTypeEnum.EmailAndNin, out exception);
                        if (!string.IsNullOrEmpty(exception) || sendAnotherMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                        {
                            output.ErrorCode = PromotionOutput.ErrorCodes.FailedToSendEamil;
                            output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("SendingConfirmationEmail", CultureInfo.GetCultureInfo(model.Lang));
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = "Failed to send email " + sendAnotherMail.ErrorDescription;
                            PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                            return output;
                        }

                        output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("VerificationMailAlreadySent", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", model.UserEmail); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "Success";
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                    // new & site & newNin != oldNin
                    else if (!string.IsNullOrEmpty(programUSer.NationalId) && (model.Nin == programUSer.NationalId || model.Nin != programUSer.NationalId) && programUSer.NinVerified)
                    {
                        string message = PromotionProgramResource.ResourceManager.GetString("emailJoinedWithAnotherNin", CultureInfo.GetCultureInfo(model.Lang));
                        string errorDescription = "email is assigned to another nin, and the nin is " + programUSer.NationalId;
                        if (model.Nin == programUSer.NationalId)
                        {
                            message = PromotionProgramResource.ResourceManager.GetString("NinAlreadyJoinedToProgram", CultureInfo.GetCultureInfo(model.Lang));
                            errorDescription = "email and nin already joined before";
                        }

                        output.ErrorCode = PromotionOutput.ErrorCodes.UserAlreasdyEnrolled;
                        output.ErrorDescription = message;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = errorDescription;
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }
                }

                PromotionProgramDomain programDomain = null;
                if (model.ProgramId > 0)
                    programDomain = _promotionProgramDomainRepository.TableNoTracking.Where(e => e.Domian == address.Host && e.PromotionProgramId == model.ProgramId).OrderByDescending(a => a.Id).FirstOrDefault();
                else
                    programDomain = _promotionProgramDomainRepository.TableNoTracking.Where(e => e.Domian == address.Host).OrderByDescending(a => a.Id).FirstOrDefault();

                if (programDomain == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ProgramDomainIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("UserDomianDosntExistInProgramDomains", CultureInfo.GetCultureInfo(model.Lang)); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Program Domain Is Null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                programUSer = new PromotionUser();
                programUSer.Email = model.UserEmail;
                programUSer.CreationDate = DateTime.Now;
                programUSer.ModificationDate = DateTime.Now;
                programUSer.UserId = userId;
                programUSer.NationalId = model.Nin;
                programUSer.PromotionProgramId = programDomain.PromotionProgramId;
                programUSer.EnrolledType = "ByEmailAndNin";
                programUSer.Key = Guid.NewGuid();
                programUSer.IsDeleted = false;
                _promotionProgramUserRepository.Insert(programUSer);

                var sendMail = SendPromotionProgramEmail(programUSer, model, (int)PromotionProgramTypeEnum.EmailAndNin, out exception);
                if (!string.IsNullOrEmpty(exception) || sendMail.ErrorCode != EmailOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.FailedToSendEamil;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("SendingConfirmationEmail", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed to send email " + sendMail.ErrorDescription;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("VerificationMailAlreadySent", CultureInfo.GetCultureInfo(model.Lang)).Replace("{0}", model.UserEmail); //"Email is't in a valid format as we receive this: " + model.UserEmail;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
        }

        public PromotionOutput JoinProgramByAttachment(JoinProgramModel model, HttpPostedFileBase file, string userName, string userId)
        {
            PromotionOutput output = new PromotionOutput();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserName = userName;
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = userId;
            log.MethodName = "JoinProgramByAttachment";

            try
            {
                if (model == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ModelIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is null";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (file == null || file.ContentLength <= 0)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("emptyFile", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "file is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (model.ProgramId < 1)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ProgramIdInvalid", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model.ProgramId is not valid";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                var promotionProgramData = _promotionProgramRepository.TableNoTracking.Where(a => a.Id == model.ProgramId).FirstOrDefault();
                if (promotionProgramData == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ProgramIdInvalid", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "no program found with this id " + model.ProgramId;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                int maxSize = 5; //MegaByte
                List<string> validFileExtensions = new List<string>() { ".jpeg", ".jpg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp", ".jfif" };
                var extension = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(file.ContentType) || (!string.IsNullOrEmpty(file.ContentType) && !validFileExtensions.Contains(extension)))
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.InvalidFileExtension;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("invalidFileExtension", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "content type is empty or file extension in not valid since uploaded extension is " + extension;
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (file.ContentLength > maxSize * 1048576) // 1048576 = 1024 * 1024
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.FileSizeLimitExceeded;
                    output.ErrorDescription = string.Format(PromotionProgramResource.ResourceManager.GetString("fileSizeLimitExceeded", CultureInfo.GetCultureInfo(model.Lang)), maxSize);
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = string.Format("File size limit exceeded, since uploaded file size is {0} bytes ", file.ContentLength);
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                var bytes = GetFileByte(file);
                var fileName = Path.GetFileName(file.FileName);
                FileNetworkShare fileShare = new FileNetworkShare();

                var currentDate = DateTime.Now;
                string offerSheetDirPath = Path.Combine(Utilities.GetAppSetting("JoinProgramByUploadAttachmentFilesBaseFolder"), promotionProgramData.Key, currentDate.Year.ToString(), currentDate.Month.ToString(), currentDate.Day.ToString());
                string generatedSheetFilePath = Path.Combine(offerSheetDirPath, fileName);

                if (_tameenkConfig.RemoteServerInfo.UseNetworkDownload)
                {
                    var domain = _tameenkConfig.RemoteServerInfo.DomainName;
                    var serverIP = _tameenkConfig.RemoteServerInfo.ServerIP;
                    var username = _tameenkConfig.RemoteServerInfo.ServerUserName;
                    var password = _tameenkConfig.RemoteServerInfo.ServerPassword;

                    //string reportFilePath = generatedSheetFilePath;
                    //generatedSheetFilePath = serverIP + "\\" + generatedSheetFilePath;
                    //offerSheetDirPath = serverIP + "\\" + offerSheetDirPath;

                    string exception = string.Empty;
                    if (!fileShare.UploadFileToShare(domain, username, password, offerSheetDirPath, generatedSheetFilePath, bytes, serverIP, out exception))
                    {
                        output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                        output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Attachment_serviceException", CultureInfo.GetCultureInfo(model.Lang));
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = "error occcured while uploading file to remote server due to:" + exception;
                        PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                        return output;
                    }

                    //output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                    //output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Attachment_Success", CultureInfo.GetCultureInfo(model.Lang));
                    //log.ErrorCode = (int)output.ErrorCode;
                    //log.ErrorDescription = "Success";
                    //PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    //return output;
                }
                else
                {
                    if (!Directory.Exists(offerSheetDirPath))
                        Directory.CreateDirectory(offerSheetDirPath);
                    File.WriteAllBytes(generatedSheetFilePath, bytes);
                }

                PromotionUser programUSer = _promotionProgramUserRepository.Table.Where(a => a.NationalId == model.Nin && a.PromotionProgramId == model.ProgramId && a.NinVerified == false && a.EnrolledType == "ByAttachmentAndNin" && (a.IsDeleted == null || a.IsDeleted == false)).OrderByDescending(a => a.Id).FirstOrDefault();
                if (programUSer == null)
                {
                    programUSer = new PromotionUser();
                    programUSer.Email = model.UserEmail;
                    programUSer.NationalId = model.Nin;
                    programUSer.PromotionProgramId = model.ProgramId;
                    programUSer.EnrolledType = "ByAttachmentAndNin";
                    programUSer.AttachmentPath = generatedSheetFilePath;
                    programUSer.UserId = userId;
                    programUSer.CreationDate = DateTime.Now;
                    programUSer.ModificationDate = DateTime.Now;
                    programUSer.NinVerified = false;
                    programUSer.IsDeleted = false;
                    _promotionProgramUserRepository.Insert(programUSer);
                }
                else
                {
                    programUSer.ModificationDate = DateTime.Now;
                    programUSer.AttachmentPath = generatedSheetFilePath;
                    _promotionProgramUserRepository.Update(programUSer);
                }

                output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Attachment_Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Success";
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
        }

        byte[] GetFileByte(HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

        public PromotionOutput ExitPromotionProgram(PromotionProgramApprovalActionModel model, string userName, string userId)
        {
            PromotionOutput output = new PromotionOutput();
            PromotionRequestLog log = new PromotionRequestLog();
            log.CreatedDate = DateTime.Now;
            log.UserName = userName;
            log.MethodName = "ExitPromotionProgram";
            log.ApiURL = Utilities.GetCurrentURL;
            log.Channel = model.Channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserIP = Utilities.GetUserIPAddress();
            log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            log.UserID = userId;

            try
            {
                if (model == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.ModelIsNull;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "model is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }
                if (model.Id < 1)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ProgramIdInvalid", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "nin is empty";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                var userProgram = _promotionProgramUserRepository.Table.Where(a => a.Id == model.Id).FirstOrDefault();
                if (userProgram == null)
                {
                    output.ErrorCode = PromotionOutput.ErrorCodes.NullResult;
                    output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("NullResult", CultureInfo.GetCultureInfo(model.Lang));
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "This national id does not enrolled in any promotion program";
                    PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                    return output;
                }

                userProgram.IsDeleted = true;
                userProgram.ModificationDate = DateTime.Now;
                userProgram.ModifiedBy = userId;
                _promotionProgramUserRepository.Update(userProgram);

                output.ErrorCode = PromotionOutput.ErrorCodes.Success;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("Success", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "success";
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = PromotionOutput.ErrorCodes.ServiceException;
                output.ErrorDescription = PromotionProgramResource.ResourceManager.GetString("ErrorGeneric", CultureInfo.GetCultureInfo(model.Lang));
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                PromotionRequestLogDataAccess.AddPromotionRequestsLog(log);
                return output;
            }
        }

        private EmailOutput SendPromotionProgramEmail(PromotionUser programUSer, JoinProgramModel model, int joinType, out string exception)
        {
            exception = string.Empty;
            try
            {
                string hashed = SecurityUtilities.HashData(programUSer.Key.ToString(), null);
                var emailSubject = PromotionProgramResource.JoinProgramConfirmationSubject;
                string url = Utilities.SiteURL + "/promotionPrograms/confirmJoinProgram/?key=" + programUSer.Key + "&hashed=" + hashed + "&jt=" + joinType;
                string emailBody = string.Format(PromotionProgramResource.JoinProgramConfirmationBody, url);
                MessageBodyModel messageBodyModel = new MessageBodyModel();
                messageBodyModel.Image = Utilities.SiteURL + "/assets/imgs/PromoActivation.png";
                messageBodyModel.Language = model.Lang;
                messageBodyModel.MessageBody = emailBody;

                EmailModel emailModel = new EmailModel();
                emailModel.To = new List<string>();
                emailModel.To.Add(model.UserEmail);
                emailModel.Subject = emailSubject;
                emailModel.EmailBody = MailUtilities.PrepareMessageBody(Strings.MailContainer, messageBodyModel);
                emailModel.Module = "Vehicle";
                emailModel.Method = "Promotions";
                emailModel.Channel = model.Channel;
                return _notificationService.SendEmail(emailModel);
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }
    }
}
