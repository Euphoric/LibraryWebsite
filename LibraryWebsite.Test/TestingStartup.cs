using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LibraryWebsite
{
    public static class TestingStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.RemoveAll<DbContextOptions<LibraryContext>>();
            var dbName = Guid.NewGuid().ToString();
            services.AddDbContext<LibraryContext>(options => options.UseInMemoryDatabase(dbName));
        }
    }
}
