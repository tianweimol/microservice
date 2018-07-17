using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace aopTest
{
    public class AspectAttribute:AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine($"我要开始注入了！");
                await next(context);
            }
            catch (Exception ee)
            {
                Console.WriteLine($"异常信息：{ee.Message}");
            }
            finally
            {
                Console.WriteLine("注入最后");
            }
        }
    }
}
