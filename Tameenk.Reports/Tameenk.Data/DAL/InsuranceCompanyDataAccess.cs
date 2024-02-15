using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Data.DAL
{
    public class InsuranceCompanyDataAccess
    {
        public static List<InsuranceCompany> GetInsuranceCompanyList(int commandTimeout)
        {
            try
            {
                using (Tameenk context = new Tameenk())
                {
                    context.Database.CommandTimeout = commandTimeout;

                    var insuranceCompanies = context.InsuranceCompanies.ToList();
                    
                    if (insuranceCompanies.Count > 0)
                        return insuranceCompanies;
                    else
                        return null;
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
