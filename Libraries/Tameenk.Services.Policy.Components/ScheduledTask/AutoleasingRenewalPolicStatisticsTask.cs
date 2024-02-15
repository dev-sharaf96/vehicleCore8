using System;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using System.Collections.Generic;
using Tameenk.Data;
using Tameenk.Core.Infrastructure;
using System.Data;
using System.Data.Entity.Infrastructure;
using Tameenk.Services.Core.Quotations;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Tameenk.Services.Policy.Components
{
    public class AutoleasingRenewalPolicStatisticsTask : ITask
    {
        #region Fields
        private readonly IRepository<AutoleasingRenewalPolicyStatistics> _AutoleasingRenewalPolicyStatistics;        #endregion

        #region Ctor
        public AutoleasingRenewalPolicStatisticsTask(
             IRepository<AutoleasingRenewalPolicyStatistics> AutoleasingRenewalPolicyStatistics
)
        {
            _AutoleasingRenewalPolicyStatistics = AutoleasingRenewalPolicyStatistics;

        }
        #endregion

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            var data = GetFromAutoleasingRenewalStatistics();

            var smallLists = data.ToLookup(item => item.parentExternalId, item => item);
            if (data == null)
                return;
            try
            {

                foreach (var bucket in smallLists)
                {
                    int year = 1;
                    foreach (var item in bucket)
                    {
                        AutoleasingRenewalPolicyStatistics Renewedpolicy = new AutoleasingRenewalPolicyStatistics();
                        Renewedpolicy.CreateDate = item.CreatedDate;
                        Renewedpolicy.ParentExternalId = item.parentExternalId;
                        Renewedpolicy.ParentReferenceId = item.parentReferenceId;
                        Renewedpolicy.PaymentAmount = item.TotalPrice;
                        Renewedpolicy.ReferenceId = item.ReferenceId;
                        Renewedpolicy.ExternalId = item.ExternalId;
                        Renewedpolicy.SequenceNumber = item.SequenceNumber;
                        Renewedpolicy.UserId = item.UserId;
                        Renewedpolicy.Year = year;
                        _AutoleasingRenewalPolicyStatistics.Insert(Renewedpolicy);
                        UpdateAutoleasingRenewalStatitics(item.ReferenceId);
                        year += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\AutoleasingRenewalStatistics" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
                return ;
            }
        }
        private static bool UpdateAutoleasingRenewalStatitics(string referenceId)
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            string exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "updateRenewalStatiscs_old";
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter referenceIdParameter = new SqlParameter() { ParameterName = "@referenceId", Value = referenceId };
                command.Parameters.Add(referenceIdParameter);

                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetFromAutoleasingQuotationFormSettings" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                idbContext.DatabaseInstance.Connection.Close();
                return false;
            }
        }

        #endregion
        private static List<AutoleasingRenewalStatisticsModel> GetFromAutoleasingRenewalStatistics()
        {
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            string exception = string.Empty;
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "RenwalStatitics_OldTable";
                command.CommandType = CommandType.StoredProcedure;
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                var result = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<AutoleasingRenewalStatisticsModel>(reader).ToList();
                idbContext.DatabaseInstance.Connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\GetFromAutoleasingQuotationFormSettings" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + exception.ToString());
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }
        private class AutoleasingRenewalStatisticsModel
        {
            public int Id { get; set; }
            public string parentExternalId { get; set; }
            public string parentReferenceId { get; set; }
            public string ReferenceId { get; set; }
            public string ExternalId { get; set; }
            public string SequenceNumber { get; set; }
            public decimal? TotalPrice { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string UserId { get; set; }
            public bool? isDeleted { get; set; }
        }
    }
}
