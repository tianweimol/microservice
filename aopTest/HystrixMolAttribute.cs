using AspectCore.DynamicProxy;
using Polly;
using System;
using System.Threading.Tasks;

namespace aopTest
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HystrixMolAttribute: AbstractInterceptorAttribute 
    {
        #region property
        /// <summary>
        /// 最多重试几次，如果为0则不重试
        /// </summary>
        public int MaxRetryTimes { get; set; } = 0;
        /// <summary>
        /// 重试间隔的毫秒数
        /// </summary>
        public int RetryIntervalMilliseconds { get; set; } = 100;
        /// <summary>
        /// 是否启用熔断
        /// </summary>
        public bool EnableCircuitBreaker { get; set; } = false;
        /// <summary>
        /// 熔断前出现允许错误几次
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 3;
        /// <summary>
        /// 熔断多长时间（毫秒）
        /// </summary>
        public int MillisecondsOfBreak { get; set; } = 1000;
        /// <summary>
        /// 执行超过多少毫秒则认为超时（0表示不检测超时）
        /// </summary>
        public int TimeOutMilliseconds { get; set; } = 0;
        /// <summary>
        /// 缓存多少毫秒（0表示不缓存），用“类名+方法名+所有参数ToString拼接”做缓存Key
        /// </summary>
        public int CacheTTLMilliseconds { get; set; } = 0;
        public string FallBackMethod { get; set; }
        private Policy policy;
        private static readonly Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache
        = new Microsoft.Extensions.Caching.Memory.MemoryCache(new
        Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
        #endregion property
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            lock (this)
            {
                if (policy == null)
                {
                    // 从外向内第0层
                    policy = Policy.NoOpAsync();
                }
                // 从外向内第1层  熔断策略
                if (EnableCircuitBreaker)
                {
                    var circuitBreakerPolicy = Policy.Handle<Exception>()
                        .CircuitBreakerAsync(ExceptionsAllowedBeforeBreaking, TimeSpan.FromMilliseconds(MillisecondsOfBreak));
                    policy.WrapAsync(circuitBreakerPolicy);
                }
                // 从外向内第2层  超时策略
                if (TimeOutMilliseconds > 0)
                {
                    var timeoutPolicy = Policy.TimeoutAsync(()=> TimeSpan.FromMilliseconds(TimeOutMilliseconds),Polly.Timeout.TimeoutStrategy.Pessimistic);
                    policy = policy.WrapAsync(timeoutPolicy);
                }
                // 从外向内第3层  重试策略
                if (MaxRetryTimes > 0)
                {
                    var retryPolicy = Policy.Handle<Exception>().RetryAsync(MaxRetryTimes,(ee,i)=> {
                        // 重试第i次，异常是ee
                    });
                }
                // 从外向内第4层 降级策略
                if (!string.IsNullOrEmpty(FallBackMethod))
                {
                    var fallbackPolicy = Policy.Handle<Exception>()
                        .FallbackAsync(async (pContext,t)=> {// ctx是polly的上下文
                            AspectContext aspectContext = (AspectContext)pContext["aspecContext"];
                            var fallbackMethod = aspectContext.ServiceMethod.DeclaringType.GetMethod(this.FallBackMethod);
                            var fallbackResult = fallbackMethod.Invoke(aspectContext.Implementation, aspectContext.Parameters);
                            aspectContext.ReturnValue = fallbackResult;
                        },async (ex,t)=> { });
                    policy = policy.WrapAsync(fallbackPolicy);
                }
            }
            // 把aop上下文传给Polly，pollyContext是媒介
            var pollyContext = new Context();
            pollyContext["aspecContext"] = context;

            if (CacheTTLMilliseconds > 0)
            {
                string cacheKey = $"HystrixMolCache-Key-{context.ServiceMethod.DeclaringType}-{context.ServiceMethod.Name}-{string.Join("-", context.Parameters)}";
                
                if (memoryCache.TryGetValue(cacheKey, out var cachevalue))
                {
                    context.ReturnValue = cachevalue;
                }
                else
                {
                    // 如果缓存中没有，那我要执行实际方法喽
                    await policy.ExecuteAsync(task => next(context), pollyContext);
                    // 把结果放入缓存
                    using (var cacheEntry = memoryCache.CreateEntry(cacheKey))
                    {
                        cacheEntry.Value = context.ReturnValue;
                        cacheEntry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMilliseconds(CacheTTLMilliseconds);
                    }
                }
            }
            else
            {
                // 如果不启用缓存，则直接返回被拦截方法的结果
                await policy.ExecuteAsync(task=>next(context),pollyContext);
            }
        }
    }
}
