using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LibraryWebsite.TestEndToEnd
{
    public class HealthCheckTests
    {
        [Fact]
        public async Task Checks_health_status()
        {
            using var httpClientHandler = new HttpClientHandler
            {
                // ignores certificate errors
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            using var client = new HttpClient(httpClientHandler);

            var webAddress = WebAddresses.WebsiteUri;
            var result = await client.GetAsync(new Uri(webAddress + "health"));
            var content = await result.Content.ReadAsStringAsync();
            Assert.True(result.IsSuccessStatusCode, content);
            Assert.Equal("Healthy", content);
        }
    }
}
