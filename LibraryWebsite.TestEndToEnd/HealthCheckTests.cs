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
            var webAddress = WebAddresses.WebsiteUri;

            using HttpClient client = new HttpClient();
            var result = await client.GetStringAsync(webAddress + "/health");
            Assert.Equal("Healthy", result);
        }
    }
}
