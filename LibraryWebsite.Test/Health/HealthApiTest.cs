using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LibraryWebsite.Health
{
    public class HealthApiTest
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public HealthApiTest()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "Security:Secret", "ABCDEFGHIJKLMNOPQRSTUVWXYZ" } })
                .Build();

            var builder = WebHost.CreateDefaultBuilder()
                            .UseConfiguration(config)
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

        [Fact]
        public async Task Calls_ping()
        {
            var result = await _client.GetStringAsync("health");
            Assert.Equal("Healthy", result);
        }
    }
}
