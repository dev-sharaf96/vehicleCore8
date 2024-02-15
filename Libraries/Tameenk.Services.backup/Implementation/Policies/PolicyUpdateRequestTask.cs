using System.Linq;
using Tameenk.Core.Configuration;
using Tameenk.Core.Domain.Enums.Policies;
using Tameenk.Services.Core.Policies;
using Tameenk.Services.Logging;
using Tameenk.Services.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class PolicyUpdateRequestTask : ITask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly TameenkConfig _config;
        private readonly IPolicyService _policyService;
        #endregion

        #region Ctor
        public PolicyUpdateRequestTask(ILogger logger, TameenkConfig config, IPolicyService policyService)
        {
            _logger = logger;
            _config = config;
            _policyService = policyService;
        }
        #endregion


        #region Public Methods

        public void Execute(int MaxTrials, int? SendingThreshold, string CommonPolicyFailureRecipient)
        {
            //get policy update requests, check if CreatedAt exceeds 16 hours
            //then change the state to Expired

            //1-Get policy update requests that has expired payment
            var policyUpdateRequests = _policyService.GetPolicyUpdateRequestsWithExpiredPayment().ToList();
            foreach (var request in policyUpdateRequests)
            {
                request.Status = PolicyUpdateRequestStatus.Expired;
            }
            _policyService.UpdatePolicyUpdateRequests(policyUpdateRequests);

        }
        #endregion
    }
}
