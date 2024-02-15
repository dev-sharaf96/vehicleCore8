using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tameenk.Cancellation.DAL.Entities;
using Tameenk.Cancellation.DAL.Extensions;

namespace Tameenk.Cancellation.DAL
{
    public class CancellationContext : DbContext
    {
        public CancellationContext(DbContextOptions<CancellationContext> options)
            : base(options)
        {

        }

        #region Enitities

        #region Logging
        public DbSet<ServiceRequestLog> ServiceRequestLogs { get; set; }
        #endregion

        #region Lookups
        public DbSet<BankCode> BankCodes { get; set; }
        public DbSet<ErrorCode> ErrorCodes { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<InsuranceType> InsuranceTypes { get; set; }
        public DbSet<VehicleIDType> VehicleIDTypes { get; set; }
        #endregion

        public DbSet<CancellationRequest> CancellationRequests { get; set; }
        public DbSet<InsuranceCompany> InsuranceCompanies { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          //  optionsBuilder.EnableSensitiveDataLogging(true);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankCode>().HasKey(x => x.Code);
            //  modelBuilder.Entity<BaseLookup>().HasKey(x => x.Code);
            modelBuilder.Entity<ErrorCode>().HasKey(x => x.Code);
            modelBuilder.Entity<ServiceRequestLog>().HasKey(x => x.Id);
            modelBuilder.Entity<CancellationRequest>().HasKey(x => x.Id);
            modelBuilder.Entity<InsuranceCompany>().HasKey(x => x.Id);
            // modelBuilder.Entity<BaseEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<Reason>().HasKey(x => x.Code);
            modelBuilder.Entity<VehicleIDType>().HasKey(x => x.Code);
            modelBuilder.Entity<InsuranceType>().HasKey(x => x.Code);


            modelBuilder.Entity<Reason>().Property(p => p.Code).UseSqlServerIdentityColumn();
            modelBuilder.Entity<VehicleIDType>().Property(p => p.Code).UseSqlServerIdentityColumn();
            modelBuilder.Entity<InsuranceType>().Property(p => p.Code).UseSqlServerIdentityColumn();

            modelBuilder.Entity<ServiceRequestLog>().Property(p => p.Id).UseSqlServerIdentityColumn();
            modelBuilder.Entity<CancellationRequest>().Property(p => p.Id).UseSqlServerIdentityColumn();
            modelBuilder.Entity<InsuranceCompany>().Property(p => p.Id).UseSqlServerIdentityColumn();
            //modelBuilder.Entity<InsuranceCompany>().Property(p => p.CreatedDate).ValueGeneratedOnAdd();
            //modelBuilder.Entity<InsuranceCompany>().Property(p => p.ModifiedDate).ValueGeneratedOnUpdate();

            #region Required
            modelBuilder.Entity<InsuranceCompany>().Property(p => p.CompanyName).IsRequired();
            modelBuilder.Entity<InsuranceCompany>().Property(p => p.UserName).IsRequired();
            modelBuilder.Entity<InsuranceCompany>().Property(p => p.Password).IsRequired();
            modelBuilder.Entity<InsuranceCompany>().Property(p => p.GetPolicyServiceUrl).IsRequired();
            modelBuilder.Entity<InsuranceCompany>().Property(p => p.PolicyCancellationServiceUrl).IsRequired();
            modelBuilder.Entity<InsuranceCompany>().Property(p => p.CreditNoteScheduleServiceUrl).IsRequired();

            modelBuilder.Entity<CancellationRequest>().Property(p => p.InsuredId).IsRequired();
            modelBuilder.Entity<CancellationRequest>().Property(p => p.VehicleId).IsRequired();
            modelBuilder.Entity<CancellationRequest>().Property(p => p.ReasonCode).IsRequired();
            modelBuilder.Entity<CancellationRequest>().Property(p => p.VehicleIdTypeCode).IsRequired();
            modelBuilder.Entity<CancellationRequest>().Property(p => p.CancelFromCompany).IsRequired();
            #endregion

            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }


        #region ConfigurSaveChanges


        public override int SaveChanges()
        {
            UpdateAuditEntities();
            return base.SaveChanges();
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }


        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                DateTime now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                    entity.IsActive = true;
                    // entity.CreatedBy = CurrentUserId;
                }
                else
                {
                    //base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }

                entity.ModifiedDate = now;
                //entity.UpdatedBy = CurrentUserId;
            }
        }

        #endregion
    }
}
