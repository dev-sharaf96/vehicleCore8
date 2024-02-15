using System.Collections.Generic;
using System.Linq.Expressions;
using Tameenk.Cancellation.DAL.Entities;
using Tameenk.Cancellation.DAL.Repositories;
using Tameenk.Cancellation.Service;
using Tameenk.Cancellation.Service.Models;

namespace Tameenk.Cancellation.BLL.Business
{
    public interface ICancellationRequestBusiness :   IBaseBusiness<CancellationRequest>
    {
        ServiceOutput GetActivePolicies(CancellationRequest CancellationRequest);
    }
}
