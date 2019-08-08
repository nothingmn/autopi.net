using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using autopi.react.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace autopi.react.Controllers
{
    [Route("api/[controller]")]

    public class TripsController : Controller
    {
        [HttpGet()]
        public string Trips()
        {
            var auth = Request.HttpContext.GetAutoApiAuthToken();
            return auth;
        }
    }
}