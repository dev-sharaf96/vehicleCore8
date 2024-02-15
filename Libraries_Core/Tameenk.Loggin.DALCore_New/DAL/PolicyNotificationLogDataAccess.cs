namespace Tameenk.Loggin.DAL
{
    public class PolicyNotificationLogDataAccess
    {
        public static bool AddtoPolicyNotificationLogs(PolicyNotificationLog toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    toSaveLog.CreatedDate = DateTime.Now;
                    context.PolicyNotificationLogs.Add(toSaveLog);
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
