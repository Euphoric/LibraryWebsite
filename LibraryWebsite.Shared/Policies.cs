using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LibraryWebsite
{
    /// <summary>
    /// Authentication policies.
    /// </summary>
    public static class Policies
    {
        public const string IsAdmin = "IsAdmin";
        public const string IsLibrarian = "IsLibrarian";
        public const string CanEditBooks = "CanEditBooks";

        public static void AddPolicies(this AuthorizationOptions options, string roleClaim)
        {
            options.AddPolicy(IsAdmin, policy => { policy.RequireClaim(roleClaim, Role.Admin); });
            options.AddPolicy(IsLibrarian, policy => { policy.RequireClaim(roleClaim, Role.Librarian); });
            options.AddPolicy(CanEditBooks, policy => policy.RequireClaim(roleClaim, Role.Librarian));
        }
    }
}