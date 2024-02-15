using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;

namespace Tamkeen.bll.Model
{
    public class PolicyResponseMessage
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? PolicyIssuanceDate { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        private string policyFileUrl = null;
        public string PolicyFileUrl
        {
            get
            {
                if (string.IsNullOrEmpty(policyFileUrl))
                {
                    if (PolicyNotifier != null)
                    {
                        PolicyScheduleRequest request = new PolicyScheduleRequest
                        {
                            PolicyNo = this.PolicyNo,
                            ReferenceId = this.ReferenceId
                        };

                        PolicyScheduleResponse response = PolicyNotifier(request);
                        while (response.StatusCode != 1)
                        {
                            System.Threading.Thread.Sleep(60000);
                            response = PolicyNotifier(request);
                        }

                        policyFileUrl = response.PolicyFileUrl;
                        return policyFileUrl;
                    }
                    else
                        return policyFileUrl;
                }
                return policyFileUrl;
            }
            set { policyFileUrl = value; }
        }
        public Byte[] PolicyFile { get; set; }
        public PolicyDetails PolicyDetails { get; set; }
        public delegate PolicyScheduleResponse PolicyScheduler(PolicyScheduleRequest request);
        public event PolicyScheduler PolicyNotifier;
        [JsonIgnore]
        public Company IssueCompany { get; set; }
    }
}
