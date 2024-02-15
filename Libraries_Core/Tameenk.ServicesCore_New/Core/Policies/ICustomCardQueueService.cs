using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Policies;
using Tameenk.Services.Implementation;

namespace Tameenk.Services.Core.Policies
{
    public interface ICustomCardQueueService
    {
        List<CustomCardQueue> GetFromCustomCardQueue(out string exception);
        bool GetAndUpdateCustomCardProcessingQueue(int id, CustomCardQueue policy, string serverIP, out string exception);
        void AddCustomCardQueue(CheckoutDetail input, string policyNo);
     
    }
}
