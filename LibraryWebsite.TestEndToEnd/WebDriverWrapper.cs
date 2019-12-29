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
    public class WebDriverWrapper : IWebDriver
    {
        private readonly IWebDriver _driver;
        private readonly string _homeUrl;
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
                _homeUrl = WebAddresses.LocalUri;
                _driver = new ChromeDriver();
            }
            else
            {
                _homeUrl = WebAddresses.WebsiteUri;
                ChromeOptions options = new ChromeOptions();
                options.PlatformName = "linux";
                _driver = new RemoteWebDriver(new Uri(seleniumHubUri), options);
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

                var ss = ((ITakesScreenshot)_driver).GetScreenshot();
                ss.SaveAsFile(screenshotName);
            }
            finally
            {
                _driver.Quit();
            }
        }

        internal void NavigateHome()
        {
            _driver.Navigate().GoToUrl(_homeUrl);
        }

        internal void NavigateTo(string page)
        {
            _driver.Navigate().GoToUrl(_homeUrl + page);
        }

        #region IWebDriver

        public string Url { get => _driver.Url; set => _driver.Url = value; }

        public string Title => _driver.Title;

        public string PageSource => _driver.PageSource;

        public string CurrentWindowHandle => _driver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => _driver.WindowHandles;

        public void Close()
        {
            _driver.Close();
        }

        public void Quit()
        {
            _driver.Quit();
        }

        public IOptions Manage()
        {
            return _driver.Manage();
        }

        public INavigation Navigate()
        {
            return _driver.Navigate();
        }

        public ITargetLocator SwitchTo()
        {
            return _driver.SwitchTo();
        }

        public IWebElement FindElement(By by)
        {
            return _driver.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _driver.FindElements(by);
        }

        #endregion
    }
}