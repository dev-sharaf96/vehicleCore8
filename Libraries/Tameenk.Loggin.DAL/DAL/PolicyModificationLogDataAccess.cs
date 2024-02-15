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

   public class PolicyModificationLogDataAccess
    {

        /// <summary>
        /// Add Add Driver Log to tameenkLog context.
        /// </summary>
        /// <param name="quotationRequestLog"></param>
        /// <returns></returns>   
        public static bool AddPolicyModificationLog(PolicyModificationLog log)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    log.CreatedDate = DateTime.Now;
                    context.PolicyModificationLogs.Add(log);
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
                        System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog_" + log.RefrenceId + "_.txt", ex.ToString());
                    }
                }
                return false;
            }

        }
    
    }
}
