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
