using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Repositories.Interfaces;
using Tameenk.Cancellation.DAL.Entities;

namespace Tameenk.Cancellation.DAL.Repositories
{
    public class ServiceRequestLogRepository : GRUDRepository<ServiceRequestLog>, IServiceRequestLogRepository
    {
        public ServiceRequestLogRepository(DbContext context) : base(context)
        { }

        private CancellationContext _appContext => (CancellationContext)_context;
    }
}
