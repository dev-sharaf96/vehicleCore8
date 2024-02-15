using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Payments
{
    /// <summary>
    /// Wallet Transaction History Filter
    /// </summary>
    public class WalletTransactionHistoryFilter
    {
        public int? AccountId { get; set; }
        public decimal? Amount { get; set; }
        public string BalanceChangedBy { get; set; }
    }
}
