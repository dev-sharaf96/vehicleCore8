using System;
using System.Collections.Generic;
using System.Text;
using Tameenk.Cancellation.DAL.Entities;
using Tameenk.Cancellation.Service.Configuration;
using Tameenk.Cancellation.Service.Models;

namespace Tameenk.Cancellation.Service.Providers
{
    public interface IRestfulInsuranceProvider
    {
        void SetRestfulConfiguration(InsuranceCompany insuranceCompany ,string ProviderUrl);
        ServiceOutput GetGetPolicyRequest(PolicyActiveRequest policyServiceRequest);
    }
}
