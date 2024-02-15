using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Tameenk.Common.Utilities;

namespace Tameenk.Loggin.DAL
{
    public class PolicyRequestLogDataAccess
    {
        public static bool AddtoPolicyRequestLogs(PolicyRequestLog toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    context.PolicyRequestLogs.Add(toSaveLog);
                    return true;
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {

                    }
                }
                return false;
            }
        }

        public static List<PolicyRequestLog> GetPolicyList(int commandTimeout)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = commandTimeout;
                    DateTime startDate = DateTime.Now.Date.AddDays(-1).AddHours(0).AddMinutes(0).AddSeconds(0);// new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 0, 0, 0);
                    DateTime endDate = DateTime.Now.Date.Date.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); //new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 23, 59, 59);

                    var policies = (from d in context.PolicyRequestLogs
                                    where d.CreatedDate >= startDate
                                    && d.CreatedDate <= endDate

                                    orderby d.CreatedDate
                                    select d);

                    List<PolicyRequestLog> policyList = policies.ToList<PolicyRequestLog>();

                    if (policyList.Count > 0)
                        return policyList;
                    else
                        return null;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;

            }
        }

    }
}
