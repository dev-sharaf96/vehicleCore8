using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core;

namespace Tameenk.Services.Implementation
{
    public class AutoleasingAgencyRepairService : IAutoleasingAgencyRepairService
    {
        private readonly IRepository<AutoleasingAgencyRepair> _autoleasingAgencyRepairRepository;

        public AutoleasingAgencyRepairService(IRepository<AutoleasingAgencyRepair> autoleasingAgencyRepairRepository)
        {
            this._autoleasingAgencyRepairRepository = autoleasingAgencyRepairRepository;
        }
        public bool ConfirmUpdateOrAddAgencyRepair(AutoleasingAgencyRepair agencyRepair, string userId)
        {
            try
            {
                var isRecordExist = _autoleasingAgencyRepairRepository.Table.Where(x => x.BankId == agencyRepair.BankId).FirstOrDefault();
                if (isRecordExist != null)
                {
                    isRecordExist.FirstYear = agencyRepair.FirstYear;
                    isRecordExist.SecondYear = agencyRepair.SecondYear;
                    isRecordExist.ThirdYear = agencyRepair.ThirdYear;
                    isRecordExist.FourthYear = agencyRepair.FourthYear;
                    isRecordExist.FifthYear = agencyRepair.FifthYear;
                    isRecordExist.BankId = agencyRepair.BankId;
                    isRecordExist.CreatedBy = userId;
                    isRecordExist.ModifiedDate = DateTime.Now;
                    _autoleasingAgencyRepairRepository.Update(isRecordExist);
                }
                else
                {
                    agencyRepair.CreatedDate = DateTime.Now;
                    agencyRepair.CreatedBy = userId;
                    _autoleasingAgencyRepairRepository.Insert(agencyRepair);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public AutoleasingAgencyRepair GetAgencyRepairByBankId(int bankId)
        {
            try
            {
                return _autoleasingAgencyRepairRepository.Table.Where(x => x.BankId == bankId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsAgencyRepairExistWithBank(AutoleasingAgencyRepair agencyRepair)
        {
            try
            {
                var isExist = _autoleasingAgencyRepairRepository.Table.Where(x => x.BankId == agencyRepair.BankId && x.FirstYear == agencyRepair.FirstYear && x.SecondYear == agencyRepair.SecondYear && x.ThirdYear == agencyRepair.ThirdYear && x.FourthYear == agencyRepair.FourthYear && x.FifthYear == agencyRepair.FifthYear).FirstOrDefault();
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
