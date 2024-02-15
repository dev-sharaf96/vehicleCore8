using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Tameenk.Services.Administration.Identity.Core.Domain;
using Tameenk.Services.Administration.Identity.Core.Repositories;

namespace Tameenk.Services.Administration.Identity.Repositories
{
    [DbConfigurationType(typeof(Tameenk.Data.EfDbConfiguration))]
    public class AdminIdentityContext : IdentityDbContext<AppUser, RoleEntity, int, UserLogin, UserRole, UserClaim>, IIdentityDbContext
    {

        public AdminIdentityContext() : base("AdminIdentityContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        //public AdminIdentityContext() : base("Data Source =.\\SQL2017;Initial Catalog = TameenkIdentity_Test; user id = sa; password=123456;Integrated Security = false; MultipleActiveResultSets=True;")
        //{
        //    this.Configuration.LazyLoadingEnabled = false;
        //    this.Configuration.ProxyCreationEnabled = false;
        //}

        //public AdminIdentityContext() : base("Data Source =.\\SQL2017;Initial Catalog = TameenkIdentity_Test; user id = sa; password=123456;Integrated Security = false; MultipleActiveResultSets=True;")
        //{
        //    this.Configuration.LazyLoadingEnabled = false;
        //    this.Configuration.ProxyCreationEnabled = false;
        //}


        public static AdminIdentityContext Create()
        {
            return new AdminIdentityContext();
        }


        //public DbSet<UserRole> UserRoles { get; set; }
        //public DbSet<UserPage> UserPages { get; set; }
        //public DbSet<Page> Pages { get; set; }
        //public DbSet<IdentityLog> IdentityLogs { get; set; }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Page>().HasKey(x => x.Id);
            modelBuilder.Entity<IdentityLog>().HasKey(x => x.Id);
            modelBuilder.Entity<UserPage>().HasKey(a => new { a.PageId, a.UserId });

            modelBuilder.Entity<Page>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<IdentityLog>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<UserRole>().HasKey(a => new { a.RoleId, a.UserId });
            modelBuilder.Entity<UserLogin>().HasKey(a => new { a.UserId });
            modelBuilder.Entity<ExpiredTokens>().HasKey(a => new { a.Id });
            modelBuilder.Entity<UserLoginsConfirmation>().HasKey(a => new { a.Id });
            
            modelBuilder.Entity<UsersLocationsDeviceInfo>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }


    }
}
