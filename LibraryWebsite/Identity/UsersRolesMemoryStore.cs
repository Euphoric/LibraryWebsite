using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace LibraryWebsite.Identity
{
    public class UsersRolesMemoryStore
    {
        public List<ApplicationUser> Users { get; } = new List<ApplicationUser>();
        public List<IdentityUserRole<string>> UserRoles { get; } = new List<IdentityUserRole<string>>();
        public List<IdentityUserClaim<string>> UserClaims { get; } = new List<IdentityUserClaim<string>>();
        
        public List<IdentityRole> Roles {get;} = new List<IdentityRole>()
        {
            new IdentityRole("Admin") {NormalizedName = "ADMIN"},
            new IdentityRole("Librarian") {NormalizedName = "LIBRARIAN"},
            new IdentityRole("Reader") {NormalizedName = "READER"}
        };
    }
}