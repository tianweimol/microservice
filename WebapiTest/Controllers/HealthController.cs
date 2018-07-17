using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebapiTest.Controllers
{
    [Produces("application/json")]
    [Route("api/Health")]
    public class HealthController : Controller
    {
        [HttpGet]
        public String Get()
        {
            Console.WriteLine($"检查时间：{DateTime.Now}");
            return "我是一个健康检查的Get请求";
        }
        [HttpGet("username")]
        public string GetUserName(string username)
        {
            return $"你好：{username}";
        }
        [HttpPost]
        public string Post([FromBody]string username)
        {
            Console.WriteLine($"{username}:我是一个Post方法，用来让消费者调用的！");
            return $"{username}:我是一个Post方法，用来让消费者调用的！";
        }

        [HttpPost("Post")]
        public string TestPost([FromBody] PostData data)
        {
            return $"你好：{data.userName}，你今年{data.Age}岁";
        }
    }

    public class PostData
    {
        public string userName { get; set; } = "mol";
        public int Age { get; set; } = 33;
    }
}