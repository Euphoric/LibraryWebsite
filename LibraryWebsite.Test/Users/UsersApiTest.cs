using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            _testServer = TestServerCreator.CreateTestServer();
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

            public string Token { get; set; }
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
        public async Task Authenticates_admin_with_correct_credentials()
        {
            var authentication = new AuthenticateRequestDto { Username = "Admin", Password = "Administrator" };
            var response = await _client.PostJsonAsync<AuthenticatedUserDto>("api/users/authenticate", authentication);

            Assert.Equal("Admin", response.Username);
            Assert.NotNull(response.Token);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);

            var response2 = await _client.GetAsync("api/users/testAdmin");
            Assert.Equal("Admin authenticated!", await response2.Content.ReadAsStringAsync());

            var response3 = await _client.GetAsync("api/users/testUser");
            Assert.Equal("Admin authenticated!", await response3.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Unauthenticated_user_should_401()
        {
            var response = await _client.GetJsonErrorResponseAsync("api/users/testAdmin");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Authenticates_user_with_correct_credentials()
        {
            var authentication = new AuthenticateRequestDto { Username = "User", Password = "UserPass" };
            var response = await _client.PostJsonAsync<AuthenticatedUserDto>("api/users/authenticate", authentication);

            Assert.Equal("User", response.Username);
            Assert.NotNull(response.Token);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);

            var response2 = await _client.GetJsonErrorResponseAsync("api/users/testAdmin");
            Assert.Equal(HttpStatusCode.Forbidden, response2.StatusCode);

            var response3 = await _client.GetAsync("api/users/testUser");
            Assert.Equal("User authenticated!", await response3.Content.ReadAsStringAsync());
        }
    }
}
