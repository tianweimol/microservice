using AspectCore.DynamicProxy;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace aopTest
{
    class HystrixRetryAttribute : AbstractInterceptorAttribute
    {
        private string _retryMethod;
        private int _retryTimes;
        public HystrixRetryAttribute(string retryMethod,int retryTimes)
        {
            this._retryMethod = retryMethod;
            this._retryTimes = retryTimes;
        }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine("Hystrix Before");
            Policy policy = Policy.Handle<Exception>()
                .Retry(_retryTimes, (ee, i) =>
                {
                    if (i >= _retryTimes)
                    {
                        Console.WriteLine($"重试次数达到{_retryTimes}次，我不来啦 ！");
                    }
                    else
                    {
                        Console.WriteLine($"当前是方法在重试，重试第{i}次");
                    }
                });
            await policy.Execute(async () =>
            {
                await next(context);
            });
            Console.WriteLine("Hystrix After");
        }
    }
}
