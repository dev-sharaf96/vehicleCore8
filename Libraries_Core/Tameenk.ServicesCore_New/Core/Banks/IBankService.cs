using System.Collections.Generic;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.AutoleasingWallet;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Services.Implementation;
using Tameenk.Services.Implementation.Banks;
using Tameenk.Services.Implementation.Drivers;

namespace Tameenk.Services.Core
{
    public interface IBankService
    {
        Bank GetBank(int id);

        Bank AddBank(string bankNameAr, string bankNameEn, string Iban, string NationalAddress, string PhoneNumber, string Email);
        List<Bank> GetBanks();
        bool DeleteBank(Bank bank);

        bool EditBank(Bank bank);

        bool EditBankIban(int bankId, string Iban);

        IList<Bank> GetBankByName(string bankname);
        IPagedList<Bank> GetBanksWithFilter(IList<Bank> query, int pageIndex = 0, int pageSize = int.MaxValue);
        void updateBankForWallet(Bank bank, out string exception);
        List<AutoleasingWalletHistory> GetAutoleasingWalletHistory(AutoleasingWalletHistoryFilterModel filter, int bankId, int pageIndex, int pageSize, out int totalCount, out string exception);
        List<AutoleasingWalletReportModel> GetAutoleasingWalletReport(AutoleasingWalletHistoryFilterModel filter, int bankId, out int totalCount, out string exception);
    }
}
