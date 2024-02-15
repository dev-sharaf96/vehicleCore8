using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Data;

namespace Tameenk.Services.Implementation
{
    public class AutoleasingMinimumPremiumService : IAutoleasingMinimumPremiumService
    {
        private readonly IRepository<AutoleasingMinimumPremium> _autoleasingMinimumPremiumRepository;


        public AutoleasingMinimumPremiumService(IRepository<AutoleasingMinimumPremium> autoleasingMinimumPremiumRepository)
        {
            this._autoleasingMinimumPremiumRepository = autoleasingMinimumPremiumRepository;
        }
        public bool ConfirmUpdateOrAddPremium(AutoleasingMinimumPremium minimumRepair,string userId)
        {
            try
            {
                var isRecordExist = _autoleasingMinimumPremiumRepository.Table.Where(x => x.BankId == minimumRepair.BankId).FirstOrDefault();
                if (isRecordExist != null)
                {
                    
                    isRecordExist.FirstYear = minimumRepair.FirstYear;
                    isRecordExist.SecondYear = minimumRepair.SecondYear;
                    isRecordExist.ThirdYear = minimumRepair.ThirdYear;
                    isRecordExist.FourthYear = minimumRepair.FourthYear;
                    isRecordExist.FifthYear = minimumRepair.FifthYear;
                    isRecordExist.BankId = minimumRepair.BankId;
                    isRecordExist.ModifiedDate = DateTime.Now;
                    isRecordExist.ModifiedBy = userId;
                    _autoleasingMinimumPremiumRepository.Update(isRecordExist);
                }
                else
                {
                    minimumRepair.CreatedDate = DateTime.Now;
                    minimumRepair.CreatedBy = userId;
                    _autoleasingMinimumPremiumRepository.Insert(minimumRepair);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public AutoleasingMinimumPremium GetPremiumByBankId(int bankId)
        {
            try
            {
                return _autoleasingMinimumPremiumRepository.Table.Where(x => x.BankId == bankId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsPremiumExistWithBank(AutoleasingMinimumPremium agencyRepair)
        {
            try
            {
                var isExist = _autoleasingMinimumPremiumRepository.Table.Where(x => x.BankId == agencyRepair.BankId && x.FirstYear == agencyRepair.FirstYear && x.SecondYear == agencyRepair.SecondYear && x.ThirdYear == agencyRepair.ThirdYear && x.FourthYear == agencyRepair.FourthYear && x.FifthYear == agencyRepair.FifthYear).FirstOrDefault();
                if (isExist != null)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

       
    }
}
