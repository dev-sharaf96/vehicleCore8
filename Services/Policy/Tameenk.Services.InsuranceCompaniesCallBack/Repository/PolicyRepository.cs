using System;
using System.Data.Entity;
using System.Linq;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Resources.WebResources;
using Tameenk.Services.Implementation.Policies;
using Tameenk.Services.InsuranceCompaniesCallBack.Models;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Repository
{
    public class PolicyRepository : IPolicyRepository
    {
        IRepository<Tameenk.Core.Domain.Entities.Policy> _policyRepo;
        IRepository<NajmStatusHistory> _najmStatusHistoryRepo;
        IRepository<NajmStatus> _najmStatusRepo;
        private readonly IRepository<WataniyaMotorPolicyInfo> _wataniyaMotorPolicyInfoRepository;

        /// <summary>
        /// The Ctor
        /// </summary>
        /// <param name="policyRepo">policy Repository </param>
        /// <param name="najmStatusHistoryRepo">Najm status history Repository</param>
        /// <param name="integrationTransactionsRepo">integration Transactions Repository</param>
        /// <param name="najmStatusRepo">najm Status Repository</param>
        public PolicyRepository(IRepository<Tameenk.Core.Domain.Entities.Policy> policyRepo, IRepository<NajmStatusHistory> najmStatusHistoryRepo, 
            IRepository<NajmStatus> najmStatusRepo, IRepository<WataniyaMotorPolicyInfo> wataniyaMotorPolicyInfoRepository
            )
        {
            _policyRepo = policyRepo;
            _najmStatusHistoryRepo = najmStatusHistoryRepo;
            _najmStatusRepo = najmStatusRepo;
            _wataniyaMotorPolicyInfoRepository = wataniyaMotorPolicyInfoRepository;
        }

        public void NotifyPolicyUploadCompletion(PolicyUploadNotificationModel policyUploadNotificationModel)
        {
            var toSaveNajmStatusHistory = new NajmStatusHistory
            {
                PolicyNo = policyUploadNotificationModel.PolicyNo,
                ReferenceId = policyUploadNotificationModel.ReferenceId,
                StatusCode = policyUploadNotificationModel.StatusCode,
                StatusDescription = policyUploadNotificationModel.StatusDescription,
                UploadedDate = policyUploadNotificationModel.UploadedDate,
                UploadedReference = policyUploadNotificationModel.UploadedReference
            };
            _najmStatusHistoryRepo.Insert(toSaveNajmStatusHistory);
        }

        public Tameenk.Core.Domain.Entities.Policy GetPolicyWithReferenceIdAndPolicyNumber(PolicyUploadNotificationModel policyUploadNotificationModel)
        {
           
            return _policyRepo.Table.Where(x=>x.PolicyNo == policyUploadNotificationModel.PolicyNo && x.CheckoutDetail.ReferenceId == policyUploadNotificationModel.ReferenceId).FirstOrDefault();
        }

        public void SavePolicyWithNajmStatus(PolicyUploadNotificationModel policyUploadNotificationModel)
        {
            Tameenk.Core.Domain.Entities.Policy toSavePolicy = GetPolicyWithReferenceIdAndPolicyNumber(policyUploadNotificationModel);
            if (toSavePolicy != null)
            {
                string strNajmStatusDescription = string.Empty;
                if (policyUploadNotificationModel.StatusCode == 1)
                {
                    toSavePolicy.NajmStatusId = _najmStatusRepo.Table.FirstOrDefault(n => n.Code == "1").Id;
                    strNajmStatusDescription = WebResources.Active;
                }
                else
                {
                    toSavePolicy.NajmStatusId = _najmStatusRepo.Table.FirstOrDefault(n => n.Code == "Fail").Id;
                    strNajmStatusDescription = WebResources.Failed;
                }
                    
                
                strNajmStatusDescription += " : " + policyUploadNotificationModel.StatusDescription;


                toSavePolicy.ModifiedDate = DateTime.Now;
                toSavePolicy.NajmStatus = strNajmStatusDescription;
                _policyRepo.Update(toSavePolicy);
            }
        }

        /// <summary>
        /// Check if there is a policy with this policyNo and Reference Id
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="policyNo"></param>
        /// <returns></returns>
        public bool IsPolicyExist(string referenceId,string policyNo)
        {
            Tameenk.Core.Domain.Entities.Policy policy = _policyRepo.TableNoTracking.Where(x => x.PolicyNo == policyNo && x.CheckOutDetailsId == referenceId).FirstOrDefault();
            if (policy != null)
                return true;
            policy = _policyRepo.TableNoTracking.Where(x => x.CheckOutDetailsId == referenceId).FirstOrDefault();
            if (policy != null)
                return true;
            return false;
        }


        /// <summary>
        /// Check if there is a policy with this policyNo or Reference Id
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="policyNo"></param>
        /// <returns></returns>
        public bool CheckPolicyExistenceByReferenceIdOrPolicyNo(string referenceId, string policyNo)
        {
            Tameenk.Core.Domain.Entities.Policy policy = _policyRepo.Table.Where(x => x.PolicyNo == policyNo || x.CheckoutDetail.ReferenceId == referenceId).FirstOrDefault();
            if (policy == null)
                return false;
            return true;
        }

        public WataniyaMotorPolicyInfo CheckTplWataniyaInitialPolicyExistenceByReferenceId(string referenceId)
        {
            var policyInitialInfo = _wataniyaMotorPolicyInfoRepository.Table.Where(a => a.ReferenceId == referenceId).FirstOrDefault();
            return policyInitialInfo;
        }

        public WataniyaMotorPolicyInfo CheckCompWataniyaInitialPolicyExistenceByReferenceId(string referenceId)
        {
            var policyInitialInfo = _wataniyaMotorPolicyInfoRepository.Table.Where(a => a.PolicyRequestReferenceNo == referenceId).FirstOrDefault();
            return policyInitialInfo;
        }

        public bool UpdateWataniyaPolicyInfoCallback(WataniyaMotorPolicyInfo policyInfo, out string exception)
        {
            exception = string.Empty;
            try
            {
                _wataniyaMotorPolicyInfoRepository.Update(policyInfo);
                return true;
            }
            catch (System.Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }
    }
}
