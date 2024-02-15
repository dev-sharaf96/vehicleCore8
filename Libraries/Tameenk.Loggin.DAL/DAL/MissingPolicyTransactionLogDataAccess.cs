using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using System.Linq.Dynamic;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using Tameenk.Loggin.DAL.Dtos;

namespace Tameenk.Loggin.DAL
{
    public class MissingPolicyTransactionLogDataAccess
    {
        public static bool AddToMissingPolicyTransactionLogDataAccess(MissingPolicyTransactionServicesLog toSaveLog)
        {
            using (TameenkLog context = new TameenkLog())
            {
                try
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    context.MissingPolicyTransactionServicesLog.Add(toSaveLog);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    toSaveLog.ErrorDescription = ex.ToString();
                    context.MissingPolicyTransactionServicesLog.Add(toSaveLog);
                    context.SaveChanges();
                    return false;
                }
            }
        }
    }
}
