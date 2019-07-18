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
using Xunit;

namespace LibraryWebsite
{
    public class BookApiTest
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public BookApiTest()
        {
            var builder = WebHost.CreateDefaultBuilder()
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
        public async Task Retrieves_books()
        {
            var resultStr = await _client.GetStringAsync("api/book");
            var json = (dynamic)Newtonsoft.Json.JsonConvert.DeserializeObject(resultStr);
            Assert.Equal(1, json.Count);
        }
    }
}
