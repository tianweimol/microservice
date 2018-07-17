using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Order.Controllers
{
    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : Controller
    {
        [HttpGet]
        public string Get()
            {
            var username = this.User.Identity?.Name;
            var userId = this.User.FindFirst("UserId")?.Value;
            return $"恭喜您{username}，下单成功,您的主键是：{userId}";
        }
    }
}