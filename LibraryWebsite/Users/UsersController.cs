using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LibraryWebsite.Users
{
    //[Authorize]
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

            bool isLoggedIn = request.Username == "Admin" && request.Password == "Administrator";

            if (!isLoggedIn)
            {
                return BadRequest(new { Message = "Username or password is incorrect" });
            }

            var token = GenerateSecurityToken();

            return new AuthenticatedUser {Username = "Admin", Token = token};
        }

        private string GenerateSecurityToken()
        {
            string secret = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string userId = "Admin";
            string userRole = Role.Admin;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.Role, userRole)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        [Authorize(Roles = Role.Admin)]
        [HttpGet("testAdmin")]
        public string TestAdmin()
        {
            return "authenticated!";
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
