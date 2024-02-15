using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using System.Linq.Dynamic;
namespace Tameenk.Loggin.DAL
{

   public class UserTicketLogDataAccess
    {
        public static bool AddUserTicketLog(UserTicketLog userTicketLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    userTicketLog.CreatedDate = DateTime.Now;
                    context.UserTicketLogs.Add(userTicketLog);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\StagingQuotationApi\logs\log1.txt", ex.ToString());
                    }
                }
                return false;
            }

        }

    }
}
