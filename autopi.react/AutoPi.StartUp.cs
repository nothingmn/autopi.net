using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace autopi.react
{
    public class AutoPiStartUp
    {
        //Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var startup = new autopi.net.core.Startup();
            startup.Initialize().Wait();
            services.AddSingleton<autopi.net.core.Startup>(startup);
            services.AddSingleton<autopi.net.core.ILogger>(startup.Logger);
            services.AddSingleton<autopi.net.core.tags.IMetaDataStore>(startup.MetaDataStorage);


        }
        //Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}