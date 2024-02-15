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
            catch (Exception)
            {
                return 0;

            }
        }

    }
}
