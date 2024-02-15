using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Tameenk.Common.Utilities;

namespace Tameenk.Loggin.DAL
{
    public class PolicyFailedTransactionDataAccess
    {
        public static bool AddToPolicyFailedTransactions(PolicyFailedTransaction toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    toSaveLog.ModifiedDate = DateTime.Now;
                    context.PolicyFailedTransactions.Add(toSaveLog);
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

        public static List<PolicyFailedTransaction> GetPolicyFailedTransactions(int commandTimeout)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = commandTimeout;
                    DateTime dtTo = DateTime.Now;
                    DateTime dtFrom = dtTo.AddHours(-23).AddMinutes(-59).AddSeconds(-59);

                    var transactions = (from trans in context.PolicyFailedTransactions
                                        where trans.CreatedDate >= dtFrom && trans.CreatedDate <= dtTo
                                        select trans).ToList();
                    if (transactions.Count > 0)
                        return transactions;
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

        public static bool UpdateNumberOfHits(int ID)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var request = (from p in context.PolicyFailedTransactions
                                   where p.ID == ID
                                   select p).FirstOrDefault();

                    if (request != null)
                    {
                        int numOfEaiHits = 0;
                        if (request.NumberOfHits.HasValue)
                            numOfEaiHits = request.NumberOfHits.Value + 1;
                        else
                            numOfEaiHits = 1;

                        request.ID = request.ID;
                        request.NumberOfHits = numOfEaiHits;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }

        public static bool UpdatePolicyFailedTransactions(int ID, int numberOfEAIHits, string serviceErrorCode, string serviceErrorDescription)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var request = (from p in context.PolicyFailedTransactions
                                   where p.ID == ID
                                   select p).FirstOrDefault();

                    if (request != null)
                    {
                        request.ID = ID;
                        request.NumberOfHits = numberOfEAIHits;
                        request.ServiceErrorCode = serviceErrorCode;
                        request.ServiceErrorDescription = serviceErrorDescription;
                        request.ModifiedDate = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;
            }
        }
        /// <returns></returns>
        public static List<PolicyFailedTransaction> GetFailedTransactionsSinceAday(int commandTimeout)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = commandTimeout;
                    DateTime dt = DateTime.Now.AddHours(-23).AddMinutes(-59).AddSeconds(-59);
                    var transactions = (from trans in context.PolicyFailedTransactions
                                        where trans.CreatedDate <= dt
                                        select trans).ToList();
                    if (transactions.Count > 0)
                        return transactions;
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

        public static PolicyFailedTransaction CheckPolicyFailedTransactionsbyUserIdForOneDay(Guid userId)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    // DateTime dt = DateTime.Now;
                    var transactions = (from trans in context.PolicyFailedTransactions
                                        where trans.UserId == userId
                                        orderby trans.CreatedDate descending
                                        select trans).FirstOrDefault();
                    return transactions;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;
            }
        }

        public static bool? DeleteFromPolicyFailedTransaction(int ID, Guid userId)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var transactions = (from trans in context.PolicyFailedTransactions
                                        where trans.ID == ID && trans.UserId == userId
                                        select trans).FirstOrDefault();
                    context.PolicyFailedTransactions.Remove(transactions);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;
            }
        }

        public static List<PolicyFailedTransaction> GetAllFailedTransaction()
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    DateTime dt = DateTime.Now;
                    DateTime dateTo = dt.AddHours(-23).AddMinutes(-59).AddSeconds(-59);
                    var transactions = (from trans in context.PolicyFailedTransactions
                                        where trans.CreatedDate <= dt && trans.CreatedDate >= dateTo
                                        select trans).ToList();
                    if (transactions.Count > 0)
                        return transactions;
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
