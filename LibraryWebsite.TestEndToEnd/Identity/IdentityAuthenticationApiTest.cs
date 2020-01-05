using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using LibraryWebsite.TestEndToEnd;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.Identity
{
    public class IdentityAuthenticationApiTest
    {
        public IdentityAuthenticationApiTest(ITestOutputHelper outputHelper)
        {
            using HttpClient client = new HttpClient();
        }

        [Fact]
        public async Task Discovery_document_retrieved()
        {
            using HttpClient client = new HttpClient {BaseAddress = new Uri(WebAddresses.WebsiteUri)};

            var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocumentResponse.IsError, discoveryDocumentResponse.Error);
        }

        [Fact]
        public async Task Client_authenticated()
        {
            using HttpClient client = new HttpClient {BaseAddress = new Uri(WebAddresses.WebsiteUri)};

            var discoveryDocument = await client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocument.IsError);

            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PublicApi"
            });

            Assert.False(response.IsError, response.Error);
            Assert.NotNull(response.AccessToken);
        }
    }
}