using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Tameenk.Core.Domain;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Redis;
using Tameenk.Resources.WebResources;

namespace Tameenk.Services.QuotationDependancy.Component
{
    public class VehicleServices : IVehicleServices
    {
        private const string quotationRequestCach_Base_KEY = "QuOtAtIoN_rEqUeSt_cAcH";
        private const int cach_TiMe = 4 * 60 * 60;

        public async Task<QuotationDependancyOutput<CarInfoResponseModel>> GetVehicleInfo(string qtRqstExtrnlId, string lang = "ar")
        {
            QuotationRequestLog log = new QuotationRequestLog();
            log.CreatedDate = DateTime.Now;
            log.ExtrnlId = qtRqstExtrnlId;
            if (string.IsNullOrEmpty(qtRqstExtrnlId))
                return HandleErrorLog(log, 2, "qtRqstExtrnlId is empty", "ErrorGeneric", lang);

            QuotationDependancyOutput<CarInfoResponseModel> output = null;
            CarInfoResponseModel response = null;

            string exception = string.Empty;
            var benchMark = DateTime.Now.AddHours(-16);

            #region Check Caching

            QuotationRequestDetailsCachingModel quotationRequestDetails = await RedisCacheManager.Instance.GetAsync<QuotationRequestDetailsCachingModel>($"{quotationRequestCach_Base_KEY}_{qtRqstExtrnlId}");
            if (quotationRequestDetails != null && quotationRequestDetails.VehicleInfo != null)
            {
                if (benchMark > quotationRequestDetails.VehicleInfo.QuotationCreatedDate)
                    return HandleErrorLog(log, 4, $"Quotation is expired as data.QuotationCreatedDate = {quotationRequestDetails.VehicleInfo.QuotationCreatedDate}", "quotations_is_expired", lang);

                response = HandleResponse(quotationRequestDetails.VehicleInfo, out exception);
                if (!string.IsNullOrEmpty(exception))
                    return HandleErrorLog(log, 3, $"HandleResponse return: {exception}", "ErrorGeneric", lang);

                output = new QuotationDependancyOutput<CarInfoResponseModel>()
                {
                    ErrorCode = QuotationDependancyOutput<CarInfoResponseModel>.ErrorCodes.Success,
                    ErrorDescription = "Success",
                    Result = response
                };

                ////
                /// here will update the redis with tries count
                var userPartialLock = ValidateIfUserPartiallyLocked(quotationRequestDetails.UserPartialLock, lang);
                if (userPartialLock != null)
                {
                    if (userPartialLock.IsLocked)
                        return HandleErrorLog(log, 4, $"user is partially locked as isLock is: {userPartialLock.IsLocked}, and tries times is: {userPartialLock.ErrorTimesUserTries}", "QuotationUserPartialLock", lang);

                    var remainingTime = GetRemainingExpirationSeconds(quotationRequestDetails.ExpirationDate);
                    quotationRequestDetails.UserPartialLock = userPartialLock;
                    await RedisCacheManager.Instance.UpdateAsync($"{quotationRequestCach_Base_KEY}_{qtRqstExtrnlId}", quotationRequestDetails, (remainingTime > 0 ? remainingTime : cach_TiMe));
                }

                return output;
            }

            #endregion

            #region Vehicle Data

            var vehicleInfo = GetQuotationRequestAndVehicleInfo(qtRqstExtrnlId, out exception);
            if (vehicleInfo == null || !string.IsNullOrEmpty(exception))
                return HandleErrorLog(log, 3, $"GetQuotationRequestAndVehicleInfo return: {exception} or return null", "ErrorGeneric", lang);

            if (benchMark > vehicleInfo.QuotationCreatedDate)
                return HandleErrorLog(log, 4, $"Quotation is expired as data.QuotationCreatedDate = {vehicleInfo.QuotationCreatedDate}", "quotations_is_expired", lang);

            exception = string.Empty;
            response = HandleResponse(vehicleInfo, out exception);
            if (!string.IsNullOrEmpty(exception))
                return HandleErrorLog(log, 3, $"HandleResponse return: {exception}", "ErrorGeneric", lang);

            #endregion

            #region Quotation Data

            var requestDetails = GetQuotationRequestDetailsByExternalId(qtRqstExtrnlId, out exception);
            if (requestDetails == null || !string.IsNullOrEmpty(exception))
                return HandleErrorLog(log, 5, $"GetQuotationRequestDetailsByExternalId return: {exception} or return null", "ErrorGeneric", lang);

            #endregion

            #region Caching Data (VehicleInfo & Quotation Info)

            quotationRequestDetails = new QuotationRequestDetailsCachingModel()
            {
                VehicleInfo = vehicleInfo,
                QuotationDetails = requestDetails,
                UserPartialLock = ValidateIfUserPartiallyLocked(null, lang),
                ExpirationDate = DateTime.Now.AddHours(16)
            };
            await RedisCacheManager.Instance.SetAsync($"{quotationRequestCach_Base_KEY}_{qtRqstExtrnlId}", quotationRequestDetails, cach_TiMe);

            #endregion

            output = new QuotationDependancyOutput<CarInfoResponseModel>()
            {
                ErrorCode = QuotationDependancyOutput<CarInfoResponseModel>.ErrorCodes.Success,
                ErrorDescription = "Success",
                Result = response
            };

            return output;
        }

        private QuotationVehicleInfoModel GetQuotationRequestAndVehicleInfo(string externalId, out string exceptoion)
        {
            exceptoion = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationRequestVehicleInfo";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter externalIdParam = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(externalIdParam);
                
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                QuotationVehicleInfoModel responseModel = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationVehicleInfoModel>(reader).FirstOrDefault();
                return responseModel;
            }
            catch(Exception ex)
            {
                exceptoion = ex.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == System.Data.ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
        
        private CarInfoResponseModel HandleResponse(QuotationVehicleInfoModel VehicleIfo, out string exception)
        {
            try
            {
                exception = string.Empty;
                CarInfoResponseModel Result = new CarInfoResponseModel();
                //Result.TypeOfInsurance = typeOfInsurance;
                Result.QtRqstExtrnlId = VehicleIfo.ExternalId;
                Result.IsRenewal = (VehicleIfo.IsRenewal.HasValue && VehicleIfo.IsRenewal.Value) ? true : false;
                Result.RenewalReferenceId = VehicleIfo.PreviousReferenceId ?? null;
                Result.NCDFreeYearsEn = VehicleIfo.NCDFreeYearsEn;
                Result.NCDFreeYearsAr = VehicleIfo.NCDFreeYearsAr;
                //Result.NCDFreeYears = VehicleIfo.NCDFreeYearsEn;
                Result.Maker = VehicleIfo.VehicleMaker;
                Result.MakerCode = VehicleIfo.VehicleMakerCode.HasValue ? VehicleIfo.VehicleMakerCode.Value : default(short);
                Result.Model = VehicleIfo.VehicleModel;
                Result.ModelYear = VehicleIfo.ModelYear;
                Result.PlateTypeCode = VehicleIfo.PlateTypeCode;
                Result.FormatedMakerCode = VehicleIfo.VehicleMakerCode.HasValue ? VehicleIfo.VehicleMakerCode.Value.ToString("0000") : default(string);
                Result.CarPlate = new CarPlateInfo(VehicleIfo?.CarPlateText1, VehicleIfo?.CarPlateText2, VehicleIfo?.CarPlateText3, VehicleIfo.CarPlateNumber.HasValue ? VehicleIfo.CarPlateNumber.Value : 0);

                if (VehicleIfo.VehicleIdTypeId == 2 && !string.IsNullOrEmpty(VehicleIfo.CustomCardNumber))
                    Result.CustomCardNumber = VehicleIfo.CustomCardNumber;

                if (VehicleIfo.PlateTypeCode.HasValue)
                    Result.PlateColor = GetCarPlateColorByCode((int)VehicleIfo.PlateTypeCode);
                return Result;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }
        
        public string GetCarPlateColorByCode(int carPlateCode)
        {
            string resultColor = "";
            switch (carPlateCode)
            {
                case 1:
                case 10:
                    resultColor = "white";
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 7:
                    resultColor = "blue";
                    break;
                case 6:
                    resultColor = "yellow";
                    break;
                case 8:
                case 11:
                    resultColor = "black";
                    break;
                case 9:
                    resultColor = "green";
                    break;
            }

            return resultColor;
        }

        private QuotationDependancyOutput<CarInfoResponseModel> HandleErrorLog(QuotationRequestLog log, int errorCode, string logException, string outPutResourceKey, string lang)
        {
            log.ErrorCode = errorCode;
            log.ErrorDescription = $"HandleResponse return: {logException}";
            QuotationRequestLogDataAccess.AddQuotationRequestLog(log);
            return new QuotationDependancyOutput<CarInfoResponseModel>
            {
                ErrorCode = (QuotationDependancyOutput<CarInfoResponseModel>.ErrorCodes)errorCode,
                ErrorDescription = WebResources.ResourceManager.GetString(outPutResourceKey, CultureInfo.GetCultureInfo(lang)),
                Result = null
            };
        }


        #region Get QuotationDetails Before access GetQuote

        private QuotationRequestInfoModel GetQuotationRequestDetailsByExternalId(string externalId, out string exceptoion)
        {
            exceptoion = string.Empty;
            IDbContext dbContext = EngineContext.Current.Resolve<IDbContext>();
            try
            {
                dbContext.DatabaseInstance.CommandTimeout = 90;
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetQuotationRequestDetailsByExternalId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter nationalIDParameter = new SqlParameter() { ParameterName = "externalId", Value = externalId };
                command.Parameters.Add(nationalIDParameter);
                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                QuotationRequestInfoModel request = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<QuotationRequestInfoModel>(reader).FirstOrDefault();
                if (request == null)
                    return null;

                request.AdditionalDrivers = new List<Driver>();
                request.MainDriverViolation = new List<DriverViolation>();
                request.MainDriverLicenses = new List<DriverLicense>();
                reader.NextResult();
                request.AdditionalDrivers = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<Driver>(reader).ToList();
                reader.NextResult();
                request.MainDriverViolation = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverViolation>(reader).ToList();
                reader.NextResult();
                request.MainDriverLicenses = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<DriverLicense>(reader).ToList();

                return request;
            }
            catch (Exception exp)
            {
                exceptoion = exp.ToString();
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }

        #endregion

        #region Manage User Lock 10 times tries

        private UserPartialLockModel ValidateIfUserPartiallyLocked(UserPartialLockModel partialLock, string lang)
        {
            var sessionId = Guid.NewGuid().ToString();

            try
            {
                if (partialLock == null)
                    return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 1, "Success", $"partialLock == null till now", lang);

                if (partialLock.IsLocked || partialLock.LockDueDate >= DateTime.Now)
                    return partialLock;

                if (partialLock.ErrorTimesUserTries >= 10)
                    return HandleUserPartialLockReturnModel(partialLock.SessionId, true, DateTime.Now.AddMinutes(5), (partialLock.ErrorTimesUserTries + 1), "QuotationUserPartialLock", $"user is partially locked as error tries times is: {partialLock.ErrorTimesUserTries}", lang);

                return HandleUserPartialLockReturnModel(partialLock.SessionId, false, DateTime.Now, (partialLock.ErrorTimesUserTries + 1), "Success", $"user is not locked yet ad tries times is: {partialLock.ErrorTimesUserTries}", lang);
            }
            catch (Exception ex)
            {
                return HandleUserPartialLockReturnModel(sessionId, false, DateTime.Now, 1, "ErrorGeneric", $"Exception error when ValidateIfUserPartiallyLocked: {ex.ToString()}", lang);
            }
        }

        private UserPartialLockModel HandleUserPartialLockReturnModel(string sessionId, bool isLocked, DateTime lockedDueDate, int errorTimesUserTries, string errorDescriptionKey, string logDescription, string lang)
        {
            UserPartialLockModel partialLock = new UserPartialLockModel()
            {
                LogDescription = logDescription,
                ErrorDescription = WebResources.ResourceManager.GetString(errorDescriptionKey, CultureInfo.GetCultureInfo(lang)),
                ErrorTimesUserTries = errorTimesUserTries,
                IsLocked = isLocked,
                LockDueDate = lockedDueDate,
                SessionId = sessionId,
            };
            return partialLock;
        }

        private int GetRemainingExpirationSeconds(DateTime expirationDate)
        {
            try
            {
                if (DateTime.Now >= expirationDate)
                    return 0;

                var _remainingSeconds = (expirationDate - DateTime.Now).TotalSeconds;
                int.TryParse(_remainingSeconds.ToString(), out int remainingSeconds);
                return remainingSeconds;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion
    }
}


