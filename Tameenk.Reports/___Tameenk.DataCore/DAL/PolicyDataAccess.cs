using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Data.DAL
{
   public class PolicyDataAccess
    {
        public static List<Policy> GetSuccessPolicyListForEachCompany(int commandTimeout,int CompnayId )
        {
            try
            {
                using (Tameenk context = new Tameenk())
                {
                    context.Database.CommandTimeout = commandTimeout;

                    DateTime startDate = DateTime.Now.Date.AddDays(-1).AddHours(0).AddMinutes(0).AddSeconds(0);// new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 0, 0, 0);
                    DateTime endDate = DateTime.Now.Date.Date.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); //new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 23, 59, 59);



                    var query = context.Policies.Where(p => p.PolicyIssueDate >= startDate
                      && p.PolicyIssueDate <= endDate && p.InsuranceCompanyID==CompnayId
                     && p.CheckoutDetail.PolicyStatu.Key== "Available" );

                    List<Policy> policyList = query.DistinctBy(q => q.CheckoutDetail.ReferenceId).ToList();

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
