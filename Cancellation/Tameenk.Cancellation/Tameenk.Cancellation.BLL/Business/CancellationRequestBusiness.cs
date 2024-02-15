using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.DAL;
using Tameenk.Cancellation.DAL.Entities;
using Tameenk.Cancellation.Service;
using Tameenk.Cancellation.Service.Models;
using Tameenk.Cancellation.Service.Providers;

namespace Tameenk.Cancellation.BLL.Business
{
    public class CancellationRequestBusiness : ICancellationRequestBusiness
    {
        protected readonly IUnitOfWork unitOfWork;
        private readonly ICachingEngine cachingEngine;
        private readonly IRestfulInsuranceProvider restfulInsuranceProvider;
        private readonly IInsuranceCompanyBusiness insuranceCompanyBusiness;

        public CancellationRequestBusiness(IUnitOfWork unitOfWork, ICachingEngine cachingEngine,
            IRestfulInsuranceProvider restfulInsuranceProvider, IInsuranceCompanyBusiness insuranceCompanyBusiness)
        {
            this.unitOfWork = unitOfWork;
            this.cachingEngine = cachingEngine;
            this.restfulInsuranceProvider = restfulInsuranceProvider;
            this.insuranceCompanyBusiness = insuranceCompanyBusiness;
        }
        public int Count()
        {
            return this.unitOfWork.CancellationRequests.Count();
        }

        public CancellationRequest Get(int id)
        {
            return this.unitOfWork.CancellationRequests.Get(id);
        }

        public List<CancellationRequest> GetAll()
        {
            return this.unitOfWork.CancellationRequests.GetAll().ToList();

        }

        public ServiceOutput GetActivePolicies(CancellationRequest CancellationRequest)
        {
            PolicyActiveRequest policyActiveRequest = new PolicyActiveRequest
            {
                InsuredId = CancellationRequest.InsuredId,
                ReasonCode = CancellationRequest.ReasonCode,
                ReferenceId = CancellationRequest.ReferenceId,
                VehicleId = CancellationRequest.VehicleId,
                VehicleIdTypeCode = CancellationRequest.VehicleIdTypeCode
            };

            var company = insuranceCompanyBusiness.Get(CancellationRequest.CancelFromCompany);
            restfulInsuranceProvider.SetRestfulConfiguration(company, company.GetPolicyServiceUrl);
            var output = restfulInsuranceProvider.GetGetPolicyRequest(policyActiveRequest);
            dynamic data = new ExpandoObject();
            data.ActivePolicies = output.Output;

            if (CancellationRequest.ReasonCode == 3)
            {
                var company2 = insuranceCompanyBusiness.Get(CancellationRequest.RegisterToCompany.Value);
                restfulInsuranceProvider.SetRestfulConfiguration(company2, company.GetPolicyServiceUrl);
                var output2 = restfulInsuranceProvider.GetGetPolicyRequest(policyActiveRequest);;
                data.SecondActivePolicies = output2.Output; 
            }
            output.Output = data;
            return output;

        }

        public ServiceOutput PolicyCancellation(CancellationRequest CancellationRequest)
        {
            PolicyActiveRequest policyActiveRequest = new PolicyActiveRequest
            {
                InsuredId = CancellationRequest.InsuredId,
                ReasonCode = CancellationRequest.ReasonCode,
                ReferenceId = CancellationRequest.ReferenceId,
                VehicleId = CancellationRequest.VehicleId,
                VehicleIdTypeCode = CancellationRequest.VehicleIdTypeCode
            };

            var company = insuranceCompanyBusiness.Get(CancellationRequest.CancelFromCompany);
            restfulInsuranceProvider.SetRestfulConfiguration(company, company.GetPolicyServiceUrl);
            var output = restfulInsuranceProvider.GetGetPolicyRequest(policyActiveRequest);
            dynamic data = new ExpandoObject();
            data.ActivePolicies = output.Output;

            if (CancellationRequest.ReasonCode == 3)
            {
                var company2 = insuranceCompanyBusiness.Get(CancellationRequest.RegisterToCompany.Value);
                restfulInsuranceProvider.SetRestfulConfiguration(company2, company.GetPolicyServiceUrl);
                var output2 = restfulInsuranceProvider.GetGetPolicyRequest(policyActiveRequest); ;
                data.SecondActivePolicies = output2.Output;
            }
            output.Output = data;
            return output;

        }

        public void Add(CancellationRequest entity)
        {
            throw new NotImplementedException();
        }

        public void Update(CancellationRequest entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(int Id)
        {
            throw new NotImplementedException();
        }

        public List<CancellationRequest> Find(Expression<Func<CancellationRequest, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<CancellationRequest> GetActive()
        {
            throw new NotImplementedException();
        }

        public List<CancellationRequest> GetInactive()
        {
            throw new NotImplementedException();
        }
    }
}
