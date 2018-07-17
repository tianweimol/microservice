using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Cache.Controllers
{
    [Produces("application/json")]
    [Route("api/Cache")]
    public class CacheController : Controller
    {
        [HttpGet]
        public string Get(string key = "Nop.setting.All") => $"您请求的键是：{key}";
    }
}