using System;

namespace LibraryWebsite
{
    /// <summary>
    /// Authentication policies.
    /// </summary>
    public static class Policies
    {
        public const string IsAdmin = "IsAdmin";
        public const string IsUser = "IsUser";
    }
}