using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.TestEndToEnd
{
    public sealed class WebOpensSpec : IDisposable
    {
        private readonly WebDriverWrapper _driver;

        public WebOpensSpec(ITestOutputHelper output)
        {
            _driver = WebDriverWrapper.Open(output);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        [Fact(Skip = "FIX for Blazor")]
        public void Welcome_is_present()
        {
            _driver.NavigateHome();
            
            WebDriverWait wait = new WebDriverWait(_driver,System.TimeSpan.FromSeconds(15));

            IWebElement element = wait.Until(driver => driver.FindElement(By.XPath("//app-home//h1")));
            
            Assert.Equal("Welcome!", element.Text);
        }
    }
}
