using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebsite.Diagnostics
{
    [Route("api/[controller]")]
    public class DiagnosticController : Controller
    {

        [HttpGet("[action]")]
        public string Ping()
        {
            return "Success!";
        }

    }
}
