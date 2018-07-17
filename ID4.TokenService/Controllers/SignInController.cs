using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ID4.TokenService.Controllers
{
    [Produces("application/json")]
    [Route("api/SignIn")]
    public class SignInController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            var dic = new Dictionary<string, string>()
            {
                { "client_id","clientPC1"},
                { "client_secret","a1"},
                { "grant_type","password"},
                { "username",username},
                { "password",password}
            };
            using (var httpClient = new HttpClient())
            using (var content = new FormUrlEncodedContent(dic))
            {
                var msg = await httpClient.PostAsync("http://127.0.0.1:9500", content);
                var result = await msg.Content.ReadAsStringAsync();
                return Json(result);
            }
        }
    }
}