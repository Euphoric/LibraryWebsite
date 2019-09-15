using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Xunit.Abstractions;

namespace LibraryWebsite.TestEndToEnd
{
    public class WebDriverWrapper : IDisposable, IWebDriver
    {
        private readonly IWebDriver driver;
        public readonly string homeURL;
        private readonly string _testName;

        internal static WebDriverWrapper Open(ITestOutputHelper output)
        {
            return new WebDriverWrapper(output);
        }

        public WebDriverWrapper(ITestOutputHelper output)
        {
            _testName = GetTestName(output);

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

        private static string GetTestName(ITestOutputHelper output)
        {
            var type = output.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = (ITest)testMember.GetValue(output);
            var testName = test.DisplayName;
            return testName;
        }

        public void Dispose()
        {
            try
            {
                var basePath = "../../../../TestResults";
                string screenshotName = Path.Combine(basePath, "Screenshot", _testName + ".png");
                Directory.CreateDirectory(Path.GetDirectoryName(screenshotName));

                var ss = ((ITakesScreenshot)driver).GetScreenshot();
                ss.SaveAsFile(screenshotName);
            }
            finally
            {
                driver.Quit();
            }
        }

        internal void NavigateHome()
        {
            driver.Navigate().GoToUrl(homeURL);
        }

        internal void NavigateTo(string page)
        {
            driver.Navigate().GoToUrl(homeURL + page);
        }

        #region IWebDriver

        public string Url { get => driver.Url; set => driver.Url = value; }

        public string Title => driver.Title;

        public string PageSource => driver.PageSource;

        public string CurrentWindowHandle => driver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => driver.WindowHandles;

        public void Close()
        {
            driver.Close();
        }

        public void Quit()
        {
            driver.Quit();
        }

        public IOptions Manage()
        {
            return driver.Manage();
        }

        public INavigation Navigate()
        {
            return driver.Navigate();
        }

        public ITargetLocator SwitchTo()
        {
            return driver.SwitchTo();
        }

        public IWebElement FindElement(By by)
        {
            return driver.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return driver.FindElements(by);
        }

        #endregion
    }
}