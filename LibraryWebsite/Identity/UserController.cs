using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebsite.Identity
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public class UserDto
        {
            public string? Id { get; set; }

            public string? UserName { get; set; }
        }

        [Authorize(Policy = Policies.IsAdmin)]
        [HttpGet]
        public async Task<PagingResult<UserDto>> GetUsers()
        {
            return await _userManager.Users.Select(usr => new UserDto()
            {
                Id = usr.Id,
                UserName = usr.UserName
            })
                .CreatePaging(1000, 0);
        }
    }
}
