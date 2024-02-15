using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain;

namespace Tameenk.Data.Mapping.Payments.HyperPay
{
    public class HyperPayNotificationTransactionMap : EntityTypeConfiguration<HyperPayNotificationTransactions>
    {
        public HyperPayNotificationTransactionMap()
        {
            ToTable("HyperPayNotificationTransactions");
            HasKey(e => e.Id);
        }
    }
}
