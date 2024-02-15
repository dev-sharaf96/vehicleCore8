using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Configuration;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Resources;
using Tameenk.Resources.UserTicket;
using Tameenk.Security.Services;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Implementation;
using Tameenk.Services.Notifications;
using Tameenk.Services.UserTicket.Components.Output;
using TameenkDAL.Models;

namespace Tameenk.Services.UserTicket.Components
{
    public class UserTicketContext : IUserTicketContext
    {
        private INotificationService _notificationService;
        private readonly IRepository<Policy> _policyRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<CheckoutDetail> _checkoutDetailsRepository;
        private readonly IRepository<Tameenk.Core.Domain.Entities.UserTicket> _userTicketRepository;
        private readonly IRepository<UserTicketType> _userTicketTypeRepository;
        private readonly IRepository<UserTicketAttachment> _userTicketAttachmentRepository;
        private readonly IRepository<Vehicle> _vehicleRepository;
        private readonly TameenkConfig _tameenkConfig;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRepository<UserTicketHistory> _userTicketHistoryRepository;
        private readonly INotificationServiceProvider _notification;

        private readonly List<int> DashboardTicketTypesNoPolicyRequiredToOpenTicket = new List<int>()
        {
            (int)EUserTicketTypes.NationalAddress,
            (int)EUserTicketTypes.ProofIDAndIBANAndEmail,
            (int)EUserTicketTypes.Others
        };

        public UserTicketContext(INotificationService notificationService,
            IRepository<Policy> policyRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<CheckoutDetail> checkoutDetailsRepository,
            IRepository<Tameenk.Core.Domain.Entities.UserTicket> userTicketRepository,
            IRepository<UserTicketType> userTicketTypeRepository,
            IRepository<UserTicketAttachment> userTicketAttachmentRepository,
            IRepository<Vehicle> vehicleRepository,
            TameenkConfig tameenkConfig,
            IAuthorizationService authorizationService,
            IRepository<UserTicketHistory> userTicketHistoryRepository, INotificationServiceProvider notification)
        {
            _notificationService = notificationService;
            _policyRepository = policyRepository;
            _invoiceRepository = invoiceRepository;
            _checkoutDetailsRepository = checkoutDetailsRepository;
            _userTicketRepository = userTicketRepository;
            _userTicketTypeRepository = userTicketTypeRepository;
            _userTicketAttachmentRepository = userTicketAttachmentRepository;
            _vehicleRepository = vehicleRepository;
            _tameenkConfig = tameenkConfig;
            _authorizationService = authorizationService;
            _userTicketHistoryRepository = userTicketHistoryRepository;
            _notification = notification;
        }

        public UserTicketOutput CreateUserTicket(int userTicketType, string extraData, string sequenceOrCustomCardNumber, string userNotes, string userId, string channel, LanguageTwoLetterIsoCode language, List<HttpPostedFileBase> postedFiles, List<AttachedFiles> attachedFiles, string nin, string createdBy = null)
        {
            if (!string.IsNullOrEmpty(sequenceOrCustomCardNumber))
            {
                sequenceOrCustomCardNumber = sequenceOrCustomCardNumber.Trim();
            }

            if (!string.IsNullOrEmpty(userNotes))
            {
                userNotes = userNotes.Trim();
            }

            UserTicketOutput output = new UserTicketOutput();
            UserTicketLog log = new UserTicketLog();
            try
            {
                log.MethodName = "CreateUserTicket";
                log.UserIP = Utilities.GetUserIPAddress();
                log.ServerIP = Utilities.GetInternalServerIP();
                log.Headers["User-Agent"].ToString() = Utilities.GetUserAgent();
                log.CreatedDate = DateTime.Now;
                log.UserId = userId;
                log.Channel = channel;

                if (userTicketType <= 0)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidTicketType;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"userTicketType not valid: {userTicketType}";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (userTicketType == (int)EUserTicketTypes.LinkWithNajm || userTicketType == (int)EUserTicketTypes.ChangePolicyData
                    || userTicketType == (int)EUserTicketTypes.CouldnotPrintPolicy || userTicketType == (int)EUserTicketTypes.PolicyGeneration
                    || userTicketType == (int)EUserTicketTypes.UpdateCustomToSequence || userTicketType == (int)EUserTicketTypes.ReachMaximumPolicyIssuance || userTicketType == (int)EUserTicketTypes.Others)
                {
                    if (string.IsNullOrEmpty(extraData) && string.IsNullOrEmpty(sequenceOrCustomCardNumber))
                    {
                        output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                        output.ErrorDescription = UserTicketResources.InvalidExtraData;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"extraData and sequenceOrCustomCardNumber are null, while userTicketType is: {userTicketType}";
                        UserTicketLogDataAccess.AddUserTicketLog(log);
                        return output;
                    }
                }

                if ((userTicketType != (int)EUserTicketTypes.LinkWithNajm && userTicketType != (int)EUserTicketTypes.CouldnotPrintPolicy
                     && userTicketType != (int)EUserTicketTypes.UpdateCustomToSequence && userTicketType != (int)EUserTicketTypes.ProofIDAndIBANAndEmail 
                     && userTicketType != (int)EUserTicketTypes.ReachMaximumPolicyIssuance)
                    && string.IsNullOrEmpty(userNotes))
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidUserNotes;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"userNotes is empty";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if ((userTicketType == (int)EUserTicketTypes.NationalAddress || userTicketType == (int)EUserTicketTypes.ProofIDAndIBANAndEmail || userTicketType == (int)EUserTicketTypes.ReachMaximumPolicyIssuance)
                    && string.IsNullOrEmpty(nin))
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidNIN;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"nin is empty";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (string.IsNullOrEmpty(userId))
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = $"{UserTicketResources.InvalidUserId}, as userId is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"userId is empty";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (string.IsNullOrEmpty(channel))
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidChannel;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"channel is empty";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (postedFiles != null && postedFiles.Count > 5)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"postedFiles are {postedFiles.Count} which greater than 5";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (attachedFiles != null && attachedFiles.Count > 5)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"attachedFiles are {attachedFiles.Count} which greater than 5";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (userTicketType == (int)EUserTicketTypes.ChangePolicyData && (attachedFiles.Count == 0 || attachedFiles.Count > 3))
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"attachedFiles are {attachedFiles.Count} which greater than 3 or less than 1";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }
                if ((userTicketType == (int)EUserTicketTypes.CouldnotPrintPolicy || userTicketType == (int)EUserTicketTypes.PolicyGeneration 
                        || userTicketType == (int)EUserTicketTypes.NationalAddress || userTicketType == (int)EUserTicketTypes.Others) 
                    && attachedFiles.Count > 2)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"attachedFiles are {attachedFiles.Count} which greater than 2";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }
                if ((userTicketType == (int)EUserTicketTypes.ReachMaximumPolicyIssuance || userTicketType == (int)EUserTicketTypes.UpdateCustomToSequence) && attachedFiles.Count != 1)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"attachedFiles are {attachedFiles.Count}, and only 1 is required";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                var user = _authorizationService.GetUserInfoByEmail(null, userId, null);
                if (user == null)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = $"{UserTicketResources.InvalidUserId}, as userId:{userId} doesnot correspond to any user";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"userId:{userId} doesnot correspond to any user";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                var validateBeforeOpenTicketOutput = ValidateBeforeOpenTicket(userTicketType, sequenceOrCustomCardNumber, nin);
                if (validateBeforeOpenTicketOutput.ErrorCode != ValidateUserTicketOutput.ErrorCodes.Valid)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.NotValidToOpenNewTicket;
                    output.ErrorDescription = validateBeforeOpenTicketOutput.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"ErrorDescription: {validateBeforeOpenTicketOutput.ErrorDescription}, userTicketType:{userTicketType}, userId:{userId}, sequenceOrCustomCardNumber:{sequenceOrCustomCardNumber}, nin:{nin}, {output.ErrorDescription}";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                UserTicketModel userTicketModel = new UserTicketModel();
                userTicketModel.CreatedDate = log.CreatedDate;
                userTicketModel.TicketTypeId = userTicketType;
                userTicketModel.UserId = userId;
                userTicketModel.UserNotes = userNotes;
                userTicketModel.CurrentTicketStatusId = (int)EUserTicketStatus.TicketOpened;
                userTicketModel.Channel = channel;

                userTicketModel.UserEmail = user.Email;
                if (string.IsNullOrEmpty(createdBy))
                {
                    userTicketModel.CreatedBy = user.Email;
                }
                else
                {
                    userTicketModel.CreatedBy = createdBy;
                }
                userTicketModel.UserPhone = user.PhoneNumber;
                string phoneToSendSMS = user.PhoneNumber;

                ////
                /// exclude the types (Others) as per Jira (VW-876) @@ 06-07-2023
                if ((!string.IsNullOrEmpty(extraData) || !string.IsNullOrEmpty(sequenceOrCustomCardNumber))
                    && (userTicketType == (int)EUserTicketTypes.LinkWithNajm
                    || userTicketType == (int)EUserTicketTypes.ChangePolicyData
                    || userTicketType == (int)EUserTicketTypes.CouldnotPrintPolicy
                    || userTicketType == (int)EUserTicketTypes.PolicyGeneration
                    || userTicketType == (int)EUserTicketTypes.UpdateCustomToSequence
                    || userTicketType == (int)EUserTicketTypes.ReachMaximumPolicyIssuance)) // Policy
                {
                    // Validate that the id is for policy for the current user
                    int policyId = 0;
                    int.TryParse(extraData, out policyId);
                    PolicyDetailsDB userPolicy = GetLastPolicyBySequenceOrCustomCardNumber(sequenceOrCustomCardNumber, userId);
                    if (userPolicy == null)
                    {
                        output.ErrorCode = UserTicketOutput.ErrorCodes.PolicyIdNotBelongsToThisUser;
                        output.ErrorDescription = UserTicketResources.PolicyIdNotBelongsToThisUser;
                        log.ErrorCode = (int)output.ErrorCode;
                        //log.ErrorDescription = $"Policy Id:{extraData} and sequenceOrCustomCardNumber:{sequenceOrCustomCardNumber} , Not Belongs To This User: {userId}";
                        log.ErrorDescription = $"No policy exist for vehicleId:{sequenceOrCustomCardNumber} and UserId: {userId}";
                        UserTicketLogDataAccess.AddUserTicketLog(log);
                        return output;
                    }

                    //if (userPolicy.PolicyIssueDate.HasValue)
                    //{
                    //    double hoursFromIssuePolicy = (DateTime.Now - userPolicy.PolicyIssueDate.Value).TotalHours;

                    //    if (userTicketType == (int)EUserTicketTypes.LinkWithNajm && hoursFromIssuePolicy < 24) // Najm 24 hour validation
                    //    {
                    //        output.ErrorCode = UserTicketOutput.ErrorCodes.NajmPolicyIssuedBefore24hours;
                    //        output.ErrorDescription = UserTicketResources.NajmPolicyIssuedBefore24hours;
                    //        log.ErrorCode = (int)output.ErrorCode;
                    //        log.ErrorDescription = $"Policy Id:{extraData} and sequenceOrCustomCardNumber:{sequenceOrCustomCardNumber}, Najm Policy Issued Before 24 hours"; // will add Policy Issued date to log
                    //        UserTicketLogDataAccess.AddUserTicketLog(log);
                    //        return output;
                    //    }
                    //}

                    userTicketModel.PolicyId = userPolicy.Id;
                    userTicketModel.PolicyNo = userPolicy.PolicyNo;
                    userTicketModel.ReferenceId = userPolicy.ReferenceId;
                    userTicketModel.CheckedoutEmail = userPolicy.CheckoutDetailEmail;
                    userTicketModel.CheckedoutPhone = userPolicy.CheckoutDetailPhone;
                    userTicketModel.DriverNin = userPolicy.NIN;
                    phoneToSendSMS = userPolicy.CheckoutDetailPhone;

                    var vehicle = new Vehicle();
                    vehicle.ID = userPolicy.VehicleID;
                    vehicle.VehicleMaker = userPolicy.VehicleMaker;
                    vehicle.VehicleMakerCode = userPolicy.VehicleMakerCode;
                    vehicle.VehicleModel = userPolicy.VehicleModel;
                    vehicle.VehicleModelCode = userPolicy.VehicleModelCode;
                    vehicle.CarPlateNumber = userPolicy.CarPlateNumber;
                    vehicle.CarPlateText1 = userPolicy.CarPlateText1;
                    vehicle.CarPlateText2 = userPolicy.CarPlateText2;
                    vehicle.CarPlateText3 = userPolicy.CarPlateText3;
                    vehicle.PlateTypeCode = userPolicy.PlateTypeCode;
                    vehicle.ModelYear = userPolicy.ModelYear;
                    vehicle.SequenceNumber = userPolicy.SequenceNumber;
                    vehicle.CustomCardNumber = userPolicy.CustomCardNumber;

                    userTicketModel.VehicleId = vehicle.ID;
                    userTicketModel.SequenceNumber = vehicle.SequenceNumber;
                    userTicketModel.CustomCardNumber = vehicle.CustomCardNumber;

                    userTicketModel.InvoiceId = userPolicy.InvoiceId;
                    userTicketModel.InvoiceNo = userPolicy.InvoiceNo;

                    log.ReferenceId = userPolicy.ReferenceId;
                    log.DriverNin = userPolicy.NIN;

                    output.Vehicle = vehicle;
                    if (language == LanguageTwoLetterIsoCode.Ar)
                    {
                        output.InsuranceCompanyName = userPolicy.InsuranceCompanyNameAR;
                    }
                    else
                    {
                        output.InsuranceCompanyName = userPolicy.InsuranceCompanyNameEN;
                    }
                }
                else if (userTicketType == (int)EUserTicketTypes.Others && !string.IsNullOrEmpty(sequenceOrCustomCardNumber))
                {
                    userTicketModel.SequenceNumber = sequenceOrCustomCardNumber;
                    userTicketModel.CustomCardNumber = sequenceOrCustomCardNumber;
                }
                else if (userTicketType == (int)EUserTicketTypes.NationalAddress || userTicketType == (int)EUserTicketTypes.ProofIDAndIBANAndEmail)
                {
                    userTicketModel.DriverNin = nin;
                }

                int userTicketId = CreateUserTicket(userTicketModel);
                if (userTicketId == 0)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.FailedToCreateUserTicket;
                    output.ErrorDescription = UserTicketResources.ErrorGeneric;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failed To Create User Ticket";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;

                }

                string exception = string.Empty;
                int i = 1;
                List<UserTicketAttachment> userTicketAttachments = new List<UserTicketAttachment>();
                //if (postedFiles != null)
                //{
                //    foreach (var item in postedFiles)
                //    {
                //        if (!item.InputStream.CanRead)
                //        {
                //            output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                //            output.ErrorDescription = UserTicketResources.FormatNotSupported;
                //            log.ErrorCode = (int)output.ErrorCode;
                //            log.ErrorDescription = $"can not read input stream";
                //            UserTicketLogDataAccess.AddUserTicketLog(log);
                //            return output;
                //        }
                //        string extension = Path.GetExtension(item.FileName);
                //        exception = string.Empty;
                //        byte[] fileBytes;
                //        Stream fileStream = item.InputStream;
                //            MemoryStream memoryStream = fileStream as MemoryStream;
                //            if (memoryStream == null)
                //            {
                //                memoryStream = new MemoryStream();
                //                fileStream.CopyTo(memoryStream);
                //            }
                //            fileBytes = memoryStream.ToArray();
                //        if(fileBytes==null)
                //        {
                //            output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                //            output.ErrorDescription = UserTicketResources.FormatNotSupported;
                //            log.ErrorCode = (int)output.ErrorCode;
                //            log.ErrorDescription = $"fileBytes is null";
                //            UserTicketLogDataAccess.AddUserTicketLog(log);
                //            return output;
                //        }
                //        if (extension.ToLower() == ".pdf")
                //        {
                //            bool validPdf = Utilities.IsValidPdf(fileBytes, out exception);
                //            if (!validPdf || !string.IsNullOrEmpty(exception))
                //            {
                //                output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                //                output.ErrorDescription = UserTicketResources.FormatNotSupported;
                //                log.ErrorCode = (int)output.ErrorCode;
                //                log.ErrorDescription = $"file contnet is not valid as extension is:" + extension + " and exception:" + exception;
                //                UserTicketLogDataAccess.AddUserTicketLog(log);
                //                return output;
                //            }
                //        }
                //        else
                //        {
                //            bool validImage = Utilities.CheckImage(item.InputStream, out exception);
                //            if (!validImage || !string.IsNullOrEmpty(exception))
                //            {
                //                output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                //                output.ErrorDescription = UserTicketResources.FormatNotSupported;
                //                log.ErrorCode = (int)output.ErrorCode;
                //                log.ErrorDescription = $"file contnet is not valid as extension is:" + extension + " and exception:" + exception;
                //                UserTicketLogDataAccess.AddUserTicketLog(log);
                //                return output;
                //            }
                //        }
                      
                //        var attachmentPath = Utilities.SaveUserTicketAttachmentFile(userTicketId, fileBytes, Path.GetExtension(item.FileName), i++.ToString()
                //            , _tameenkConfig.RemoteServerInfo.UseNetworkDownload, _tameenkConfig.RemoteServerInfo.DomainName, _tameenkConfig.RemoteServerInfo.ServerIP,
                //            _tameenkConfig.RemoteServerInfo.ServerUserName, _tameenkConfig.RemoteServerInfo.ServerPassword, out exception);

                //        fileStream.Close();
                //        if (string.IsNullOrEmpty(attachmentPath))
                //        {
                //            //errors.Add(exception);
                //            output.ErrorCode = UserTicketOutput.ErrorCodes.FailedToUploadAttachment;
                //            output.ErrorDescription = UserTicketResources.ErrorGeneric;
                //            log.ErrorCode = (int)output.ErrorCode;
                //            log.ErrorDescription = $"Failed To UploadAttachment: {exception}";
                //            UserTicketLogDataAccess.AddUserTicketLog(log);
                //            // return output;
                //        }

                //        userTicketAttachments.Add(new UserTicketAttachment { AttachmentPath = attachmentPath, TicketId = userTicketId });
                //    }
                //}
                //else 
                if (attachedFiles != null)
                {
                    foreach (var item in attachedFiles)
                    {
                        if (string.IsNullOrEmpty(item.Extension) || item.File == null)
                        {
                            continue;
                        }
                        exception = string.Empty;
                        Stream fileStream = new MemoryStream(item.File);

                        if (!fileStream.CanRead)
                        {
                            output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                            output.ErrorDescription = UserTicketResources.FormatNotSupported;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = $"can not read input stream";
                            UserTicketLogDataAccess.AddUserTicketLog(log);
                            return output;
                        }
                        var fileBytes = item.File;
                        //string extension = Path.GetExtension(item.Extension);
                        exception = string.Empty;
                        if (item.Extension.ToLower().Contains("pdf")) // if (extension.ToLower() == ".pdf")
                        {
                            bool validPdf = Utilities.IsValidPdf(fileBytes, out exception);
                            if (!validPdf || !string.IsNullOrEmpty(exception))
                            {
                                output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                                output.ErrorDescription = UserTicketResources.FormatNotSupported;
                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = $"file contnet is not valid as extension is:" + item.Extension + " and exception:" + exception;
                                UserTicketLogDataAccess.AddUserTicketLog(log);
                                return output;
                            }
                        }
                        else
                        {
                            bool validImage = Utilities.CheckImage(fileStream, out exception);
                            if (!validImage || !string.IsNullOrEmpty(exception))
                            {
                                output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                                output.ErrorDescription = UserTicketResources.FormatNotSupported;
                                log.ErrorCode = (int)output.ErrorCode;
                                log.ErrorDescription = $"file contnet is not valid as extension is:" + item.Extension + " and exception:" + exception;
                                UserTicketLogDataAccess.AddUserTicketLog(log);
                                return output;
                            }
                        }


                       
                        var attachmentPath = Utilities.SaveUserTicketAttachmentFile(userTicketId, fileBytes, item.Extension, i++.ToString()
                            , _tameenkConfig.RemoteServerInfo.UseNetworkDownload, _tameenkConfig.RemoteServerInfo.DomainName, _tameenkConfig.RemoteServerInfo.ServerIP,
                            _tameenkConfig.RemoteServerInfo.ServerUserName, _tameenkConfig.RemoteServerInfo.ServerPassword, out exception);

                        if (string.IsNullOrEmpty(attachmentPath))
                        {
                            //errors.Add(exception);
                            output.ErrorCode = UserTicketOutput.ErrorCodes.FailedToUploadAttachment;
                            output.ErrorDescription = UserTicketResources.ErrorGeneric;
                            log.ErrorCode = (int)output.ErrorCode;
                            log.ErrorDescription = $"Failed To UploadAttachment: {exception}";
                            UserTicketLogDataAccess.AddUserTicketLog(log);
                            // return output;
                        }

                        userTicketAttachments.Add(new UserTicketAttachment { AttachmentPath = attachmentPath, TicketId = userTicketId });
                    }
                }

                if (userTicketAttachments.Any())
                {
                    int result = InsertUserTicketAttahments(userTicketAttachments);
                }

                //Send SMS With Ticket Number
                string messageBodySMS = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreated", CultureInfo.GetCultureInfo(language.ToString().ToLower())), userTicketId.ToString("0000000000"));
                var smsModel = new SMSModel()
                {
                    PhoneNumber = phoneToSendSMS,
                    MessageBody = messageBodySMS,
                    Module = Module.Vehicle.ToString(),
                    Channel = channel
                };
                if (channel == Channel.Dashboard.ToString())
                {
                    smsModel.Method = SMSMethod.UserTicketAdmin.ToString();
                }
                else
                {
                    smsModel.Method = SMSMethod.UserTicketWebsite.ToString();
                }
                _notificationService.SendSmsBySMSProviderSettings(smsModel);

                output.ErrorCode = UserTicketOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                output.UserTicketId = userTicketId;
                output.PolicyNo = userTicketModel.PolicyNo;
                output.InvoiceNo = userTicketModel.InvoiceNo;
                output.TicketTypeId = userTicketModel.TicketTypeId;
                output.UserId = userId;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = UserTicketOutput.ErrorCodes.Exception;
                output.ErrorDescription = UserTicketResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = ex.ToString();
                UserTicketLogDataAccess.AddUserTicketLog(log);
                return output;
            }
        }

        public List<Policy> GetPoliciesByUserId(string userId)
        {
            return _policyRepository.TableNoTracking.
                Include(p => p.CheckoutDetail)
                .Include(p => p.CheckoutDetail.PolicyStatus)
                .Where(p => p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                && p.CheckoutDetail.UserId == userId).OrderBy(p => p.PolicyNo).ToList();
        }

        public List<UserTicketType> GetTicketTypesDB()
        {
            List<UserTicketType> ticketTypes = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            idbContext.DatabaseInstance.CommandTimeout = new int?(60);
            var command = idbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetTicketTypes";
            command.CommandType = CommandType.StoredProcedure;
            idbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            ticketTypes = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<UserTicketType>(reader).ToList();
            idbContext.DatabaseInstance.Connection.Close();
            return ticketTypes;
        }



        public List<Invoice> GetInvoicesByUserId(string userId)
        {
            return (from invo in _invoiceRepository.TableNoTracking
                    join checkOutDetails in _checkoutDetailsRepository.TableNoTracking on invo.ReferenceId equals checkOutDetails.ReferenceId
                    orderby invo.InvoiceNo ascending
                    where invo.UserId == userId &&
                    checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PaymentFailure &&
                    checkOutDetails.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                    select invo).ToList();
        }

        public List<Vehicle> GetVehiclesByUserId(string userId)
        {
            return _checkoutDetailsRepository.TableNoTracking.Include(c => c.Vehicle)
                .Where(c => c.UserId == userId && !c.Vehicle.IsDeleted).Select(x => x.Vehicle).Distinct().ToList();
        }

        public List<UserTicketsDBModel> GetUserTicketsWithLastHistory(string userId)
        {
            List<UserTicketsDBModel> userTickets = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            idbContext.DatabaseInstance.CommandTimeout = 60;
            var command = idbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetUserTicketsWithLastHistory";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "@userId", Value = userId });
            idbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            userTickets = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<UserTicketsDBModel>(reader).ToList();
            idbContext.DatabaseInstance.Connection.Close();
            return userTickets;

        }

        public UserTicketOutput CreateUserTicketFromDashboard(CreateUserTicketModel createUserTicketModel)
        {
            UserTicketOutput output = new UserTicketOutput();
            UserTicketLog log = new UserTicketLog();
            try
            {
                if (createUserTicketModel == null)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidInput;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"createUserTicketModel is null";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (string.IsNullOrEmpty(createUserTicketModel.UserPhone))
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidInput;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"UserPhone is empty";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                var phone = Utilities.ValidateInternalPhoneNumber(createUserTicketModel.UserPhone);
                bool checkForpolicy = DashboardTicketTypesNoPolicyRequiredToOpenTicket.Contains(createUserTicketModel.TicketTypeId) ? false : true;
                var user = GetUserByPhone(phone, checkForpolicy);
                if (user == null)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = $"UserPhone:{createUserTicketModel.UserPhone} doesnot correspond to any user"; //UserTicketResources.InvalidUserId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"UserPhone:{createUserTicketModel.UserPhone} doesnot correspond to any user";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }
                var lang = LanguageTwoLetterIsoCode.Ar;
                if (createUserTicketModel.Language == LanguageTwoLetterIsoCode.En.ToString().ToLower())
                {
                    lang = LanguageTwoLetterIsoCode.En;
                }
                output = CreateUserTicket(createUserTicketModel.TicketTypeId, null, createUserTicketModel.SequenceOrCustomCardNumber, createUserTicketModel.UserNotes,
                    user.Id, createUserTicketModel.Channel, lang, null, createUserTicketModel.AttachedFiles, createUserTicketModel.NIN, createUserTicketModel.CreatedBy);
                return output;

            }
            catch (Exception ex)
            {
                output.ErrorCode = UserTicketOutput.ErrorCodes.Exception;
                output.ErrorDescription = UserTicketResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "CreateUserTicketFromDashboard: " + ex.ToString();
                UserTicketLogDataAccess.AddUserTicketLog(log);
                return output;
            }

        }

        public UserTicketOutput CreateUserTicketFromAPI(CreateUserTicketAPIModel createUserTicketModel)
        {
            UserTicketOutput output = new UserTicketOutput();
            UserTicketLog log = new UserTicketLog();
            try
            {
                if (createUserTicketModel == null)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidInput;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"createUserTicketModel is null";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                if (string.IsNullOrEmpty(createUserTicketModel.UserId))
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidUserId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"UserId is empty";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }

                var lang = LanguageTwoLetterIsoCode.Ar;
                if (createUserTicketModel.Language == LanguageTwoLetterIsoCode.En.ToString().ToLower())
                {
                    lang = LanguageTwoLetterIsoCode.En;
                }
                output = CreateUserTicket(createUserTicketModel.TicketTypeId, null, createUserTicketModel.SequenceOrCustomCardNumber, createUserTicketModel.UserNotes,
                   createUserTicketModel.UserId, createUserTicketModel.Channel, lang, createUserTicketModel.postedFiles, createUserTicketModel.AttachedFiles, createUserTicketModel.Nin);
                return output;

            }
            catch (Exception ex)
            {
                output.ErrorCode = UserTicketOutput.ErrorCodes.Exception;
                output.ErrorDescription = UserTicketResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "CreateUserTicketFromDashboard: " + ex.ToString();
                UserTicketLogDataAccess.AddUserTicketLog(log);
                return output;
            }

        }
        public void SendUpdatedStatusSMS(string userId, string language, int newTicketStatus, int userTicketId, string adminReply)
        {
            string phoneToSendSMS = string.Empty;
            var ticket = _userTicketRepository.TableNoTracking.Where(x => x.Id == userTicketId).FirstOrDefault();
            if (ticket != null)
            {
                if (ticket.TicketTypeId == 1 ||
                    ticket.TicketTypeId == 2 ||
                    ticket.TicketTypeId == 3 ||
                    ticket.TicketTypeId == 4) // Policy related
                {
                    phoneToSendSMS = ticket.CheckedoutPhone;
                }
                else
                {
                    phoneToSendSMS = ticket.UserPhone;
                }
            }
            else
            {
                return;
            }

            string messageBodySMS = string.Empty;

            if (newTicketStatus == (int)EUserTicketStatus.TicketClosed)
            {
                messageBodySMS = string.Format(UserTicketResources.ResourceManager.GetString("TicketClosed", CultureInfo.GetCultureInfo(language)), userTicketId);
                messageBodySMS += ": " + adminReply;
            }
            else
            {
                messageBodySMS = string.Format(UserTicketResources.ResourceManager.GetString("NewReplyAdded", CultureInfo.GetCultureInfo(language)), userTicketId);
            }

            var smsModel = new SMSModel()
            {
                PhoneNumber = phoneToSendSMS,
                MessageBody = messageBodySMS,
                Method = SMSMethod.UserTicketAdmin.ToString(),
                Module = Module.Vehicle.ToString(),
                Channel = Channel.Dashboard.ToString()
            };
            _notificationService.SendSmsBySMSProviderSettings(smsModel);
            _notification.SendFireBaseNotification(userId, "بي كير - Bcare", messageBodySMS, "TicketClose", userTicketId.ToString(), string.Empty);

        }

        public UserTicketOutput DeleteUserTicketHistory(int historyId)
        {
            UserTicketOutput output = new UserTicketOutput();
            UserTicketLog log = new UserTicketLog();
            log.MethodName = "DeleteUserTicketHistory";
            try
            {
                var ticketHistoryItem = _userTicketHistoryRepository.Table.Where(h => h.Id == historyId && h.TicketStatusId == (int)EUserTicketStatus.UnderProcessing).FirstOrDefault();
                if (ticketHistoryItem == null)
                {
                    output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                    output.ErrorDescription = UserTicketResources.InvalidInput;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = $"DeleteUserTicketHistory: ticketHistoryItem is null for historyId:{historyId}";
                    UserTicketLogDataAccess.AddUserTicketLog(log);
                    return output;
                }
                int userTicketId = ticketHistoryItem.TicketId.Value;

                _userTicketHistoryRepository.Delete(ticketHistoryItem);


                var lastUserTicketHistory = _userTicketHistoryRepository.TableNoTracking.Where(t => t.TicketId == userTicketId).OrderByDescending(t => t.Id).FirstOrDefault();
                if (lastUserTicketHistory != null && lastUserTicketHistory.TicketStatusId == (int)EUserTicketStatus.TicketOpened)
                {
                    var userTicket = _userTicketRepository.Table.Where(t => t.Id == userTicketId).FirstOrDefault();
                    if (userTicket == null)
                    {
                        output.ErrorCode = UserTicketOutput.ErrorCodes.InvalidInput;
                        output.ErrorDescription = UserTicketResources.InvalidInput;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"DeleteUserTicketHistory: userTicket is null for userTicketId:{userTicketId}";
                        UserTicketLogDataAccess.AddUserTicketLog(log);
                        return output;
                    }

                    userTicket.CurrentTicketStatusId = (int)EUserTicketStatus.TicketOpened;
                    _userTicketRepository.Update(userTicket);
                }

                output.ErrorCode = UserTicketOutput.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = UserTicketOutput.ErrorCodes.Exception;
                output.ErrorDescription = UserTicketResources.ErrorGeneric;
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "DeleteUserTicketHistory: " + ex.ToString();
                UserTicketLogDataAccess.AddUserTicketLog(log);
                return output;
            }
        }
        #region Private Methods

        private Policy GetPolicyByPolicyID(int policyId, string sequenceOrCustomCardNumber, string userId)
        {
            var mainQuery = _policyRepository.TableNoTracking
                .Include(p => p.InsuranceCompany)
                .Include(p => p.CheckoutDetail)
                .Include(p => p.CheckoutDetail.PolicyStatus)
                .Include(p => p.CheckoutDetail.Driver)
                .Include(p => p.CheckoutDetail.Vehicle);

            if (policyId != 0)
            {
                return mainQuery.Where(p => p.Id == policyId && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                && p.CheckoutDetail.UserId == userId).OrderByDescending(p => p.Id).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(sequenceOrCustomCardNumber))
            {
                return mainQuery.Where(p => (p.CheckoutDetail.Vehicle.SequenceNumber == sequenceOrCustomCardNumber || p.CheckoutDetail.Vehicle.CustomCardNumber == sequenceOrCustomCardNumber)
                && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PaymentFailure
                && p.CheckoutDetail.PolicyStatusId != (int)EPolicyStatus.PendingPayment
                && p.CheckoutDetail.UserId == userId).OrderByDescending(p => p.Id).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        private PolicyDetailsDB GetLastPolicyBySequenceOrCustomCardNumber(string sequenceOrCustomCardNumber, string userId)
        {
            PolicyDetailsDB userPolicyDetails = null;
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();
            idbContext.DatabaseInstance.CommandTimeout = 60;
            var command = idbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetLastPolicyBySequenceOrCustomCardNumber";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "@userId", Value = userId });            command.Parameters.Add(new SqlParameter() { ParameterName = "@sequenceOrCustomCardNumber", Value = sequenceOrCustomCardNumber });
            idbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            userPolicyDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<PolicyDetailsDB>(reader).FirstOrDefault();
            idbContext.DatabaseInstance.Connection.Close();
            return userPolicyDetails;
        }

        private Vehicle GetUserVehicleByVehicleId(string vehicleId, string sequenceOrCustomCardNumber)
        {
            var mainQuery = _vehicleRepository.TableNoTracking;

            if (!string.IsNullOrEmpty(vehicleId))
            {
                Guid vegicleId = Guid.Empty;
                Guid.TryParse(vehicleId, out vegicleId);
                return mainQuery.Where(p => p.ID == vegicleId).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(sequenceOrCustomCardNumber))
            {
                return mainQuery.Where(p => (p.SequenceNumber == sequenceOrCustomCardNumber || p.CustomCardNumber == sequenceOrCustomCardNumber) && !p.IsDeleted).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        private UserInvoiceDetailsDBModel GetUserInvoiceByInvoiceId(int? invoiceId, string sequenceOrCustomCardNumber, string userId)
        {
            UserInvoiceDetailsDBModel userInvoiceDetails = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            idbContext.DatabaseInstance.CommandTimeout = 60;
            var command = idbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetUserInvoiceByInvoiceId";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "@userId", Value = userId });            command.Parameters.Add(new SqlParameter() { ParameterName = "@invoiceId", Value = invoiceId });
            idbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            userInvoiceDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<UserInvoiceDetailsDBModel>(reader).FirstOrDefault();
            idbContext.DatabaseInstance.Connection.Close();
            return userInvoiceDetails;
        }




        private int CreateUserTicket(UserTicketModel userTicketModel)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();            dbContext.DatabaseInstance.CommandTimeout = 60;            var command = dbContext.DatabaseInstance.Connection.CreateCommand();            command.CommandText = "CreateUserTicket";            command.CommandType = CommandType.StoredProcedure;            command.Parameters.Add(new SqlParameter() { ParameterName = "@userId", Value = userTicketModel.UserId });            command.Parameters.Add(new SqlParameter() { ParameterName = "@checkedoutEmail", Value = userTicketModel.CheckedoutEmail });            command.Parameters.Add(new SqlParameter() { ParameterName = "@checkedoutPhone", Value = userTicketModel.CheckedoutPhone });            command.Parameters.Add(new SqlParameter() { ParameterName = "@userEmail", Value = userTicketModel.UserEmail });            command.Parameters.Add(new SqlParameter() { ParameterName = "@userPhone", Value = userTicketModel.UserPhone });            command.Parameters.Add(new SqlParameter() { ParameterName = "@ticketTypeId", Value = userTicketModel.TicketTypeId });            command.Parameters.Add(new SqlParameter() { ParameterName = "@userNotes", Value = userTicketModel.UserNotes });            command.Parameters.Add(new SqlParameter() { ParameterName = "@createdDate", Value = userTicketModel.CreatedDate });            command.Parameters.Add(new SqlParameter() { ParameterName = "@referenceId", Value = userTicketModel.ReferenceId });            command.Parameters.Add(new SqlParameter() { ParameterName = "@driverNin", Value = userTicketModel.DriverNin });            if (userTicketModel.PolicyId.HasValue && !string.IsNullOrEmpty(userTicketModel.PolicyNo))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@policyId", Value = userTicketModel.PolicyId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@policyNo", Value = userTicketModel.PolicyNo });
            }            if (userTicketModel.InvoiceId.HasValue && userTicketModel.InvoiceNo.HasValue)
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@invoiceId", Value = userTicketModel.InvoiceId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@invoiceNo", Value = userTicketModel.InvoiceNo });
            }            if (userTicketModel.VehicleId != null && userTicketModel.VehicleId != Guid.Empty)
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@vehicleId", Value = userTicketModel.VehicleId });            }            if (!string.IsNullOrEmpty(userTicketModel.SequenceNumber))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@sequenceNumber", Value = userTicketModel.SequenceNumber });            }            if (!string.IsNullOrEmpty(userTicketModel.CustomCardNumber))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@customCardNumber", Value = userTicketModel.CustomCardNumber });            }            if (!string.IsNullOrEmpty(userTicketModel.Channel))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@channel", Value = userTicketModel.Channel });
            }
            else
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@channel", Value = "Portal" });
            }            command.Parameters.Add(new SqlParameter() { ParameterName = "@createdBy", Value = userTicketModel.CreatedBy });            dbContext.DatabaseInstance.Connection.Open();            var reader = command.ExecuteReader();            int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();            dbContext.DatabaseInstance.Connection.Close();            return result;
        }

        private UserTicketHistory GetFirstUserTicketHistoryByTicketId(int userTicketId)
        {
            UserTicketHistory ticketHistory = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            idbContext.DatabaseInstance.CommandTimeout = 60;
            var command = idbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetFirstUserTicketHistory";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "@userTicketId", Value = userTicketId });
            idbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            ticketHistory = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<UserTicketHistory>(reader).FirstOrDefault();
            idbContext.DatabaseInstance.Connection.Close();
            return ticketHistory;

        }

        private int InsertUserTicketAttahments(List<UserTicketAttachment> userTicketAttachments)
        {
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();            dbContext.DatabaseInstance.CommandTimeout = 60;            var command = dbContext.DatabaseInstance.Connection.CreateCommand();            command.CommandText = "InsertUserTicketAttahment";            command.CommandType = CommandType.StoredProcedure;            DataTable dtUserTicketAttachmentInfo = new DataTable();
            //Add columns  
            dtUserTicketAttachmentInfo.Columns.Add(new DataColumn("AttachmentPath", typeof(string)));
            dtUserTicketAttachmentInfo.Columns.Add(new DataColumn("TicketId", typeof(int)));

            //Add rows  
            foreach (var item in userTicketAttachments)
            {
                dtUserTicketAttachmentInfo.Rows.Add(item.AttachmentPath, item.TicketId);
            }
            command.Parameters.Add(new SqlParameter() { ParameterName = "@tableAttachmentInfo", Value = dtUserTicketAttachmentInfo });            dbContext.DatabaseInstance.Connection.Open();            var reader = command.ExecuteReader();            int result = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();            dbContext.DatabaseInstance.Connection.Close();            return result;
        }

        private AspNetUser GetUserByPhone(string phone, bool checkForpolicy)
        {
            AspNetUser userDetails = null;
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();
            idbContext.DatabaseInstance.CommandTimeout = 60;
            var command = idbContext.DatabaseInstance.Connection.CreateCommand();
            command.CommandText = "GetUserByPhone";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "@phone", Value = phone });
            command.Parameters.Add(new SqlParameter() { ParameterName = "@formatedPhone", Value = Utilities.ValidatePhoneNumber(phone) });            command.Parameters.Add(new SqlParameter() { ParameterName = "@checkForpolicy", Value = checkForpolicy });
            idbContext.DatabaseInstance.Connection.Open();
            var reader = command.ExecuteReader();
            userDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AspNetUser>(reader).FirstOrDefault();
            idbContext.DatabaseInstance.Connection.Close();
            return userDetails;
        }

        private ValidateUserTicketOutput ValidateBeforeOpenTicket(int ticketTypeId, string sequenceOrCustomCardNumber, string nin)
        {
            ValidateUserTicketOutput output = new ValidateUserTicketOutput();
            output.ErrorCode = ValidateUserTicketOutput.ErrorCodes.Valid;

            if (ticketTypeId == (int)EUserTicketTypes.LinkWithNajm
                    || ticketTypeId == (int)EUserTicketTypes.ChangePolicyData
                    || ticketTypeId == (int)EUserTicketTypes.CouldnotPrintPolicy
                    || ticketTypeId == (int)EUserTicketTypes.PolicyGeneration
                    || ticketTypeId == (int)EUserTicketTypes.UpdateCustomToSequence
                    || ticketTypeId == (int)EUserTicketTypes.Percentage
                    || ticketTypeId == (int)EUserTicketTypes.ReachMaximumPolicyIssuance
                    || ticketTypeId == (int)EUserTicketTypes.Others)
            {
                var notClosedTickets = _userTicketRepository.TableNoTracking
                    .Where(t => t.CurrentTicketStatusId != (int)EUserTicketStatus.TicketClosed
                    && (t.SequenceNumber == sequenceOrCustomCardNumber || t.CustomCardNumber == sequenceOrCustomCardNumber)).ToList();
                if (notClosedTickets != null && notClosedTickets.Any())
                {
                    output.ErrorCode = ValidateUserTicketOutput.ErrorCodes.NotValid;
                    output.ErrorDescription = string.Format(UserTicketResources.OpenedTicketExistsForSequenceOrCustomCardNumber, sequenceOrCustomCardNumber, notClosedTickets.FirstOrDefault().Id);
                }
            }
            else if (ticketTypeId == (int)EUserTicketTypes.NationalAddress || ticketTypeId == (int)EUserTicketTypes.ProofIDAndIBANAndEmail)
            {
                var notClosedTickets = _userTicketRepository.TableNoTracking
                    .Where(t => t.CurrentTicketStatusId != (int)EUserTicketStatus.TicketClosed
                    && t.TicketTypeId == ticketTypeId
                    && t.DriverNin == nin).ToList();
                if (notClosedTickets != null && notClosedTickets.Any())
                {
                    output.ErrorCode = ValidateUserTicketOutput.ErrorCodes.NotValid;
                    output.ErrorDescription = string.Format(UserTicketResources.OpenedTicketExistsForNIN, nin, notClosedTickets.FirstOrDefault().Id);
                }
            }
            else
            {
                output.ErrorCode = ValidateUserTicketOutput.ErrorCodes.TicketTypeNotAvaliable;
            }
            return output;
        }
        #endregion
    }
}
