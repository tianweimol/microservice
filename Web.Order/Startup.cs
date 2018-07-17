﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Order.Models;

namespace Web.Order
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //private IOptions<AppSettingModel> _appsettingModel;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 高能预警：一定要先注册验证，再注册mvc
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://127.0.0.1:9500";//identity server 地址
                    options.RequireHttpsMetadata = false;
                });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime, IOptions<AppSettingModel> appsettingModel)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 高能预警：一定要先注册验证，再注册mvc
            app.UseAuthentication();
            app.UseMvc();
            //_appsettingModel = appsettingModel;
            string ip = Configuration["ip"];
            int.TryParse(Configuration["port"], out int port);

            var serviceName = "订单服务名";// _appsettingModel.Value.ServiceName;
            var serviceId = $"{serviceName}-{Guid.NewGuid()}";

            using (var client = new ConsulClient((ConsulClientConfiguration c) =>
            {
                //c.Address = new Uri($"{_appsettingModel.Value.Schema}{_appsettingModel.Value.ConsulIp}:{_appsettingModel.Value.ConsulPort}");// new Uri("http://127.0.0.1:8500");
                //c.Datacenter = _appsettingModel.Value.DataCenter;

                c.Address = new Uri("http://127.0.0.1:8500");
                c.Datacenter = "订单中心";
            }))
            {
                client.Agent.ServiceRegister(new AgentServiceRegistration()
                {
                    ID = serviceId,
                    Name = serviceName,
                    Address = ip,// 服务提供者能被消费者访问到的ip  localhost 127
                    Port = port,
                    Check = new AgentServiceCheck()// 健康检查
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),// 服务停止5秒后注销
                        Interval = TimeSpan.FromSeconds(10),// 10秒做一次健康检查（10秒向服务器发一次心跳）
                        HTTP = $"http://{ip}:{port}/api/health",// 健康检查的地址
                        Timeout = TimeSpan.FromSeconds(3)// 健康检查的超时时间

                    }
                }).Wait();
            }
            applicationLifetime.ApplicationStopped.Register(
                () =>
                {
                    using (var client = new ConsulClient((ConsulClientConfiguration c) =>
                    {
                        //c.Address = new Uri($"{_appsettingModel.Value.Schema}{_appsettingModel.Value.ConsulIp}:{_appsettingModel.Value.ConsulPort}");// new Uri("http://127.0.0.1:8500");
                        //c.Datacenter = _appsettingModel.Value.DataCenter;

                        c.Address =  new Uri("http://127.0.0.1:8500");
                        c.Datacenter = "订单中心";
                    }))
                    {
                        Console.WriteLine("订单应用退出，并从consul注销");
                        client.Agent.ServiceDeregister(serviceId).Wait();
                    }
                }
                );
            
        }
    }
}