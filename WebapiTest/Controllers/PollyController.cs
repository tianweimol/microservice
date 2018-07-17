using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace WebapiTest.Controllers
{
    [Produces("application/json")]
    [Route("api/Polly")]
    public class PollyController : Controller
    {
        public string Get1(int num)
        {
            Policy policy = Policy.Handle<Exception>()
                .Fallback(() =>
                {
                    Get2(num);
                });
            policy.Execute(()=> {
                return 100 / num + "";
            });
            return "Get1";
        }

        public string Get2(int num)
        {
            return 100 / (num + 2) + "";
        }

        [HttpGet]
        public string First(int num)
        {
            // 策略：我要处理Exception，fallBack(降级)，Retry(重试),CircuitBreaker(熔断)
            Policy policy = Policy.Handle<Exception>()
                .Fallback(() =>
                {
                    Console.WriteLine("降级了");
                    Second(num);
                }, (ee) => { Console.WriteLine(ee.Message); });
            policy.Execute(() =>
            {
                Console.WriteLine("第一个方法开始_降级到Second");
                var re = 10 / num;
            });
            return "first";
        }

        private string Second(int num)
        {
            Policy policy = Policy.Handle<Exception>()
                .Retry(5, (ee, i) =>
                {
                    if (i >= 5)
                    {
                        Console.WriteLine("重试次数达到5次，我要转到Third方法了！");
                        Third(num);
                    }
                    else
                    {
                        Console.WriteLine($"当前是第二个方法在重试，重试第{i}次");
                    }
                });
            //.Fallback(() =>
            // {
            //     Third();
            // });
            policy.Execute(() =>
            {
                Console.WriteLine("第二个方法开始_重试");
                var re = 100 / num;
            });
            return "second";
        }

        private string Third(int num)
        {
            Policy policy = Policy.Handle<Exception>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(20), (ee, ts) =>
                {
                    Console.WriteLine($"等了{ts}秒");
                }, () => { Console.WriteLine("OnReset"); });
            //.Fallback(() => {
            //    Second(num);
            //});
            policy.Execute(() =>
            {
                Console.WriteLine("第三个方法开始_熔断");// 熔断后的代码都不会被执行
                var re = 1000 / num;
            });
            Console.WriteLine("第三个方法结束_熔断");
            return "hahaha";
        }


        [HttpPost]
        public string FallBack(int num)
        {
            // 降级处理
            //Policy<string> policy = Policy<string>.Handle<Exception>()
            //    .Fallback(()=> {
            //        Console.WriteLine("开始降级了");
            //        return (100 / (num+2)).ToString();
            //    });
            //return policy.Execute(()=> {
            //    var re = (100 / num).ToString();
            //    Console.WriteLine($"正常返回是{re}");
            //    return re;
            //});
            // 降级处理
            //Policy<string> policy = Policy<string>.Handle<Exception>(t=>t.Message.Equals("异常内容"))
            //   .Fallback("我是一个降级值");
            //return policy.Execute(() => {
            //    var re = (100 / num).ToString();
            //    Console.WriteLine($"正常返回是{re}");
            //    return re;
            //});
            // 熔断处理
            /*Policy policy = Policy
                    .Handle<Exception>()
                    .CircuitBreaker(5, TimeSpan.FromSeconds(10));//连续出错5次之后熔断10秒(不会再去尝试执行业务代码）。
            while (true)
            {
                Console.WriteLine("开始Execute");
                try
                {
                    policy.Execute(() =>
                    {
                        Console.WriteLine("开始任务");
                        throw new Exception("出错");
                        Console.WriteLine("完成任务");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("execute出错" + ex);
                }
                Thread.Sleep(500);
            }*/
            // 策略打包
            /*
            Policy<string> policyRetry = Policy<string>.Handle<Exception>().Retry(3,(ee,i)=> {
                Console.WriteLine($"当前重试第{i}次");
            });
            Policy<string> policyFallBack = Policy<string>.Handle<Exception>().Fallback(()=> {
                Console.WriteLine("我是一个fallback结果");
                return "我是一个fallback结果";
            });
            Policy<string> policy = policyFallBack.Wrap(policyRetry);
            return policy.Execute(()=> {
                Console.WriteLine("开始Execute");
                if (DateTime.Now.Second % 10 != 0)
                {
                    throw new Exception("出错");
                }
                Console.WriteLine("结束Execute");
                return "结束Execute";
            });*/
            // 超时
            Policy policyTimeOut = Policy.Timeout(2, Polly.Timeout.TimeoutStrategy.Pessimistic);
            Policy<string> policyFallBack = Policy<string>.Handle<Exception>().Fallback(() =>
            {
                Console.WriteLine("我是一个fallback结果");
                return "我是一个fallback结果";
            });
            Policy<string> policy = policyFallBack.Wrap(policyTimeOut);
            return policy.Execute(() =>
            {
                Console.WriteLine("开始睡觉");
                Task.Delay(5000).Wait();
                Console.WriteLine("醒了");
                return "醒了";
            });
        }

    }
}