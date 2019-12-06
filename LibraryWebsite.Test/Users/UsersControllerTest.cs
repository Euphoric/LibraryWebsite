using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LibraryWebsite.Users
{
    public class UsersControllerTest
    {
        readonly UsersController _controller;

        public UsersControllerTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "Security:Secret", "ABCDEFGHIJKLMNOPQRSTUVWXYZ" } })
                .Build();

            _controller = new UsersController(config);
        }

        [Fact]
        public Task User_authentication()
        {
            var request = new AuthenticateRequest() { Username = "Admin", Password = "Administrator" };
            var response  = _controller.Authenticate(request);

            Assert.Null(response.Result);

            Assert.Equal("Admin", response.Value.Username);
            Assert.NotNull(response.Value.Token);

            var request2 = new AuthenticateRequest() { Username = "User", Password = "UserPass" };
            var response2 = _controller.Authenticate(request2);

            Assert.Null(response2.Result);

            Assert.Equal("User", response2.Value.Username);
            Assert.NotNull(response2.Value.Token);

            Assert.NotEqual(response.Value.Token, response2.Value.Token);

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
