using Consul;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var consul = new ConsulClient(c => { c.Address = new System.Uri("http://127.0.0.1:8500"); c.Datacenter = "数据中心1"; }))
            {
                var services=consul.Agent.Services().Result.Response;
                foreach (var s in services.Values)
                {
                    Console.WriteLine($"id={s.ID},service={s.Service},address={s.Address},port={s.Port}");
                }
                services.Values.FirstOrDefault(t=>t.Service.Equals("我的服务名字"));
                var service=services.Values.First();
                Console.WriteLine($"'我的服务名字'的信息：id={service.ID},service={service.Service},address={service.Address},port={service.Port}");
                using (HttpClient httpClient = new HttpClient())
                //using (var httpContent = new StringContent("{username:'mol'}", encoding: System.Text.Encoding.UTF8))
                using (var httpContent = new StringContent("'mol'",  System.Text.Encoding.UTF8, "application/json"))// 单参数
                //using (var httpContent=new StringContent(JsonConvert.SerializeObject(new postModel()), System.Text.Encoding.UTF8, "application/json"))// 多参数
                //using (var httpContent = new StringContent("hahahahahahahaha", System.Text.Encoding.UTF8, "application/json"))
                {
                    //Console.WriteLine(httpContent.ReadAsStringAsync().Result);
                    var r = httpClient.PostAsync($"http://{service.Address}:{service.Port}/api/health", httpContent).Result;
                    var pr = httpClient.GetAsync($"http://{service.Address}:{service.Port}/api/health").Result;
                    Console.WriteLine(r);
                    Console.WriteLine(r.Content.ReadAsStringAsync().Result);
                    Console.WriteLine(pr);
                    Console.WriteLine(pr.Content.ReadAsStringAsync().Result);
                }
            }
            //Console.ReadKey();
        }
    }

    class postModel
    {
        public string username { get; set; } = "mol";
    }
}
