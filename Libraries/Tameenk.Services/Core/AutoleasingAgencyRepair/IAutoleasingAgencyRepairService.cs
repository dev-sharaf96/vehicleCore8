using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
namespace Tameenk.Services.Implementation
{
    public interface IAutoleasingAgencyRepairService
    {

        AutoleasingAgencyRepair GetAgencyRepairByBankId(int bankId);
        bool ConfirmUpdateOrAddAgencyRepair(AutoleasingAgencyRepair agencyRepair, string userId);
        bool IsAgencyRepairExistWithBank(AutoleasingAgencyRepair agencyRepair);

    }
}
