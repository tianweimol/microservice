using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebapiTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();
            var ip = config["ip"];
            var port = config["port"];
            Console.WriteLine($"ip={ip},port={port}");
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://{ip}:{port}")
                .Build();
        }
        //WebHost.CreateDefaultBuilder(args)
        //            .UseStartup<Startup>()
        //        .UseUrls("http://127.0.0.1:9800")
        //            .Build();

        public static IWebHostBuilder CreateWebhostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            string ip = config["ip"];
            int.TryParse(config["port"], out int port);

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
                    Address = ip,// 服务提供者能被消费者访问到的ip
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


            return WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls($"http://{ip}:{port}");
        }

        // 程序正常退出时，从consul注销
       

    }
}
