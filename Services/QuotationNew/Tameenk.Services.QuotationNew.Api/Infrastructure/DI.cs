using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Tameenk.Core.Domain.Entities;
using Tameenk.Data;

namespace Tameenk.Services.QuotationNew.Api.Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddQuotationNewServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<TameenkObjectContext>(
                   options =>
                   options.UseSqlServer(configuration.GetConnectionString("Tameenk"),
                   builder => builder.MigrationsAssembly(typeof(TameenkObjectContext).Assembly.FullName)));

            services.AddDbContext<IdentityDbContext>(
                   options =>
                   options.UseSqlServer(configuration.GetConnectionString("AdminIdentityContext"),
                   builder => builder.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));
            
            services.AddDefaultIdentity<AspNetUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityDbContext>();



            return services;
        }
    }
}