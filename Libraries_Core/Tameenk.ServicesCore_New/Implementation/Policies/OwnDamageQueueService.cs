using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Infrastructure;
using Tameenk.Data;
using Tameenk.Services.Core.Policies;

namespace Tameenk.Services.Implementation.Policies
{
 public   class OwnDamageQueueService: IOwnDamageQueueService
    {

        private readonly IRepository<OwnDamageQueue> _OwnDamageQueueRepository;
        public OwnDamageQueueService(IRepository<OwnDamageQueue> OwnDamageQueueRepository)
        {
            _OwnDamageQueueRepository = OwnDamageQueueRepository;
        }

        public void AddOwnDamageQueue(List<policyDetailForOD> policies, out string excption)
        {
            excption = string.Empty;
            try
            {
                List<OwnDamageQueue> queueList = new List<OwnDamageQueue>();
                foreach (var policy in policies)
                {
                    OwnDamageQueue OwnDamage = new OwnDamageQueue();
                    OwnDamage.Method = "OwnDamage";
                    OwnDamage.CreatedDate = DateTime.Now;
                    OwnDamage.Phone = policy.Phone;
                    OwnDamage.PolicyNo = policy.PolicyNo;
                    OwnDamage.ExternalId = policy.ExternalId;
                    OwnDamage.SelectedLanguage = policy.SelectedLanguage ?? 1;
                    OwnDamage.PolicyExpiryDate = policy.PolicyExpiryDate;

                    queueList.Add(OwnDamage);
                }

                _OwnDamageQueueRepository.Insert(queueList);
            }
            catch (Exception ex)
            {
                excption = ex.ToString();
            }
        }


    }
}
