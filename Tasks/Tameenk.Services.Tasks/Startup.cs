using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Policy.Components;

[assembly: OwinStartup(typeof(Tameenk.Services.Tasks.Startup))]

namespace Tameenk.Services.Tasks
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            EngineContext.Initialize(false);
            //ConfigureAuth(app);
            AutoMapperConfiguration.Init();
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
        }
    }
}
