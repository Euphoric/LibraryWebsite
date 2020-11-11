using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebsite.Identity
{
    [Authorize]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        [Authorize(Policy = Policies.IsAdmin)]
        [HttpGet("testAdmin")]
        public string TestAdmin()
        {
            var userName = HttpContext.User.Identity?.Name;
            return userName + " authenticated!";
        }

        [Authorize(Policy = Policies.IsLibrarian)]
        [HttpGet("testLibrarian")]
        public string TestLibrarian()
        {
            var userName = HttpContext.User.Identity?.Name;
            return userName + " authenticated!";
        }
    }
}
