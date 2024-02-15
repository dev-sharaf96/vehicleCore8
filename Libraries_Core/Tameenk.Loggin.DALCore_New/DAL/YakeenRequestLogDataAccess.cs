namespace Tameenk.Loggin.DAL
{
    public class YakeenRequestLogDataAccess
    {
        public static YakeenServiceRequestLog GetYakeenResponseByNIN(string connectionString, string nin)
        {
            try
            {
                TameenkLog context;

                if (string.IsNullOrEmpty(connectionString))
                    context = new TameenkLog();
                else
                    context = new TameenkLog(connectionString);

                string methodName = nin.StartsWith("1") ? "Yakeen-getCitizenIDInfo" : "Yakeen-getAlienInfoByIqama";
                using (context)
                {
                    context.Database.CommandTimeout = 240;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    return (from d in context.YakeenServiceRequestLogs
                            where d.DriverNin == nin && d.Method == methodName
                            orderby d.ID descending
                            select d
                            ).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static YakeenServiceRequestLog GetYakeenMobileVerification(string nin)
        {
            try
            {

                using (TameenkLog context = new TameenkLog())
                {
                    return (from d in context.YakeenServiceRequestLogs
                            where d.DriverNin == nin && d.Method == "Yakeen-getYakeenMobileVerification"
                            orderby d.ID descending
                            select d
                             ).FirstOrDefault();
                }
            }
            catch (Exception )
            {
                return null;
            }
        }

        public static YakeenServiceRequestLog GetYakeenUserDataVerification(string nin)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    return (from d in context.YakeenServiceRequestLogs
                            where d.DriverNin == nin && (d.Method == "Yakeen-getCitizenIDInfo" || d.Method == "Yakeen-getAlienInfoByIqama")
                            orderby d.ID descending
                            select d).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
