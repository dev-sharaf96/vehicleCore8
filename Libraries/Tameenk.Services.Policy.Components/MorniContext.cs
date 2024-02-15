using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Integration.Providers.Wataniya.Dtos;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Notifications;
using Tameenk.Services.Policy.Components.Morni;
using static Tameenk.Common.Utilities.Utilities;

namespace Tameenk.Services.Policy.Components
{
    public class MorniContext : IMorniContext
    {
        private readonly IHttpClient _httpClient;
        private readonly IRepository<MorniRequest> _morniRepository;
        public MorniContext(IRepository<MorniRequest> morniRepository)
        {
            _morniRepository = morniRepository;
            _httpClient = EngineContext.Current.Resolve<IHttpClient>();

        }
        public MorniMembershipOutput CreateMembership(string refrenceId, string channel)
        {
            MorniMembershipOutput output = new MorniMembershipOutput();
            ServiceRequestLog log = new ServiceRequestLog();
            log.Channel = channel;
            log.ServerIP = Utilities.GetInternalServerIP();
            log.ReferenceId = refrenceId;
            log.Method = "Morni";
            log.ServiceURL = "https://interface.morniksa.com/v1/memberships";
            MorniRequest morniRequest = new MorniRequest();
            try
            {
                var input = GetMorniRequest(refrenceId);
                if (input == null||input.membership==null||string.IsNullOrEmpty(input.membership.PolicyNumber))
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.RequestIsNull;
                    output.ErrorDescription = "Request Is Null";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Request Is Null";
                    log.ServiceResponse = null;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceRequest = JsonConvert.SerializeObject(input);
                
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "Authorization", "Apikey 066430f4-6575-4842-979f-9bcfc816d749" }
                };
              
                var postTask = _httpClient.PostAsync(log.ServiceURL, input, headers: headers);
                postTask.Wait();
                if (postTask == null)
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = "Failure null response";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failure null response";
                    log.ServiceResponse = null;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = postTask.Result.ToString();

                if (string.IsNullOrEmpty(postTask.Result.ToString()))
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = "Failure response is empty";
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = "Failure response is empty";
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                log.ServiceResponse = postTask.Result.Content.ReadAsStringAsync().Result;
                if ((int)postTask.Result.StatusCode == 503)
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = "Status Code is not Ok as we got " + postTask.Result.StatusCode.ToString();
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var response = JsonConvert.DeserializeObject<MorniOutput>(postTask.Result.Content.ReadAsStringAsync().Result);
                if (response.Code == "E0013")
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = response.Message;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = response.Code;
                    log.ServiceErrorDescription = response.Message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Code == "E0021")
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = response.Message;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = response.Code;
                    log.ServiceErrorDescription = response.Message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Code == "E0022")
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = response.Message;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = response.Code;
                    log.ServiceErrorDescription = response.Message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Code == "E0030")
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = response.Message;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = response.Code;
                    log.ServiceErrorDescription = response.Message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Code == "E0034")
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = response.Message;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = response.Code;
                    log.ServiceErrorDescription = response.Message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                if (response.Code == "E0037")
                {
                    output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                    output.ErrorDescription = response.Message;
                    log.ErrorCode = (int)output.ErrorCode;
                    log.ErrorDescription = output.ErrorDescription;
                    log.ServiceErrorCode = response.Code;
                    log.ServiceErrorDescription = response.Message;
                    ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                    return output;
                }
                var info =_morniRepository.TableNoTracking.Where(a=>a.RefrenceId==refrenceId).FirstOrDefault();
                if (info == null)
                {

                    //User Object
                    morniRequest.RefrenceId = refrenceId;
                    morniRequest.FirstName = input.user.FirstName;
                    morniRequest.LastName = input.user.LastName;
                    morniRequest.NationalId = input.user.NationalId;
                    morniRequest.PhoneNumber = input.user.PhoneNumber;

                    //vehicle object
                    morniRequest.Make = input.vehicle.Make;
                    morniRequest.Model = input.vehicle.Model;
                    morniRequest.Year = input.vehicle.Year;
                    morniRequest.PlateNumber = input.vehicle.PlateNumber;
                    morniRequest.PlateFirstLetterId = input.vehicle.PlateFirstLetterId;
                    morniRequest.PlateSecondLetterId = input.vehicle.PlateSecondLetterId;
                    morniRequest.PlateThirdLetterId = input.vehicle.PlateThirdLetterId;
                    morniRequest.Color = input.vehicle.Color;
                    morniRequest.VIN = input.vehicle.VIN;
                    morniRequest.SequenceNumber = input.vehicle.SequenceNumber;
                    morniRequest.CustomsNumber = input.vehicle.CustomsNumber;

                    //membership object
                    morniRequest.PlanReferenceNumber = input.membership.PlanReferenceNumber;
                    morniRequest.PolicyNumber = input.membership.PolicyNumber;
                    if (input.membership.EffectiveDate != null && input.membership.EffectiveDate.ToString() != "0001-01-01T00:00:00")
                    {
                        morniRequest.PolicyEffectiveDate =new DateTime(input.membership.EffectiveDate.Year, input.membership.EffectiveDate.Month, input.membership.EffectiveDate.Day);
                    }
                    if (input.membership.ExpiryDate != null && input.membership.ExpiryDate.ToString() != "0001-01-01T00:00:00")
                    {
                        morniRequest.PolicyExpiryDate =new DateTime(input.membership.ExpiryDate.Year, input.membership.ExpiryDate.Month, input.membership.ExpiryDate.Day);
                    }
                    morniRequest.PolicyType = input.membership.PolicyType;
                    DateTime dt = DateTime.Now;
                    morniRequest.CreatedDate = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                    try
                    {
                        _morniRepository.Insert(morniRequest);
                    }
                    catch
                    {
                        morniRequest.PolicyEffectiveDate = null;
                        morniRequest.PolicyExpiryDate = null;
                       _morniRepository.Insert(morniRequest);
                    }
                }
                output.ErrorCode = MorniMembershipOutput.StatusCode.Success;
                output.ErrorDescription = "Succes";
                log.ErrorCode = (int)output.ErrorCode;
                log.ErrorDescription = "Succes";
                log.ServiceResponse = postTask.Result.Content.ReadAsStringAsync().Result;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (DbEntityValidationException ex)
            {
                output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)MorniMembershipOutput.StatusCode.Failure;
                log.ErrorDescription = ex.ToString() + " morniRequest:" + JsonConvert.SerializeObject(morniRequest);
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
            catch (Exception ex)
            {
                output.ErrorCode = MorniMembershipOutput.StatusCode.Failure;
                output.ErrorDescription = ex.ToString();
                log.ErrorCode = (int)MorniMembershipOutput.StatusCode.Failure;
                log.ErrorDescription = ex.ToString()+ " morniRequest table:" + JsonConvert.SerializeObject(morniRequest) ;
                ServiceRequestLogDataAccess.AddtoServiceRequestLogs(log);
                return output;
            }
        }

        public MorniModel GetMorniRequest(string refrenceId)
        {
            var morniModel = new MorniModel();
            try
            {
                IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
                idbContext.DatabaseInstance.CommandTimeout = 240;
                SqlConnection connection = (SqlConnection)idbContext.DatabaseInstance.Connection;
                SqlCommand sqlCommand = new SqlCommand("GetPolicyInformationForMorni", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter parameter = new SqlParameter("@refrenceId", SqlDbType.NVarChar);
                parameter.Value = refrenceId;
                sqlCommand.Parameters.Add(parameter);
                idbContext.DatabaseInstance.Connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                    //User
                    morniModel.user.FirstName = reader["FirstName"].ToString();
                    morniModel.user.LastName = reader["LastName"].ToString();
                    morniModel.user.NationalId = reader["NationalId"].ToString();
                    morniModel.user.PhoneNumber = reader["PhoneNumber"].ToString();
                    //Vehicle
                    morniModel.vehicle.Color = reader["Color"].ToString();
                    morniModel.vehicle.CustomsNumber = string.IsNullOrEmpty(reader["CustomsNumber"].ToString()) ? "" : reader["CustomsNumber"].ToString();
                    morniModel.vehicle.Make = reader["Make"].ToString();
                    morniModel.vehicle.Model = reader["Model"].ToString();
                    morniModel.vehicle.PlateFirstLetterId = string.IsNullOrEmpty(reader["PlateFirstLetterId"].ToString()) ? "" : reader["PlateFirstLetterId"].ToString();
                    morniModel.vehicle.PlateNumber = string.IsNullOrEmpty(reader["PlateNumber"].ToString()) ? "" : reader["PlateNumber"].ToString();
                    morniModel.vehicle.PlateSecondLetterId = string.IsNullOrEmpty(reader["PlateSecondLetterId"].ToString()) ? "" : reader["PlateSecondLetterId"].ToString();
                    morniModel.vehicle.PlateThirdLetterId = string.IsNullOrEmpty(reader["PlateThirdLetterId"].ToString()) ? "" : reader["PlateThirdLetterId"].ToString();
                    morniModel.vehicle.SequenceNumber = string.IsNullOrEmpty(reader["SequenceNumber"].ToString()) ? "" : reader["SequenceNumber"].ToString();
                    morniModel.vehicle.VIN = reader["VIN"].ToString();
                    morniModel.vehicle.Year = string.IsNullOrEmpty(reader["Year"].ToString()) ? "" : reader["Year"].ToString();
                    //Membership
                    DateTime effectiveDate;
                    if(reader["EffectiveDate"].ToString() != "0001-01-01T00:00:00" && DateTime.TryParse(reader["EffectiveDate"].ToString(), out effectiveDate))
                    {
                        morniModel.membership.EffectiveDate = DateTime.Parse(reader["EffectiveDate"].ToString());
                    }
                    DateTime expiryDate;
                    if (reader["ExpiryDate"].ToString()!="0001-01-01T00:00:00"&&DateTime.TryParse(reader["ExpiryDate"].ToString(), out expiryDate))
                    {
                        morniModel.membership.ExpiryDate = expiryDate;
                    }
                    morniModel.membership.PlanReferenceNumber = reader["PlanReferenceNumber"].ToString();
                    morniModel.membership.PolicyNumber = reader["PolicyNumber"].ToString();
                    morniModel.membership.PolicyType = reader["PolicyType"].ToString();

                }
                idbContext.DatabaseInstance.Connection.Close();
                return morniModel;

            }
            catch (Exception ex)
            {
                morniModel=null;
                throw ex;
            }
        }


        public bool CreateMorniMembership(string refrenceId, string channel)
        {
            MorniMembershipOutput morniMembershipOutput = CreateMembership(refrenceId, channel);
            if (morniMembershipOutput.ErrorCode != MorniMembershipOutput.StatusCode.Success)
                return false;
            else
                return true;

        }

    }
}
