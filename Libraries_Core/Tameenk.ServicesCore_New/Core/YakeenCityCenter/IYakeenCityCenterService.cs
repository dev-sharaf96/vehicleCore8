using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL.Dtos;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Services.Core
{
    public interface IYakeenCityCenterService
    {
        PolicyOutput AddorNewYakeenCityCenter(YakeenCityCenterModel model);
        List<YakeenCityCenterModel> GetYakeenCityCentersWithFilter(out int total, bool export, int cityId, string cityName, int zipCode, int elmCode, int pageIndex, int pageSize);
    }
}
