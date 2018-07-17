using AspectCore.DynamicProxy;
using System;
using System.Reflection;

namespace aopTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(typeof(Person).Assembly.FullName);
            Console.WriteLine(typeof(Person).Assembly.GetName());
            Console.WriteLine(Assembly.LoadFrom("WebapiTest").FullName);
            return;
            // 必须使用这种方法来得到Person类，不能直接new一个person出来
            ProxyGeneratorBuilder proxyGeneratorBuilder = new ProxyGeneratorBuilder();
            using (IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build())
            {
                var person = proxyGenerator.CreateClassProxy<Person>();
                Console.WriteLine($"主函数的结果是：{person.Third(1).Result}"); 
            }
        }
    }
}
