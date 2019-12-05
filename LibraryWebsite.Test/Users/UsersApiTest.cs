using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite.Users
{

    public class UsersApiTest
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public UsersApiTest()
        {
            var builder = WebHost.CreateDefaultBuilder()
                            .UseEnvironment(Environments.Production)
                            .UseStartup<Startup>()
                            .ConfigureTestServices(TestingStartup.ConfigureServices)

                            // override default startup configuration for testing purposes
                            .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning).AddConsole())
                            ;

            _testServer = new TestServer(builder);
            _testServer.BaseAddress = new Uri("https://localhost/"); // use HTTPS for all requests
            _client = _testServer.CreateClient();
        }

        private class AuthenticateRequestDto
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }

        private class AuthenticatedUserDto
        {
            public string Username { get; set; }
        }

        private class ErrorResponse
        {
            public string Message { get; set; }

            public string[] Errors { get; set; }
        }

        [Fact]
        public async Task Empty_credentials_is_invalid_request()
        {
            var authentication = new AuthenticateRequestDto { };
            var errorResponse = await _client.PostJsonErrorResponseAsync("api/users/authenticate", authentication);

            Assert.Equal(HttpStatusCode.BadRequest, errorResponse.StatusCode);

            var responseJson = JsonSerializer.Deserialize<ErrorResponse>(errorResponse.ErrorContent, JsonSerializerOptionsProvider.Options);
            Assert.Equal(2, responseJson.Errors.Length);
        }

        [Fact]
        public async Task Empty_username_is_invalid_request()
        {
            var authentication = new AuthenticateRequestDto { Password = "PASS" };
            var errorResponse = await _client.PostJsonErrorResponseAsync("api/users/authenticate", authentication);

            Assert.Equal(HttpStatusCode.BadRequest, errorResponse.StatusCode);
        }

        [Fact]
        public async Task Empty_password_is_invalid_request()
        {
            var authentication = new AuthenticateRequestDto { Username = "USER" };
            var errorResponse = await _client.PostJsonErrorResponseAsync("api/users/authenticate", authentication);

            Assert.Equal(HttpStatusCode.BadRequest, errorResponse.StatusCode);
        }

        [Fact]
        public async Task Invalid_user_doesnt_authenticate()
        {
            var authentication = new AuthenticateRequestDto { Username = "USER", Password = "PASS" };
            var errorResponse = await _client.PostJsonErrorResponseAsync("api/users/authenticate", authentication);

            Assert.Equal(HttpStatusCode.BadRequest, errorResponse.StatusCode);
        }

        [Fact]
        public async Task Authenticates_user_with_correct_credentials()
        {
            var authentication = new AuthenticateRequestDto { Username = "Admin", Password = "Administrator" };
            var response = await _client.PostJsonAsync<AuthenticatedUserDto>("api/users/authenticate", authentication);

            Assert.Equal("Admin", response.Username);
        }
    }
}
