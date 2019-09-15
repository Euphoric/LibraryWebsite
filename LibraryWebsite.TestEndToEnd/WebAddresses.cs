using System;

namespace LibraryWebsite.TestEndToEnd
{
    public static class WebAddresses
    {
        public static string LocalAddress { get; private set; } = "localhost:58419";

        public static string LocalUri
        {
            get
            {
                return "http://" + LocalAddress;
            }
        }

        public static string WebsiteUri
        {
            get
            {
                return "http://" + (Environment.GetEnvironmentVariable("WEB_ADDRESS") ?? LocalAddress);
            }
        }
    }
}
