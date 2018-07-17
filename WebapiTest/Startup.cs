using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.DependencyInjection;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebapiTest.Models;

namespace WebapiTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<Person>();
            // 通过反射遍历某个程序集中加了[HystrixMol标签的类]来注册 
            loadHystrixClassAsync(this.GetType().Assembly.FullName,services).Wait();
            // 由aspectcore接管di的功能
            return services.BuildAspectCoreServiceProvider();
        }

        private async Task loadHystrixClassAsync(string assemblyName,IServiceCollection services )
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null) return;
            foreach (var type in assembly.GetExportedTypes())
            {
                // 判断方法上面是否有HystrixMol注解
                bool hasHystrixMolAttribute = type.GetMethods()
                    .Any(t => t.GetCustomAttribute(typeof(aopTest.HystrixMolAttribute)) != null);
                if (hasHystrixMolAttribute)
                {
                    services.AddSingleton(type);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            string ip = Configuration["ip"];
            int.TryParse(Configuration["port"], out int port);

            var serviceName = "我的服务名字";
            var serviceId = $"{serviceName}-{Guid.NewGuid()}";
            //Action<ConsulClientConfiguration> initClient = (ConsulClientConfiguration c) =>
            //{
            //    c.Address = new Uri("http://127.0.0.1:8500");
            //    c.Datacenter = "数据中心1";
            //};
            using (var client = new ConsulClient((ConsulClientConfiguration c) =>
            {
                c.Address = new Uri("http://127.0.0.1:8500");
                c.Datacenter = "数据中心1";
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
                        c.Address = new Uri("http://127.0.0.1:8500");
                        c.Datacenter = "数据中心1";
                    }))
                    {
                        Console.WriteLine("应用退出，并从consul注销");
                        client.Agent.ServiceDeregister(serviceId).Wait();
                    }
                }
                );
        }


    }
}
