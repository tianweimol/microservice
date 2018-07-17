using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aopTest;

namespace WebapiTest.Models
{
    public class Person
    {
        [Aspect]
        public virtual void Say(string msg)
        {
            Console.WriteLine($"我在说话，内容是：{msg}");
        }

        [HystrixMol(FallBackMethod = nameof(Fourth), EnableCircuitBreaker = true, ExceptionsAllowedBeforeBreaking = 3, MaxRetryTimes = 4,MillisecondsOfBreak =2000)]
        public async virtual Task<string> Third(int num)
        {
            var re = 1000 / num + "";
            Console.WriteLine($"Third的结果是：{re}");
            return re;
        }


        public async virtual Task<string> Fourth(int num)
        {
            var re = 100 / (num + 2) + "";
            Console.WriteLine($"Fourth的结果是：{re}");
            return re;
        }
    }
}
