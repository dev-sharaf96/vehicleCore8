using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Services.Logging;
using Tameenk.Core.Domain.Entities;


namespace Tameenk.Services.Implementation.IntegrationTransactions
{
    public class IntegrationTransactionService : IIntegrationTransactionService
    {
        private static IRepository<IntegrationTransaction> _integrationTransactionRepository;

        public IntegrationTransactionService(IRepository<IntegrationTransaction> integrationTransactioRepository)
        {
            _integrationTransactionRepository = integrationTransactioRepository;
        }

        public virtual void Insert(IntegrationTransaction transaction)
        {
            if (transaction != null)
                _integrationTransactionRepository.Insert(transaction);
        }
    }
}
