using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.Health
{
    public class HealthApiTest
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public HealthApiTest(ITestOutputHelper outputHelper)
        {
            _testServer = TestServerCreator.CreateTestServer(outputHelper);
            _client = _testServer.CreateClient();
        }

        [Fact]
        public async Task Calls_ping()
        {
            var result = await _client.GetStringAsync("health");
            Assert.Equal("Healthy", result);
        }
    }
}
