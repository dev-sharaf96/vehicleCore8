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
    public class CheckoutDetailDataAccess
    {
        public static List<CheckoutDetail> GetFailedFiles(int commandTimeout)
        {
            try
            {
                using (Tameenk context = new Tameenk())
                {
                    context.Database.CommandTimeout = commandTimeout;
                    int? status = 7;
                    var result =( from checkout in context.CheckoutDetails
                                 where checkout.PolicyStatusId == status
                                 select checkout).ToList();
                    if (result.Count > 0)
                        return result;
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
