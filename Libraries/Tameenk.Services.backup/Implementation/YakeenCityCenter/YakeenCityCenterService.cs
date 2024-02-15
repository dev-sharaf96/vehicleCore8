using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Core;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Implementation
{
    public class YakeenCityCenterService : IYakeenCityCenterService
    {
        private readonly IRepository<YakeenCityCenter> _yakeenCityCenterRepository;

        public YakeenCityCenterService(IRepository<YakeenCityCenter> yakeenCityCenterRepository)
        {
            _yakeenCityCenterRepository = yakeenCityCenterRepository ?? throw new TameenkArgumentNullException(nameof(IRepository<YakeenCityCenter>)); ;
        }

        public PolicyOutput AddorNewYakeenCityCenter(YakeenCityCenterModel model)
        {
            var outPut = new PolicyOutput();
            try
            {
                var dbModel = new YakeenCityCenter()
                {
                    CityID = model.CityId,
                    CityName = model.CityName,
                    EnglishName = model.EnglishName,
                    ZipCode = model.ZipCode,
                    RegionID = model.RegionId,
                    RegionArabicName = model.RegionArabicName,
                    RegionEnglishName = model.RegionEnglishName,
                    ElmCode = model.ElmCode
                };
                _yakeenCityCenterRepository.Insert(dbModel);

                outPut.ErrorCode = 1;
                outPut.ErrorDescription = "success";

                return outPut;
            }
            catch (Exception ex)
            {
                outPut.ErrorCode = 2;
                outPut.ErrorDescription = ex.Message;
                return outPut;
            }
        }

        public List<YakeenCityCenterModel> GetYakeenCityCentersWithFilter(out int total, bool export, int cityId, string cityName, int zipCode, int elmCode, int pageIndex, int pageSize)
        {
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            try
            {
                var command = dbContext.DatabaseInstance.Connection.CreateCommand();
                command.CommandText = "GetAllYakeenCityCentersWithFilter";
                command.CommandType = CommandType.StoredProcedure;
                dbContext.DatabaseInstance.CommandTimeout = 60 * 60 * 60;

                SqlParameter CityIdParameter = new SqlParameter() { ParameterName = "cityId", Value = (cityId > 0) ? cityId : 0 };
                command.Parameters.Add(CityIdParameter);

                SqlParameter CityNameParameter = new SqlParameter() { ParameterName = "cityName", Value = cityName ?? "" };
                command.Parameters.Add(CityNameParameter);

                SqlParameter ZipCodeParameter = new SqlParameter() { ParameterName = "zipCode", Value = (zipCode > 0) ? zipCode : 0 };
                command.Parameters.Add(ZipCodeParameter);

                SqlParameter ElmCodeParameter = new SqlParameter() { ParameterName = "elmCode", Value = (elmCode > 0) ? elmCode : 0 };
                command.Parameters.Add(ElmCodeParameter);

                SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = pageIndex + 1 };
                command.Parameters.Add(pageNumberParameter);

                SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                command.Parameters.Add(pageSizeParameter);

                SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = export };
                command.Parameters.Add(ExportParameter);

                dbContext.DatabaseInstance.Connection.Open();
                var reader = command.ExecuteReader();

                // get policy filteration data
                List<YakeenCityCenterModel> filteredData = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<YakeenCityCenterModel>(reader).ToList();

                //get data count
                reader.NextResult();
                total = ((IObjectContextAdapter)dbContext).ObjectContext.Translate<int>(reader).FirstOrDefault();

                return filteredData;
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
            finally
            {
                if (dbContext.DatabaseInstance.Connection.State == ConnectionState.Open)
                    dbContext.DatabaseInstance.Connection.Close();
            }
        }
    }
}
