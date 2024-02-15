using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Core;
using Tameenk.Services.Core.IVR;
using Tameenk.Services.Core.Notifications;

namespace Tameenk.Services.Implementation.IVR
{
    public class IVRService : IIVRService
    {
        private INotificationService _notificationService;

        public IVRService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IVRTicketPolicyDetails GetLastPolicyBySequenceOrCustomCardNumber(string sequenceOrCustomCardNumber, out string exception)
        {
            exception = string.Empty;
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "IVRGetLastPolicyBySequenceOrCustomCardNumber";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter() { ParameterName = "@sequenceOrCustomCardNumber", Value = sequenceOrCustomCardNumber });

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                IVRTicketPolicyDetails userPolicyDetails = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<IVRTicketPolicyDetails>(reader).FirstOrDefault();
                return userPolicyDetails;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }
        }

        public RenewalPolicyDriversDataModel GetLastPolicyDriversCheckoutuserIdAndPolicyNo(string checkoutuserId, string policyNo, out string exception)
        {
            exception = String.Empty;
            RenewalPolicyDriversDataModel driversDataModel = null;
            IDbContext idbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                idbContext.DatabaseInstance.CommandTimeout = 60;
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetIVRRenewalePolicyDriversCheckoutuserIdAndPolicyNo";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter() { ParameterName = "@userId", Value = checkoutuserId });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@policyNumber", Value = policyNo });

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                driversDataModel = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<RenewalPolicyDriversDataModel>(reader).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            finally
            {
                if (idbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    idbContext.DatabaseInstance.Connection.Close();
            }

            return driversDataModel;
        }

        public bool SendSMS(string phone, string body, SMSMethod smsMethod, out string exception)
        {
            exception = String.Empty;
            try
            {
                var smsModel = new SMSModel()
                {
                    PhoneNumber = phone,
                    MessageBody = body,
                    Module = Module.Vehicle.ToString(),
                    Method = smsMethod.ToString(),
                    Channel = "IVR"
                };

                _notificationService.SendSmsBySMSProviderSettings(smsModel);
                return true;
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\IVR_SendSMS_Exception.txt", ex.ToString());
                exception = ex.ToString();
                return false;
            }
        }
    }
}
