﻿using LibraryWebsite.Books;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Options;
using LibraryWebsite.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Options;

namespace LibraryWebsite
{
    public class LibraryContext : ApiAuthorizationDbContext<ApplicationUser>
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public LibraryContext(
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
            DbContextOptions<LibraryContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            :base(options, operationalStoreOptions)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
