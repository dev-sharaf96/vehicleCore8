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
