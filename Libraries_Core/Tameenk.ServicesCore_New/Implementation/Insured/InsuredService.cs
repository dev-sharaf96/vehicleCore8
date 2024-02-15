using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;


namespace Tameenk.Services
{
    public class InsuredService : IInsuredService
    {
        #region constructor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="productTypeRepository">product type Repository</param>
        public InsuredService()
        {
           
        }
        #endregion region 
        public Insured GetIInsuredByNationalId(string nationalId)
        {
            Insured insured = null;
            IDbContext idbContext = (IDbContext)EngineContext.Current.Resolve<IDbContext>();
            try
            {
                idbContext.DatabaseInstance.CommandTimeout = new int?(60);
                var command = idbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetIInsuredByNationalId";
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter driverNinParam = new SqlParameter() { ParameterName = "@nationalId", Value = nationalId };
                command.Parameters.Add(driverNinParam);
                idbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();
                insured = ((IObjectContextAdapter)idbContext).ObjectContext.Translate<Insured>(reader).FirstOrDefault();
                idbContext.DatabaseInstance.Connection.Close();
                return insured;
            }
            catch (Exception )
            {
                idbContext.DatabaseInstance.Connection.Close();
                return null;
            }
        }

    }
}
