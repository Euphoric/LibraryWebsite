using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.TestEndToEnd
{
    public class BooksSpec : IDisposable
    {
        private readonly WebDriverWrapper _driver;

        public BooksSpec(ITestOutputHelper output)
        {
            _driver = WebDriverWrapper.Open(output);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        [Fact]
        public void Books_list()
        {
            _driver.NavigateTo("/book-list");

            WebDriverWait wait = new WebDriverWait(_driver, System.TimeSpan.FromSeconds(15));

            IWebElement title = wait.Until(driver => driver.FindElement(By.XPath("//h1")));

            Assert.Equal("Books", title.Text);

            var table = wait.Until(driver => driver.FindElement(By.XPath("//table")));

            IReadOnlyCollection<IWebElement> headers = table.FindElements(By.XPath("thead//th"));
            var headerTexts = headers.Select(th => th.Text).ToArray();

            Assert.Equal(new[] { "Title", "Author", "Description", "ISBN-13", "", "" }, headerTexts);
        }
    }
}
