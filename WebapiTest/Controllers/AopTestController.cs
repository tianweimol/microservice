using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebapiTest.Models;

namespace WebapiTest.Controllers
{
    [Produces("application/json")]
    [Route("api/AopTest")]
    public class AopTestController : Controller
    {
        private Person _person;
        public AopTestController(Person person)
        {
            this._person = person;
        }
        [HttpGet]
        public async Task< string> GetMethod(int num)=>await this._person.Third(num);
        
    }
}