﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tameenk.Loggin.DAL
{
    public class IdentityRequestLogDataAccess
    {
        public static bool AddToIdentityRequestLog(IdentityRequestLog toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    context.IdentityRequestLogs.Add(toSaveLog);
                    context.SaveChanges();
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

    }
}