using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;
using Tameenk.Services.Core.IVR;

namespace Tameenk.IVR.Component
{
    public class IVRInquiryContext : IIVRInquiryContext
    {
        private readonly IRepository<Driver> _driverRepository;
        private IIVRService _iIVRTicketService;
        private readonly IRepository<Vehicle> _vehicleRepository;

        public IVRInquiryContext(IRepository<Driver> driverRepository, IIVRService iIVRTicketService, IRepository<Vehicle> vehicleRepository)
        {
            _driverRepository = driverRepository;
            _iIVRTicketService = iIVRTicketService;
            _vehicleRepository = vehicleRepository;
        }

        public IVRInquiryOutput<UserModel> GetUserDetails(string nationalId, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRInquiryOutput<UserModel> output = new IVRInquiryOutput<UserModel>();
            output.Result = null;

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = $"nationalId: {nationalId}";
            AddBasicLog(log, methodName, IVRModuleEnum.Inquiry);

            try
            {
                if (string.IsNullOrEmpty(nationalId))
                {
                    output.ErrorCode = IVRInquiryOutput<UserModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "nationalId is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var userData = _driverRepository.TableNoTracking.Where(a => a.NIN == nationalId && !a.IsDeleted).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();
                if (userData == null)
                {
                    output.ErrorCode = IVRInquiryOutput<UserModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "There is no data in DB with the provided nationalId: " + nationalId + " or the user is deleted";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var arrayNameAr = new[] { userData.FirstName, userData.SecondName, userData.ThirdName, userData.LastName };
                var arrayNameEn = new[] { userData.EnglishFirstName, userData.EnglishSecondName, userData.EnglishThirdName, userData.EnglishLastName };
                output.Result = new UserModel()
                {
                    NationalId = userData.NIN,
                    NameAr = string.Join(" ", arrayNameAr.Where(s => !string.IsNullOrEmpty(s))),
                    NameEn = string.Join(" ", arrayNameEn.Where(s => !string.IsNullOrEmpty(s)))
                };

                output.ErrorCode = IVRInquiryOutput<UserModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRInquiryOutput<UserModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        public IVRInquiryOutput<VehicleDataModel> GetVehicleDetails(string vehicleId, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRInquiryOutput<VehicleDataModel> output = new IVRInquiryOutput<VehicleDataModel>();
            output.Result = new VehicleDataModel() { IsExist = false };

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = $"vehicleId: {vehicleId}";
            AddBasicLog(log, methodName, IVRModuleEnum.Inquiry);

            try
            {
                if (string.IsNullOrEmpty(vehicleId))
                {
                    output.ErrorCode = IVRInquiryOutput<VehicleDataModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "vehicleId is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                var vehicleData = _vehicleRepository.TableNoTracking.Where(a => a.SequenceNumber == vehicleId && !a.IsDeleted).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();
                if (vehicleData == null)
                    vehicleData = _vehicleRepository.TableNoTracking.Where(a => a.CustomCardNumber == vehicleId && !a.IsDeleted).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();

                if (vehicleData == null)
                {
                    output.ErrorCode = IVRInquiryOutput<VehicleDataModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "There is no data in DB related to the provided vehicleId: " + vehicleId + " or the vehicle is deleted";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output.Result.IsExist = true;
                output.ErrorCode = IVRInquiryOutput<VehicleDataModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRInquiryOutput<VehicleDataModel>.ErrorCodes.ServiceException;
                output.ErrorDescription = "Processing Exception error";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = $"Processing ExceptionError, and error is: {ex.ToString()}";
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
        }

        public IVRInquiryOutput<CheckIfPolicyExistResponseModel> CheckIfPolicyExist(string vehicleId, string methodName)
        {
            DateTime dtBeforeCalling = DateTime.Now;
            IVRInquiryOutput<CheckIfPolicyExistResponseModel> output = new IVRInquiryOutput<CheckIfPolicyExistResponseModel>();
            output.Result = new CheckIfPolicyExistResponseModel() { IsExist = false };

            IVRServicesLog log = new IVRServicesLog();
            log.ServiceRequest = $"vehicleId: {vehicleId}";
            AddBasicLog(log, methodName, IVRModuleEnum.Inquiry);

            try
            {
                if (string.IsNullOrEmpty(vehicleId))
                {
                    output.ErrorCode = IVRInquiryOutput<CheckIfPolicyExistResponseModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "vehicleId is null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                string exception;
                IVRTicketPolicyDetails ticketPolicyDetails = _iIVRTicketService.GetLastPolicyBySequenceOrCustomCardNumber(vehicleId, out exception);
                if (!string.IsNullOrEmpty(exception) || ticketPolicyDetails == null)
                {
                    output.ErrorCode = IVRInquiryOutput<CheckIfPolicyExistResponseModel>.ErrorCodes.NotFound;
                    output.ErrorDescription = "There is no active policy in DB related to the provided vehicleId: " + vehicleId;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                    IVRLogDataAccess.AddToIVRLogDataAccess(log);
                    return output;
                }

                output.Result.IsExist = true;
                output.ErrorCode = IVRInquiryOutput<CheckIfPolicyExistResponseModel>.ErrorCodes.Success;
                output.ErrorDescription = "Success";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = output.ErrorDescription;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                IVRLogDataAccess.AddToIVRLogDataAccess(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = IVRInquiryOutput<CheckIfPolicyExistResponseModel>.ErrorCodes.ServiceException;
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

        #endregion
    }
}
