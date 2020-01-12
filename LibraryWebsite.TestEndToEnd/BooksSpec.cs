using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.TestEndToEnd
{
    public sealed class BooksSpec : IDisposable
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
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            IWebElement title = wait.Until(driver => driver.FindElement(By.XPath("//h1")));

            Assert.Equal("Books", title.Text);

            var table = wait.Until(driver => driver.FindElement(By.XPath("//table")));

            IReadOnlyCollection<IWebElement> headers = table.FindElements(By.XPath("thead//th"));
            var headerTexts = headers.Select(th => th.Text).ToArray();

            Assert.Equal(new[] { "Title", "Author", "Description", "ISBN-13", "", "" }, headerTexts);

            const int defaultRowCount = 10;
            wait.Until(dr => dr.FindElements(By.XPath("//table//tr")).Count == defaultRowCount + 1);

            // paging

            var selectedTitle = wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text);

            {
                var nextPageLink = _driver.FindElement(By.ClassName("pagination-next"));
                nextPageLink.Click();
            }
            wait.Until(dr => dr.Url.Contains("page=2", StringComparison.InvariantCulture));

            wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text != selectedTitle);

            selectedTitle = wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text);

            var lastPageLink = _driver.FindElement(By.XPath("//pagination-controls//li[position() = (last() - 1)]"));
            lastPageLink.Click();
            
            wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text != selectedTitle);

            {
                var nextPageLink = _driver.FindElement(By.ClassName("pagination-next"));
                Assert.Contains("disabled", nextPageLink.GetAttribute("class"), StringComparison.InvariantCulture);
            }
        }
    }
}
