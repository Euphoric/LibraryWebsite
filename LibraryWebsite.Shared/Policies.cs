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

        public static void AddPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(Policies.IsAdmin, policy => { policy.RequireClaim(ClaimTypes.Role, Role.Admin); });
            options.AddPolicy(Policies.IsLibrarian, policy => { policy.RequireClaim(ClaimTypes.Role, Role.Librarian); });
            options.AddPolicy(Policies.CanEditBooks, policy => policy.RequireClaim(ClaimTypes.Role, Role.Librarian));
        }
    }
}