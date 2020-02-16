using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.Identity
{
    public class UserApiTest : IAsyncLifetime
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public UserApiTest(ITestOutputHelper outputHelper)
        {
            _testServer = TestServerCreator.CreateTestServer(outputHelper);
            _client = _testServer.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _testServer.Host.Services.AddTestingUsers();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Needs_admin_priviledges_to_get_users()
        {
            var anynymousResponse = await _client.GetAsync("api/user");
            Assert.Equal(HttpStatusCode.Unauthorized, anynymousResponse.StatusCode);

            await _client.LoginAsLibrarian();
            var librarianResponse = await _client.GetAsync("api/user");
            Assert.Equal(HttpStatusCode.Forbidden, librarianResponse.StatusCode);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class UserDto
        {
            public string? Id { get; set; }

            public string? UserName { get; set; }
        }

        [Fact]
        public async Task Retrieves_users()
        {
            await _client.LoginAsAdmin();
            var usersResponse = await _client.GetJsonAsync<PagingResultDto<UserDto>>("api/user");
            
            Assert.Equal(3, usersResponse.TotalCount);
            Assert.False(usersResponse.Items.All(usr=>string.IsNullOrWhiteSpace(usr.Id)));
            Assert.False(usersResponse.Items.All(usr=>string.IsNullOrWhiteSpace(usr.UserName)));
        }
    }
}