using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebsite.Users
{
    //[Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Authenticate request was not well-formed",
                    Errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage)
                });
            }

            bool isLoggedIn = request.Username == "Admin" && request.Password == "Administrator";

            if (!isLoggedIn)
            {
                return BadRequest(new { Message = "Username or password is incorrect" });
            }

            return Ok();
        }

        //[Authorize(Roles = Role.Admin)]
        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var users = _userService.GetAll();
        //    return Ok(users);
        //}

        //[HttpGet("{id}")]
        //public IActionResult GetById(int id)
        //{
        //    // only allow admins to access other user records
        //    var currentUserId = int.Parse(User.Identity.Name);
        //    if (id != currentUserId && !User.IsInRole(Role.Admin))
        //        return Forbid();

        //    var user = _userService.GetById(id);

        //    if (user == null)
        //        return NotFound();

        //    return Ok(user);
        //}
    }
}
