using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LibraryWebsite.Users
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<AuthenticatedUser> Authenticate([FromBody]AuthenticateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Authenticate request was not well-formed",
                    Errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage)
                });
            }

            bool isLoggedIn = 
                (request.Username == "Admin" && request.Password == "Administrator") ||
                (request.Username == "User" && request.Password == "UserPass")
                ;

            if (!isLoggedIn)
            {
                return BadRequest(new { Message = "Username or password is incorrect" });
            }

            string[] roles = request.Username == "Admin" ? new[] {Role.Admin, Role.User} : new[] {Role.User};
            var token = new JwtAuthentication().GenerateSecurityToken(request.Username, roles);

            return new AuthenticatedUser { Username = request.Username, Token = token };
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("testAdmin")]
        public string TestAdmin()
        {
            var userName = HttpContext.User.Identity.Name;
            return userName + " authenticated!";
        }

        [Authorize(Roles = Role.User)]
        [HttpGet("testUser")]
        public string TestUser()
        {
            var userName = HttpContext.User.Identity.Name;
            return userName + " authenticated!";
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
