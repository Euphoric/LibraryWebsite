using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite.Users
{
    public class UsersControllerTest
    {
        readonly UsersController _controller;

        public UsersControllerTest()
        {
            _controller = new UsersController();
        }

        [Fact]
        public Task User_authentication()
        {
            var request = new AuthenticateRequest() { Username = "Admin", Password = "Administrator" };
            var response  = _controller.Authenticate(request);

            Assert.Null(response.Result);

            Assert.Equal("Admin", response.Value.Username);
            Assert.NotNull(response.Value.Token);

            return Task.CompletedTask;
        }

        [Fact]
        public Task Existing_user_incorrect_password()
        {
            var request = new AuthenticateRequest() { Username = "Admin", Password = "abcd" };
            var response = _controller.Authenticate(request);
            
            Assert.IsType<BadRequestObjectResult>(response.Result);

            return Task.CompletedTask;
        }
    }
}
