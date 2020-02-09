using System;
using System.Collections.Generic;
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
    public class IdentityAuthenticationApiTest : IAsyncLifetime
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public IdentityAuthenticationApiTest(ITestOutputHelper outputHelper)
        {
            _testServer = TestServerCreator.CreateTestServer(outputHelper);
            _client = _testServer.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await TestServerCreator.AddTestingUsers(_testServer);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Discovery_document_retrieved()
        {
            var discoveryDocumentResponse = await _client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocumentResponse.IsError);
        }

        [Fact]
        public async Task Client_authenticated()
        {
            var discoveryDocument = await _client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocument.IsError);

            using var request = new ClientCredentialsTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PublicApi"
            };
            var response = await _client.RequestClientCredentialsTokenAsync(request);

            Assert.False(response.IsError, response.Error);
            Assert.NotNull(response.AccessToken);
        }

        [Fact]
        public async Task User_authenticated()
        {
            var discoveryDocument = await _client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocument.IsError);

            using var request = new PasswordTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PublicApi",

                UserName = "Admin",
                Password = "Administrator_1"
            };
            var response = await _client.RequestPasswordTokenAsync(request);

            Assert.False(response.IsError, response.Error);
            Assert.NotNull(response.AccessToken);
        }

                private async Task LoginAsUser(string userName, string password)
        {
            var discoveryDocument = await _client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocument.IsError);

            using var request = new PasswordTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PublicApi",

                UserName = userName,
                Password = password
            };
            var response = await _client.RequestPasswordTokenAsync(request);

            Assert.False(response.IsError, response.Error);
            Assert.NotNull(response.AccessToken);

            _client.SetBearerToken(response.AccessToken);
        }

        [Fact]
        public async Task Authenticates_admin_with_correct_credentials()
        {
            await LoginAsUser("Admin", "Administrator_1");

            var response2 = await _client.GetAsync("api/identity/testAdmin");
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.Equal("Admin authenticated!", await response2.Content.ReadAsStringAsync());

            var response3 = await _client.GetAsync("api/identity/testLibrarian");
            Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
            Assert.Equal("Admin authenticated!", await response3.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Unauthenticated_user_should_401()
        {
            var response = await _client.GetJsonErrorResponseAsync("api/identity/testAdmin");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Authenticates_user_with_correct_credentials()
        {
            await LoginAsUser("User", "User_1");

            var response2 = await _client.GetJsonErrorResponseAsync("api/identity/testAdmin");
            Assert.Equal(HttpStatusCode.Forbidden, response2.StatusCode);

            var response3 = await _client.GetAsync("api/identity/testLibrarian");
            Assert.Equal("User authenticated!", await response3.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Retrieves_LibraryWebsite_Oidc_client_configuration()
        {
            var response = await _client.GetJsonAsync<Dictionary<string, string>>("_configuration/LibraryWebsite");

            Assert.Equal(6, response.Count);

            Assert.Equal(_testServer.BaseAddress+"authentication/login-callback", response["redirect_uri"]);
            Assert.Equal(_testServer.BaseAddress+"authentication/logout-callback", response["post_logout_redirect_uri"]);
        }
    }
}