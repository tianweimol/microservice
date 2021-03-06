﻿using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace aopTest
{
    public class HystrixTimeoutAttribute : AbstractInterceptorAttribute
    {
        private string _timeoutMethod;
        public HystrixTimeoutAttribute(string timeoutMethod)
        {
            this._timeoutMethod = timeoutMethod;
        }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine("Hystrix Before");
            try
            {
                await next(context);
            }
            catch (Exception ee)
            {
                Console.WriteLine($"{context.ImplementationMethod.Name}出错了，异常信息是：{ee.Message}");

                //// 获得降级的方法
                //var fallbackMethod = context.Implementation.GetType().GetMethod(_fallBackMethod);
                //// 调用降级的方法 
                //var result=fallbackMethod.Invoke(context.Implementation,context.Parameters);
                //// 把降级方法的返回值返回
                //context.ReturnValue = result;

                context.ReturnValue = context.Implementation.GetType().GetMethod(_fallBackMethod).Invoke(context.Implementation, context.Parameters);
            }
            Console.WriteLine("Hystrix After");
        }
    }
}
