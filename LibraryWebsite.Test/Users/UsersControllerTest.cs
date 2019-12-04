using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite.Users
{
    public class UsersControllerTest
    {
        UsersController controller;

        public UsersControllerTest()
        {
            controller = new UsersController();
        }

        [Fact]
        public Task User_authentication()
        {
            var request = new AuthenticateRequest() { Username = "Admin", Password = "Administrator" };
            var response  = controller.Authenticate(request);

            Assert.IsType<OkResult>(response);
            
            return Task.CompletedTask;
        }

        [Fact]
        public Task Existing_user_incorrect_password()
        {
            var request = new AuthenticateRequest() { Username = "Admin", Password = "abcd" };
            var response = controller.Authenticate(request);

            Assert.IsType<BadRequestObjectResult>(response);

            return Task.CompletedTask;
        }
    }
}
