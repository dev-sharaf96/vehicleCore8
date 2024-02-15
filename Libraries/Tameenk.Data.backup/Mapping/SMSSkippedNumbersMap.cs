using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class SMSSkippedNumbersMap : EntityTypeConfiguration<SMSSkippedNumbers>
    {
        public SMSSkippedNumbersMap()
        {
            ToTable("SMSSkippedNumbers");
            HasKey(e => e.Id);
        }
    }
}
