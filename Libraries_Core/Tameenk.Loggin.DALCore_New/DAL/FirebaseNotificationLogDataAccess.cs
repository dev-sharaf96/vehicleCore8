namespace Tameenk.Loggin.DAL
{

    public class FirebaseNotificationLogDataAccess
    {
        public static bool AddToFirebaseNotificationLog(FirebaseNotificationLog log)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    log.CreatedDate = DateTime.Now;
                    context.FirebaseNotificationLogs.Add(log);
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
        public static FirebaseNotificationLog GetFromFirebaseNotificationByRefernceId(string refernceId,string method)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    var notification = (from d in context.FirebaseNotificationLogs
                                  where d.ReferenceId == refernceId &&d.ErrorCode==0&&d.MethodName== method
                                        orderby d.ID descending
                                  select d).FirstOrDefault();
                    return notification;
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
