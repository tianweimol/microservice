using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ID4.Ocelot
{
    public class Startup
    {

        private readonly IConfiguration Configuration;


        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // id4
            // cache服务需要的验证 这TM都可以写在配置文件中
            Action<IdentityServerAuthenticationOptions> isaOptCache = c =>
            {
                c.Authority = "http://127.0.0.1:9500";
                c.ApiName = "缓存服务";
                c.RequireHttpsMetadata = false;
                c.SupportedTokens = SupportedTokens.Both;
                c.ApiSecret = "a1";
            };
            // order服务需要的验证
            Action<IdentityServerAuthenticationOptions> isaOptOrder = o =>
            {
                o.Authority = "http://127.0.0.1:9500";
                o.ApiName = "订单服务名";// 要和ID4中config提供的apiResource中的名字一样,也要和consul中的服务名一样。
                o.RequireHttpsMetadata = false;
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "a1";
            };


            // 把上面定义好的谁附加到配置文件定义的服务上。注意，下面的cachekey一定要和配置文件中一致
            services.AddAuthentication()
                .AddIdentityServerAuthentication("CacheKey", isaOptCache)
                .AddIdentityServerAuthentication("OrderKey", isaOptOrder);

            // 网关
            services.AddOcelot(Configuration);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOcelot().Wait();
            app.UseMvc();
        }
    }
}
