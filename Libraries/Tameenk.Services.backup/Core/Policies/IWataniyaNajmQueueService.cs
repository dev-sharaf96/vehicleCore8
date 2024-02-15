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
    public interface IWataniyaNajmQueueService
    {
        List<WataniyaNajmQueue> GetFromWataniyaNajmQueue(out string exception);
        bool GetAndUpdateWataniyaNajmQueue(int id, WataniyaNajmQueue policy, string serverIP, out string exception);
        bool AddWataniyaNajmQueue(string policyNo, string referenceId, out string exception);

    }
}
