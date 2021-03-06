﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ID4.IdServer
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
            //services.AddMvc();
            //services.AddIdentityServer()
            //        .AddDeveloperSigningCredential()
            //        .AddInMemoryApiResources(Config.GetApiResources())
            //        .AddInMemoryClients(Config.GetClients());

            var idResources = new List<IdentityResource>
                    {
                    new IdentityResources.OpenId(), //必须要添加，否则报无效的 scope 错误
                    new IdentityResources.Profile()
                    };
            services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryIdentityResources(idResources)
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryClients(Config.GetClients())//
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            .AddProfileService<ProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // id4服务器只提供身价验证，所以可以不开启mvc
            //app.UseMvc();
            app.UseIdentityServer();
        }
    }
}
