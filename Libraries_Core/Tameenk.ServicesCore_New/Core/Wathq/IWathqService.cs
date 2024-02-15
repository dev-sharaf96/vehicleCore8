
using Tameenk.Core.Domain.Entities;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Implementation.Wathq;

namespace Tameenk.Services.Core.Wathq
{
   public interface IWathqService
    {
        WathqOutput GetTreeFromWathqResponse(string OwnerNationalId, ServiceRequestLog log);
        WathqInfo GetTreeFromWathqResponseCache(string OwnerNationalId, out string exception);
    }
}
