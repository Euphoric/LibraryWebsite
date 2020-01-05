using System;

namespace LibraryWebsite.TestEndToEnd
{
    public static class WebAddresses
    {
        public static string LocalAddress { get; } = "localhost:5001";

        public static string LocalUri
        {
            get
            {
                return "https://" + LocalAddress;
            }
        }

        public static string WebsiteUri
        {
            get
            {
                return "https://" + (Environment.GetEnvironmentVariable("WEB_ADDRESS") ?? LocalAddress);
            }
        }
    }
}
