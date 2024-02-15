using System.Collections.Generic;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Core
{
    public interface ICorporateAccountService
    {
        List<CorporateAccount> GetCorporateAccounts();
        CorporateAccount GetCorporateAccountById(int accountId);
        List<CorporateAccountModel> GetCorporateAccountWithFilter(CorporateAccountFilter filterModel, int pageIndex, int pageSize, int commandTimeout, bool export, out int totalCount, out string exception);
        CorporateAccount GetCorporateAccount(int accountId);
        void AddOrupdateCororateAccount(CorporateAccount account, out string exception);
        WalletOutput AddBalanceToWallet(WalletAddBalanceModel addBalanceModel);
    }
}
