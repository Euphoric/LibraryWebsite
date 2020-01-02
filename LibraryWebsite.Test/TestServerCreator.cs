using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace LibraryWebsite
{
    public static class TestServerCreator
    {
        public static TestServer CreateTestServer(ITestOutputHelper outputHelper)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Logging:LogLevel:Default", "Warning"},
                    {"Security:Secret", "ABCDEFGHIJKLMNOPQRSTUVWXYZ"}
                })
                .Build();

            var builder = WebHost.CreateDefaultBuilder()
                .UseConfiguration(config)
                .UseEnvironment(Environments.Production)
                .UseStartup<Startup>()
                .ConfigureTestServices(TestingStartup.ConfigureServices)

                // override default startup configuration for testing purposes
                .ConfigureLogging(logging =>
                    logging
                        .ClearProviders()
                        .AddConfiguration(config.GetSection("Logging"))
                        .AddXUnit(outputHelper));
                ;

            var testServer = new TestServer(builder);
            testServer.BaseAddress = new Uri("https://localhost/"); // use HTTPS for all requests
            return testServer;
        }
    }
}