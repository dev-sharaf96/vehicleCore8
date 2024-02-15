using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Tameenk.Core.Infrastructure;
//using Tameenk.Services.Policy.Components;

[assembly: OwinStartup(typeof(Tameenk.Startup))]
namespace Tameenk
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //initialize engine context
            EngineContext.Initialize(false);
            ConfigureAuth(app);
            Tameenk.Services.Quotation.Components.AutoMapperConfiguration.Init();
            //  TaskManager.Instance.Initialize();
            // TaskManager.Instance.Start();
        }
    }
}