using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace aopTest
{
    public class Person
    {
        [Aspect]
        public virtual void Say(string msg)
        {
            Console.WriteLine($"我在说话，内容是：{msg}");
        }

        //[HystrixFallBack(nameof(Second))]
        [HystrixRetry(nameof(Second),6)]
        public async virtual Task<string> First(int num)
        {
            var re = 100 / num + "";
            Console.WriteLine($"First结果是：{re}");
            return re;
        }

        public async virtual Task<string> Second(int num)
        {
            var re = 100 / (num+2) + "";
            Console.WriteLine($"Second结果是：{re}");
            return re;
        }
        [HystrixMol(FallBackMethod =nameof(Fourth),EnableCircuitBreaker =true,ExceptionsAllowedBeforeBreaking =3,MaxRetryTimes =4)]
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
