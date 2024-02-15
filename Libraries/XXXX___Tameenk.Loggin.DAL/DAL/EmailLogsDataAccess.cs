﻿using System;
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

namespace Tameenk.Loggin.DAL
{
    public class EmailLogsDataAccess
    {
        public static bool AddToEmailLogsDataAccess(EmailLog toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    context.EmailLogs.Add(toSaveLog);
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

        public static List<EmailLog> GetFromEmailNotification(string email, string ReferenceId)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    var info = (from d in context.EmailLogs
                                where d.ReferenceId == ReferenceId
                                   && d.Email.Contains(email)
                                   && d.ErrorCode == 1
                                select d).ToList();
                    return info;
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