using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Data.Model;

namespace Tameenk.Data.DAL
{
    public class PolicyProcessingQueueDataAccess
    {
        public static List<FailPolicy> GetFailPolicyList(int commandTimeout)
        {
            try
            {
                using (Tameenk context = new Tameenk())
                {
                    context.Database.CommandTimeout = commandTimeout;
                    DateTime startDate = DateTime.Now.Date.AddDays(-1).AddHours(0).AddMinutes(0).AddSeconds(0);// new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 0, 0, 0);
                    DateTime endDate = DateTime.Now.Date.Date.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); //new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 23, 59, 59);

                    DateTime dateTime = DateTime.Now;

                    ScheduleTask scheduleTask = context.ScheduleTasks.FirstOrDefault(
                        s => s.Type.Equals("Tameenk.Services.Implementation.Policies.PolicyProcessingTask, Tameenk.Services"));

                    //  && !(from policy in context.Policies select policy.CheckOutDetailsId).Contains(QueueProcessing.ReferenceId)

                    var query = from QueueProcessing in context.PolicyProcessingQueues
                                join CheckOutDetails in context.CheckoutDetails
                                on QueueProcessing.ReferenceId equals CheckOutDetails.ReferenceId
                                join invoice in context.Invoices
                                on QueueProcessing.ReferenceId equals invoice.ReferenceId
                                where QueueProcessing.ProcessedOn == null
                                && QueueProcessing.CreatedOn <= endDate 
                                && QueueProcessing.CreatedOn >= startDate
                                && !context.Policies.Any(p => p.CheckOutDetailsId == QueueProcessing.ReferenceId)
                                && QueueProcessing.ProcessingTries == scheduleTask.MaxTrials
                                select new FailPolicy
                                {
                                    CheckoutDetail = CheckOutDetails,
                                    PolicyProcessingQueue = QueueProcessing,
                                    Invoice = invoice,
                                    ProductType = CheckOutDetails.ProductType,
                                    InsuranceCompany = invoice.InsuranceCompany,
                                    PolicyStatus = CheckOutDetails.PolicyStatu,
                                    Vehicle = CheckOutDetails.Vehicle,
                                    Driver = CheckOutDetails.Driver
                                };

                    List<FailPolicy> policyList = query.DistinctBy(q=>q.CheckoutDetail.ReferenceId).ToList<FailPolicy>();

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
