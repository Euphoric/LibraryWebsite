using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Xunit;

namespace LibraryWebsite.TestEndToEnd.Identity
{
    public class IdentityAuthenticationSpec
    {
        [Fact]
        public async Task Discovery_document_retrieved()
        {
            using var httpClientHandler = new HttpClientHandler
            {
                // ignores certificate errors
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            using var client = new HttpClient(httpClientHandler) {BaseAddress = WebAddresses.WebsiteUri};

            var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocumentResponse.IsError, discoveryDocumentResponse.Error);
        }

        [Fact]
        public async Task Client_authenticated()
        {
            using var httpClientHandler = new HttpClientHandler
            {
                // ignores certificate errors
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            using var client = new HttpClient(httpClientHandler) {BaseAddress = WebAddresses.WebsiteUri};

            var discoveryDocument = await client.GetDiscoveryDocumentAsync();
            Assert.False(discoveryDocument.IsError);

            using var request = new ClientCredentialsTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "PublicApi"
            };
            var response = await client.RequestClientCredentialsTokenAsync(request);

            Assert.False(response.IsError, response.Error);
            Assert.NotNull(response.AccessToken);
        }
    }
}