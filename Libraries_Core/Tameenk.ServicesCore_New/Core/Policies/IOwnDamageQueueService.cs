using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core.Policies
{
   public interface IOwnDamageQueueService
    {
        void AddOwnDamageQueue(List<policyDetailForOD> policies, out string excption);
    }
}
