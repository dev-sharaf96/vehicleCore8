﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;

namespace Tameenk.Loggin.DAL.DAL
{
    public class QuotationResponseLogDataAccess
    {
        public static List<ServiceRequestResponseTimeFromDBModel> ServiceRequestResponseTimeFromDBFilter(ServiceResponseTimeFilterModel responsetimefilter, int commandTimeout, out int total, out string exception, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Company Name", bool? sortOrder = false)

                    if (!string.IsNullOrEmpty(responsetimefilter.InsuranceTypeId))
                    {
                        SqlParameter InsuranceTypeId = new SqlParameter() { ParameterName = "InsuranceTypeId", Value = responsetimefilter.InsuranceTypeId };
                        command.Parameters.Add(InsuranceTypeId);
                    }

                    if (!string.IsNullOrEmpty(responsetimefilter.InsuranceCompanyId))
                    {
                        SqlParameter InsuranceCompanyId = new SqlParameter() { ParameterName = "InsuranceCompanyId", Value = responsetimefilter.InsuranceCompanyId };
                        command.Parameters.Add(InsuranceCompanyId);
                    }
                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    List<ServiceRequestResponseTimeFromDBModel> filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<ServiceRequestResponseTimeFromDBModel>(reader).ToList();
                    total = filteredData.Count();
                    filteredData = filteredData.OrderBy(x => x.AvgInSec).ToList();
                    return filteredData;
                }
            }
            catch (Exception exp)
            {
                exception = exp.ToString();
                total = 0;
                return null;
            }
        }
    }
}