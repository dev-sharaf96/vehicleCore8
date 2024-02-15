using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums;
using Tameenk.Loggin.DAL;
using Tameenk.Resources.UserTicket;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Addresses;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.YakeenIntegration.Business;
using Tameenk.Services.YakeenIntegration.Business.WebClients.Core;

namespace Tameenk.IVR.Component
{
    public class IVRTicketContext : IIVRTicketContext
    {
        private readonly IRepository<UserTicket> _userTicketRepository;
        private IIVRService _iIVRTicketService;
        private readonly IRepository<Driver> _driverRepository;
        private readonly IRepository<InsuredAddressesCount> _insuredAddressesCountRepository;
        private readonly IYakeenClient _yakeenClient;
        private readonly IAddressService _addressService;
        private readonly IRepository<UserTicketHistory> _userTicketHistoryRepository;


        public IVRTicketContext(IRepository<UserTicket> userTicketRepository, IIVRService iIVRTicketService
            , IRepository<Driver> driverRepository, IRepository<InsuredAddressesCount> insuredAddressesCountRepository, IYakeenClient yakeenClient, IAddressService addressService
            , IRepository<UserTicketHistory> userTicketHistoryRepository)
        {
            _userTicketRepository = userTicketRepository;
            _iIVRTicketService = iIVRTicketService;
            _driverRepository = driverRepository;
            _insuredAddressesCountRepository = insuredAddressesCountRepository;
            _yakeenClient = yakeenClient;
            _addressService = addressService;
            _userTicketHistoryRepository = userTicketHistoryRepository;
        }

        public IVRTicketOutput<TicketDetailsModel> GetTicketDetails(int ticketNo, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRTicketOutput<TicketDetailsModel> output = new IVRTicketOutput<TicketDetailsModel>();
            output.Result = null;

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = $"ticketNo: {ticketNo}";
            AddBasicLog(log, methodName, IVRModuleEnum.Ticket);

            try
            {
                var ticket = _userTicketRepository.TableNoTracking.Where(a => a.Id == ticketNo).FirstOrDefault();
                if (ticket == null)
                {
                    output.ErrorCode = IVRTicketOutput<TicketDetailsModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "There is no ticket in DB with the provided tickeNo: " + ticketNo;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output.Result = new TicketDetailsModel()
                {
                    TicketNo = ticket.Id,
                    TicketStatusId = ticket.CurrentTicketStatusId ?? (int)EUserTicketStatus.UnderProcessing,
                    TicketStatusName = ticket.CurrentTicketStatusId.HasValue ? ((EUserTicketStatus)ticket.CurrentTicketStatusId.Value).ToString() : EUserTicketStatus.UnderProcessing.ToString(),
                    TicketTypeId = ticket.TicketTypeId ?? (int)IVRTicketTypeEnum.Others,
                    TicketTypeName = ticket.CurrentTicketStatusId.HasValue ? ((IVRTicketTypeEnum)ticket.TicketTypeId.Value).ToString() : IVRTicketTypeEnum.Others.ToString()
                };

                output.ErrorCode = IVRTicketOutput<TicketDetailsModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRTicketOutput<TicketDetailsModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        public IVRTicketOutput<NewTicketResponseModel> CreateNewTicket(NewTicketModel ticketModel, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRTicketOutput<NewTicketResponseModel> output = new IVRTicketOutput<NewTicketResponseModel>();
            output.Result = null;

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = JsonConvert.SerializeObject(ticketModel);
            AddBasicLog(log, methodName, IVRModuleEnum.Ticket);

            try
            {
                if (ticketModel == null)
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.EmptyDataModel;
                    output.ErrorDescription = "Ticket data model is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (!Enum.IsDefined(typeof(IVRTicketTypeEnum), ticketModel.TicketTypeId))
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.TicketTypeNotAvaliable;
                    output.ErrorDescription = "TicketTypeId value is invalid, please check the shared values";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if ((ticketModel.TicketTypeId != (int)IVRTicketTypeEnum.Others && ticketModel.TicketTypeId != (int)IVRTicketTypeEnum.UpdateNationalAddress) && string.IsNullOrEmpty(ticketModel.VehicleId))
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = $"Custom card or Sequesnce No is required with TicketTypeId: {ticketModel.TicketTypeId}.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.ChangePolicyData) //  || ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.CouldNotPrintThePolicy
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.NotValidToOpenNewTicket;
                    output.ErrorDescription = $"Not allowed to open new ticket for type ChangePolicyData, as it must be from employee not IVR.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;

                    //if (!ticketModel.TicketSubTypeId.HasValue || ticketModel.TicketSubTypeId == 0)
                    //{
                    //    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.TicketTypeNotAvaliable;
                    //    output.ErrorDescription = $"TicketSubTypeId value is required with TicketTypeId: {ticketModel.TicketTypeId}.";
                    //    log.ErrorCode = (int)output.ErrorCode;
                    //    log.ErrorDescription = output.ErrorDescription;
                    //    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    //    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    //    return output;
                    //}
                    //if (!Enum.IsDefined(typeof(IVRTicketSubTypeEnum), ticketModel.TicketSubTypeId))
                    //{
                    //    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.TicketTypeNotAvaliable;
                    //    output.ErrorDescription = "TicketSubTypeId value is invalid, please check the shared values";
                    //    log.ErrorCode = (int)output.ErrorCode;
                    //    log.ErrorDescription = output.ErrorDescription;
                    //    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    //    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    //    return output;
                    //}

                    //IVRTicketTypeEnum ticketMainType = (IVRTicketTypeEnum)ticketModel.TicketTypeId;
                    //IVRTicketSubTypeEnum ticketSubType = (IVRTicketSubTypeEnum)ticketModel.TicketSubTypeId;
                    //if (!ticketSubType.IsEnumSubTypeOf(ticketMainType))
                    //{
                    //    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.TicketSubTypeNotAvaliableWithTicketType;
                    //    output.ErrorDescription = "TicketSubTypeId value is invalid because it's not sub type from the provided ticket type";
                    //    log.ErrorCode = (int)output.ErrorCode;
                    //    log.ErrorDescription = output.ErrorDescription;
                    //    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    //    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    //    return output;
                    //}
                    ////if (ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.CouldNotPrintThePolicy && ticketModel.TicketSubTypeId == (int)IVRTicketSubTypeEnum.PolicyNotExistWithSada && (!ticketModel.SadadNo.HasValue || ticketModel.SadadNo.Value <= 0))
                    ////{
                    ////    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.SadadNumberIsRequired;
                    ////    output.ErrorDescription = "Sadad no is required with the provided ticket type and sub type";
                    ////    log.ErrorCode = (int)output.ErrorCode;
                    ////    log.ErrorDescription = output.ErrorDescription;
                    ////    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    ////    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    ////    return output;
                    ////}
                }
                if (ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.UpdateNationalAddress && string.IsNullOrEmpty(ticketModel.NationalId))
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = $"National (Id, Iqama, CR) is required with TicketTypeId: {ticketModel.TicketTypeId}.";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (!ValidateBeforeOpenTicket(ticketModel.TicketTypeId, ticketModel.TicketSubTypeId, ticketModel.VehicleId, ticketModel.NationalId))
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.NotValidToOpenNewTicket;
                    output.ErrorDescription = "There is an open ticket for this user with the specified ticket type";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                IVRTicketOutput<NewTicketResponseModel> validateVehiclePolicyResult = ValidateVehiclePolicy(ticketModel);
                if (validateVehiclePolicyResult.ErrorCode != IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.Success)
                {
                    output.ErrorCode = validateVehiclePolicyResult.ErrorCode;
                    output.ErrorDescription = validateVehiclePolicyResult.ErrorDescription;
                    log.ErrorCode = (int)validateVehiclePolicyResult.ErrorCode;
                    log.ErrorDescription = validateVehiclePolicyResult.LogDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return validateVehiclePolicyResult;
                }

                string exception = string.Empty;
                if (ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.CouldNotPrintThePolicy)
                {
                    string sendToPhone = ticketModel.SendToCallerPhone ? ticketModel.PhoneNumber : validateVehiclePolicyResult.PolicyData.CheckoutDetailPhone;
                    var whatsAppResponse = SendWhatsAppMessage(sendToPhone, validateVehiclePolicyResult.PolicyData.ReferenceId, validateVehiclePolicyResult.PolicyData.SelectedLanguage, out exception);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        output.ErrorDescription = "Error happend while sending whatsApp message";
                        output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.ServiceException;
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = $"SendWhatsAppMessage return exception: {exception}";
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    }
                    else
                    {
                        output.ErrorDescription = "Success";
                        output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.Success;
                    }
                    
                    return output;
                }

                var result = HandleCreateNewTicket(ticketModel, validateVehiclePolicyResult.PolicyData, out exception);
                if (!string.IsNullOrEmpty(exception) || !result.IsSuccess)
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "Error happend while inserting new ticket data";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = !string.IsNullOrEmpty(exception) ? exception : output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output.Result = result;
                output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        public IVRTicketOutput<bool> UpdateNationalAddress(IVRUpdateNationalAddressModel model, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRTicketOutput<bool> output = new IVRTicketOutput<bool>();
            output.Result = false;

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = JsonConvert.SerializeObject(model);
            AddBasicLog(log, methodName, IVRModuleEnum.Ticket);

            try
            {
                if (model == null)
                {
                    output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.EmptyDataModel;
                    output.ErrorDescription = "Update national address data model is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }
                if (string.IsNullOrEmpty(model.NationalId))
                {
                    output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "NationalId parameter is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var driverData = _driverRepository.TableNoTracking.Where(a => a.NIN == model.NationalId && !a.IsDeleted).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();
                if (driverData == null)
                {
                    output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.NotFound;
                    output.ErrorDescription = "There is no data in DB with the provided nationalId: " + model.NationalId + " or the user is deleted";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var nationalAddressUpdates = _insuredAddressesCountRepository.TableNoTracking.Where(a => a.NationalId == model.NationalId && a.CreatedDate.Value.Year == DateTime.Now.Year).ToList();
                if (nationalAddressUpdates != null && nationalAddressUpdates.Count >= 1)
                {
                    var CanUpdate = (nationalAddressUpdates.Count >= 3) ? false : true;
                    if (!CanUpdate)
                    {
                        output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.ReachedToMaximum;
                        output.ErrorDescription = string.Format("This national id {0} has reached the maximum number of updates 3 times this year", model.NationalId);
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        IVRLogDataAccess.AddToIVRLogDataAccess(log);
                        return output;
                    }

                    var lastUpdate = nationalAddressUpdates.OrderByDescending(a => a.CreatedDate).FirstOrDefault();
                    if (lastUpdate != null && lastUpdate.CreatedDate.HasValue && lastUpdate.CreatedDate.Value >= DateTime.Now.AddMinutes(-10))
                    {
                        output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.CannotUpdateNationalAddressInLessThan10Minutes;
                        output.ErrorDescription = "You are trying to update your national address in less than 10 minutes";
                        log.ErrorCode = (int)output.ErrorCode;
                        log.ErrorDescription = output.ErrorDescription;
                        log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                        IVRLogDataAccess.AddToIVRLogDataAccess(log);
                        return output;
                    }
                }

                var updateResult = HandleUpdateNationalAddress(driverData);
                if (updateResult.ErrorCode != IVRTicketOutput<bool>.ErrorCodes.Success)
                {
                    output.ErrorCode = updateResult.ErrorCode;
                    output.ErrorDescription = updateResult.ErrorDescription;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.LogDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output.Result = true;
                output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        #region Shared Methods

        public void AddBasicLog(IVRServicesLog log, string methodName, IVRModuleEnum module)
        {
            log.Method = methodName;
            log.ModuleId = (int)module;
            log.ModuleName = module.ToString();
            log.CreatedDate = DateTime.Now;
            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
            log.RequesterUrl = Utilities.GetUrlReferrer();
        }

        private bool ValidateBeforeOpenTicket(int ticketTypeId, int? ticketSubTypeId, string vehicleId, string nin)
        {
            if (ticketTypeId == (int)IVRTicketTypeEnum.UpdateNationalAddress)
            {
                var notClosedTickets = _userTicketRepository
                                            .TableNoTracking
                                            .Where(t => t.CurrentTicketStatusId != (int)EUserTicketStatus.TicketClosed && t.TicketTypeId == ticketTypeId && t.DriverNin == nin).ToList();
                if (notClosedTickets != null && notClosedTickets.Count > 0)
                {
                    return false;
                }
            }
            else
            {
                if (ticketTypeId == (int)IVRTicketTypeEnum.ChangePolicyData) // || ticketTypeId == (int)IVRTicketTypeEnum.CouldNotPrintThePolicy
                {
                    var notClosedTickets = _userTicketRepository
                                            .TableNoTracking
                                            .Where(t => t.CurrentTicketStatusId != (int)EUserTicketStatus.TicketClosed && t.TicketTypeId == ticketTypeId && t.TicketSubTypeId == ticketSubTypeId
                                                        && (t.SequenceNumber == vehicleId || t.CustomCardNumber == vehicleId)).ToList();
                    if (notClosedTickets != null && notClosedTickets.Count > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    var notClosedTickets = _userTicketRepository
                                            .TableNoTracking
                                            .Where(t => t.CurrentTicketStatusId != (int)EUserTicketStatus.TicketClosed && t.TicketTypeId == ticketTypeId && (t.SequenceNumber == vehicleId || t.CustomCardNumber == vehicleId)).ToList();
                    if (notClosedTickets != null && notClosedTickets.Count > 0)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        private IVRTicketOutput<NewTicketResponseModel> ValidateVehiclePolicy(NewTicketModel ticketModel)
        {
            IVRTicketOutput<NewTicketResponseModel> output = new IVRTicketOutput<NewTicketResponseModel>() { ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.Success };
            try
            {
                var exception = string.Empty;
                IVRTicketPolicyDetails ticketPolicyDetails = _iIVRTicketService.GetLastPolicyBySequenceOrCustomCardNumber(ticketModel.VehicleId, out exception);
                if (!string.IsNullOrEmpty(exception) || ticketPolicyDetails == null)
                {
                    var logException = $"_iIVRTicketService.GetLastPolicyBySequenceOrCustomCardNumber return: {(!string.IsNullOrEmpty(exception) ? exception : "null result")}";
                    output.ErrorCode = !string.IsNullOrEmpty(exception) ? IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.ServiceException : IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = !string.IsNullOrEmpty(exception) ? "Error happend while checking for vehicle policy in DB" : $"No policy exist for the provided vehicleId {ticketModel.VehicleId}";
                    output.LogDescription = logException;
                    return output;
                }
                if (ticketPolicyDetails.PolicyExpiryDate < DateTime.Now)
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.PolicyExpired;
                    output.ErrorDescription = "User poilicy is expired";
                    output.LogDescription = $"User poilicy is expired, as ticketPolicyDetails.PolicyExpiryDate = {ticketPolicyDetails.PolicyExpiryDate}";
                    return output;
                }
                if (string.IsNullOrEmpty(ticketModel.PhoneNumber))
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.EmptyInputParamter;
                    output.ErrorDescription = "Caller phone is empty";
                    output.LogDescription = $"Caller phone is empty, as value is: = {ticketModel.PhoneNumber}";
                    return output;
                }
                if (!ticketModel.SendToCallerPhone && Utilities.GetPurePhoneNumber(ticketPolicyDetails.CheckoutDetailPhone) != Utilities.GetPurePhoneNumber(ticketModel.PhoneNumber))
                {
                    output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.CallerPhoneIsNotSameAsPolicyIssuer;
                    output.ErrorDescription = "Caller phone is not same as policy issuer";
                    output.LogDescription = $"Caller phone is not same as policy issuer, as ticketPolicyDetails.CheckoutDetailPhone = {ticketPolicyDetails.CheckoutDetailPhone} and caller = {Utilities.ValidatePhoneNumber(ticketModel.PhoneNumber)}";
                    return output;
                }

                output.PolicyData = ticketPolicyDetails;
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRTicketOutput<NewTicketResponseModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Error happend while checking for vehicle policy in DB";
                output.LogDescription = $"ValidateVehiclePolicy Exception, and error is: {ex.ToString()}";
                return output;
            }
        }

        private NewTicketResponseModel HandleCreateNewTicket(NewTicketModel ticketModel, IVRTicketPolicyDetails ticketPolicyDetails, out string exception)
        {
            exception = string.Empty;
            NewTicketResponseModel output = new NewTicketResponseModel() { IsSuccess = false };

            try
            {
                string phoneToSendSMS = ticketModel.SendToCallerPhone ? ticketModel.PhoneNumber : ticketPolicyDetails.CheckoutDetailPhone;
                UserTicket userTicket = new UserTicket()
                {
                    UserId = ticketPolicyDetails.CheckoutUserId,
                    TicketTypeId = ticketModel.TicketTypeId,
                    DriverNin = ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.UpdateNationalAddress ? ticketModel.NationalId : null,
                    SequenceNumber = ticketModel.VehicleId,
                    CustomCardNumber = ticketModel.VehicleId,
                    CreatedDate = DateTime.Now,
                    TicketModuleId = (int)TicketModuleEnum.IVR,
                    TicketSubTypeId = (ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.ChangePolicyData) ? ticketModel.TicketSubTypeId : null,
                    //SadadNo = (ticketModel.TicketTypeId == (int)IVRTicketTypeEnum.CouldNotPrintThePolicy && ticketModel.TicketSubTypeId == (int)IVRTicketSubTypeEnum.PolicyNotExistWithSada) ? ticketModel.SadadNo : null,
                    CurrentTicketStatusId = (int)EUserTicketStatus.TicketOpened,
                    Channel = "IVR",
                    CheckedoutPhone = phoneToSendSMS,
                    UserPhone = phoneToSendSMS
                };

                if (ticketModel.TicketTypeId != (int)IVRTicketTypeEnum.Others || ticketModel.TicketTypeId != (int)IVRTicketTypeEnum.UpdateNationalAddress)
                {
                    userTicket.PolicyId = ticketPolicyDetails.PolicyId;
                    userTicket.PolicyNo = ticketPolicyDetails.PolicyNo;
                    userTicket.ReferenceId = ticketPolicyDetails.ReferenceId;
                    userTicket.CheckedoutEmail = ticketPolicyDetails.CheckoutDetailEmail;
                    userTicket.VehicleId = ticketPolicyDetails.VehicleID;
                    userTicket.SequenceNumber = ticketPolicyDetails.SequenceNumber;
                    userTicket.CustomCardNumber = ticketPolicyDetails.CustomCardNumber;
                    userTicket.InvoiceId = ticketPolicyDetails.InvoiceId;
                    userTicket.InvoiceNo = ticketPolicyDetails.InvoiceNo;
                }

                _userTicketRepository.Insert(userTicket);

                UserTicketHistory tickethistory = new UserTicketHistory()
                {
                    TicketId = userTicket.Id,
                    TicketStatusId = (int)EUserTicketStatus.TicketOpened,
                    CreatedDate = DateTime.Now
                };
                _userTicketHistoryRepository.Insert(tickethistory);

                SendSMS(phoneToSendSMS, userTicket.Id, out exception);
                if (!string.IsNullOrEmpty(exception))
                    return output;

                output.IsSuccess = true;
                output.TicketNo = userTicket.Id;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }

            return output;
        }

        private bool SendSMS(string phone, int userTicketId, out string exception)
        {
            exception = String.Empty;
            try
            {
                string body = string.Format(UserTicketResources.ResourceManager.GetString("UserTicketCreated", CultureInfo.GetCultureInfo("ar")), userTicketId.ToString());
                _iIVRTicketService.SendSMS(phone, body, SMSMethod.UserTicketWebsite, out exception);
                return true;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\IVR_SendSMS_Exception.txt", ex.ToString());
                exception = ex.ToString();
                return false;
            }
        }

        private bool SendWhatsAppMessage(string phone, string referenceId, int lang, out string exception)
        {
            exception = String.Empty;
            try
            {
                string smsBody = string.Empty;
                string emo = DecodeEncodedNonAsciiCharacters("\uD83E\uDD73");
                string policyFileUrl = "https://bcare.com.sa/Identityapi/api/u/p?r=" + referenceId;
                string shortUrl = Utilities.GetShortUrl(policyFileUrl);
                if (!string.IsNullOrEmpty(shortUrl))
                    policyFileUrl = shortUrl;

                if (lang == (int)LanguageTwoLetterIsoCode.Ar)
                {
                    smsBody += "وثيقتك جاهزة" + "!" + " " + emo;
                    smsBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    smsBody += policyFileUrl;
                    smsBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    smsBody += "بي كير تهتم.";
                }
                else
                {
                    smsBody = "Policy is ready! " + emo + " ";
                    smsBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    smsBody += policyFileUrl;
                    smsBody += DecodeEncodedNonAsciiCharacters("\u000d");
                    smsBody += "BCare cares.";
                }

                //_notificationService.SendWhatsAppMessageAsync(phone, smsBody, SMSMethod.PolicyFile.ToString(), referenceId, Enum.GetName(typeof(LanguageTwoLetterIsoCode), lang.ToString()).ToLower());
                _iIVRTicketService.SendSMS(phone, smsBody, SMSMethod.PolicyFile, out exception);
                if (!string.IsNullOrEmpty(exception))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        private IVRTicketOutput<bool> HandleUpdateNationalAddress(Driver driverData)
        {
            IVRTicketOutput<bool> output = new IVRTicketOutput<bool>();
            output.Result = false;

            try
            {
                string birthDate = string.Empty;
                if (driverData.IsCitizen)
                {
                    var splitedBirthDate = driverData.DateOfBirthH.Split('-');
                    birthDate = string.Format("{0}-{1}", splitedBirthDate[1], splitedBirthDate[2]);
                }
                else
                    birthDate = string.Format("{0}-{1}", driverData.DateOfBirthG.Month.ToString("00"), driverData.DateOfBirthG.Year);

                var updatedAddress = _yakeenClient.GetYakeenAddress("0", driverData.NIN, birthDate, "A", driverData.IsCitizen, "IVR", "0", "0");
                if (updatedAddress.ErrorCode == YakeenAddressOutput.ErrorCodes.NullResponse || updatedAddress.ErrorCode == YakeenAddressOutput.ErrorCodes.NoAddressFound)
                {
                    output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = "No address found in yakeen for this nationalId";
                    output.LogDescription = $"No address found in yakeen for this nationalId as Yakeen return error code {updatedAddress.ErrorCode}";
                    return output;
                }
                if (updatedAddress.ErrorCode != YakeenAddressOutput.ErrorCodes.Success)
                {
                    output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.ServiceException;
                    output.ErrorDescription = "No address found in yakeen for this nationalId";
                    output.LogDescription = $"error happend while getting addresses from yakeen and the error is {updatedAddress.ErrorDescription}";
                    return output;
                }

                List<Address> allAddresses = new List<Address>();
                string zipcodes = string.Empty;
                foreach (var addressInfo in updatedAddress.Addresses)
                {
                    zipcodes += addressInfo.PostCode + ",";
                    var yakeenCityCenter = _addressService.GetYakeenCityCenterByZipCode(addressInfo.PostCode);
                    if (yakeenCityCenter == null)
                        continue;
                    Address address = new Address();
                    address.AdditionalNumber = addressInfo.AdditionalNumber.ToString();
                    address.BuildingNumber = addressInfo.BuildingNumber.ToString();
                    address.CityId = yakeenCityCenter.CityID.ToString();
                    address.City = addressInfo.City;
                    address.District = addressInfo.District;
                    address.DriverId = driverData.DriverId;
                    address.IsPrimaryAddress = addressInfo.IsPrimaryAddress.ToString();
                    if (addressInfo.LocationCoordinates.Split(' ').Count() == 2)
                    {
                        address.Latitude = addressInfo.LocationCoordinates.Split(' ')[0];
                        address.Longitude = addressInfo.LocationCoordinates.Split(' ')[1];
                    }
                    address.ObjLatLng = addressInfo.LocationCoordinates;
                    address.PostCode = addressInfo.PostCode.ToString();
                    address.RegionId = yakeenCityCenter.RegionID.ToString();
                    address.RegionName = yakeenCityCenter.RegionArabicName;
                    address.Street = addressInfo.StreetName;
                    address.UnitNumber = addressInfo.UnitNumber.ToString();
                    address.NationalId = driverData.NIN;
                    address.CreatedDate = DateTime.Now;
                    allAddresses.Add(address);
                }

                if (updatedAddress.Addresses.Count() > 0 && allAddresses.Count == 0)
                {
                    output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.NoAddressFound;
                    output.ErrorDescription = "Error happend while update nationa address";
                    output.LogDescription = "Error happend while update nationa address, and error is {updatedAddress.Addresses.Count() > 0 && allAddresses.Count == 0}";
                    return output;
                }

                var oldAddresses = _addressService.GetAllAddressesByNationalId(driverData.NIN).Where(a => !a.IsDeleted.Value || !a.IsDeleted.HasValue);
                if (oldAddresses != null)
                {
                    foreach (var address in oldAddresses)
                    {
                        address.IsDeleted = true;
                        _addressService.UpdateAddress(address);
                    }
                }
                _addressService.InsertAddresses(allAddresses);

                InsuredAddressesCount insuredAddressesCount = new InsuredAddressesCount();
                insuredAddressesCount.NationalId = driverData.NIN;
                insuredAddressesCount.YakeenAddressesCount = allAddresses.Count;
                insuredAddressesCount.CreatedDate = DateTime.Now;
                _insuredAddressesCountRepository.Insert(insuredAddressesCount);

                output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRTicketOutput<bool>.ErrorCodes.ServiceException;
                output.ErrorDescription = $"Error happend while update nationa address";
                output.LogDescription = $"Exception reror happend while update nationa address, and error is {ex.ToString()}";
                return output;
            }
        }

        private string DecodeEncodedNonAsciiCharacters(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

        #endregion
    }
}
