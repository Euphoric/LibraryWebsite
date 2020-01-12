using System;

namespace LibraryWebsite.TestEndToEnd
{
    public static class WebAddresses
    {
        public static string LocalAddress { get; } = "localhost:5001";

        public static Uri LocalUri
        {
            get
            {
                var address = "https://" + LocalAddress;
                return new Uri(address);
            }
        }

        public static Uri WebsiteUri
        {
            get
            {
                var address = "https://" + (Environment.GetEnvironmentVariable("WEB_ADDRESS") ?? LocalAddress);
                return new Uri(address);
            }
        }
    }
}
