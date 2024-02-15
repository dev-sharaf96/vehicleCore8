using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Loggin.DAL
{
    public class WhatsAppLogDataAccess
    {
        
        public static int GetFromWhatsAppNotification(string referenceId)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    int count = (from d in context.WhatsAppLogs
                                  where d.ReferenceId == referenceId&&d.ErrorCode==0
                                  select d).Count();
                    return count;
                }
            }
            catch (Exception exp)
            {
                return 0;

            }
        }

    }
}
