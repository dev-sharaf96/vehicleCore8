using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tameenk.Cancellation.BLL.Business;
using Tameenk.Cancellation.BLL.Caching;

namespace Tameenk.Cancellation.Api.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependencyInjectionBusiness(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICachingEngine, CachingEngine>();

            serviceCollection.AddScoped<IReasonLookup, ReasonLookup>();
            serviceCollection.AddScoped<IErrorCodeLookup, ErrorCodeLookup>();
            serviceCollection.AddScoped<IInsuranceTypeLookup, InsuranceTypeLookup>();
            serviceCollection.AddScoped<IBankCodeLookup, BankCodeLookup>();
            serviceCollection.AddScoped<IVehicleIDTypeLookup, VehicleIDTypeLookup>();
            serviceCollection.AddScoped<IInsuranceCompanyBusiness, InsuranceCompanyBusiness>();
        }

    }
}
