using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    public class YakeenCityCenterMap : EntityTypeConfiguration<YakeenCityCenter>
    {
        public YakeenCityCenterMap() {
            ToTable("YakeenCityCenter");
            HasKey(c => c.Id);
        }
    }
}