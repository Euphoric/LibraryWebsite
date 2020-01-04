using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace LibraryWebsite
{
    public static class TestServerCreator
    {
        public static TestServer CreateTestServer(ITestOutputHelper outputHelper)
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("Testing")
                .UseStartup<Startup>()
                .ConfigureTestServices(TestingStartup.ConfigureServices)
                .ConfigureLogging(logging => logging.AddXUnit(outputHelper));

            var testServer = new TestServer(builder);
            testServer.BaseAddress = new Uri("https://localhost/"); // use HTTPS for all requests
            return testServer;
        }
    }
}