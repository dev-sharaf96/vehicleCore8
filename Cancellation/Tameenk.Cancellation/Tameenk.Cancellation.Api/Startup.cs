using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tameenk.Cancellation.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tameenk.Cancellation.BLL.Caching;
using Tameenk.Cancellation.BLL.Business;
using Tameenk.Cancellation.Api.Extensions;

namespace Tameenk.Cancellation.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContextPool<CancellationContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDistributedMemoryCache();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDependencyInjectionBusiness();
            //services.AddScoped<IReasonLookup, ReasonLookup>();
            //services.AddScoped<ICachingEngine, CachingEngine>();

            // Add cors
            services.AddCors();

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAnyOrigin",
            //        builder => builder
            //        .AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader());
            //});

            //services.Configure<MvcOptions>(options => {
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAnyOrigin"));
            //});

            services.AddLogging();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env , ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            //Configure Cors
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());



            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
