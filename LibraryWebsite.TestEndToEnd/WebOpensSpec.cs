using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using Xunit;

namespace LibraryWebsite.TestEndToEnd
{
    public class WebOpensSpec : IDisposable
    {
        private readonly IWebDriver driver;
        public readonly string homeURL;

        public WebOpensSpec()
        {
            var seleniumHubUri = Environment.GetEnvironmentVariable("SELENIUM_HUB");

            if (seleniumHubUri == null)
            {
                homeURL = WebAddresses.LocalUri;
                driver = new ChromeDriver();
            }
            else
            {
                homeURL = WebAddresses.WebsiteUri;
                ChromeOptions options = new ChromeOptions();
                options.PlatformName = "linux";
                driver = new RemoteWebDriver(new Uri(seleniumHubUri), options);
            }
        }

        public void Dispose()
        {
            try
            {
                var basePath = "../../../../TestResults";
                string screenshotName = Path.Combine(basePath, "Screenshot", "screen.png");
                Directory.CreateDirectory(Path.GetDirectoryName(screenshotName));

                var ss = ((ITakesScreenshot)driver).GetScreenshot();
                ss.SaveAsFile(screenshotName);
            }
            finally
            {
                driver.Quit();
            }
        }

        [Fact]
        public void Welcome_is_present()
        {
            driver.Navigate().GoToUrl(homeURL);
            
            WebDriverWait wait = new WebDriverWait(driver,System.TimeSpan.FromSeconds(15));

            IWebElement element = wait.Until(driver => driver.FindElement(By.XPath("//app-home//h1")));

            Assert.Equal("Welcome!", element.Text);
        }
    }
}
