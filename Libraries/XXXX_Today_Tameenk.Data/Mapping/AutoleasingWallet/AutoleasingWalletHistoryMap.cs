using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.AutoleasingWallet;

namespace Tameenk.Data.Mapping.AutoleasingWallet
{
   public class AutoleasingWalletHistoryMap:EntityTypeConfiguration<AutoleasingWalletHistory>
    {
        public AutoleasingWalletHistoryMap()
        {
            ToTable("AutoleasingWalletHistory");
            HasKey(e => e.Id);
        }
    }
}