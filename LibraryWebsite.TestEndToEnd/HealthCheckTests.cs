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
            var webAddress = "http://" + Environment.GetEnvironmentVariable("WEB_ADDRESS") ?? "http://localhost:54321";

            using HttpClient client = new HttpClient();
            var result = await client.GetStringAsync(webAddress + "/health");
            Assert.Equal("Healthy", result);
        }
    }
}
