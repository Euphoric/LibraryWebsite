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

            Assert.Equal(new[] { "Title", "Author", "Description", "ISBN-13" }, headerTexts);

            const int defaultRowCount = 10;
            wait.Until(dr => dr.FindElements(By.XPath("//table/tbody/tr")).Count == defaultRowCount);

            // paging

            var selectedTitle = wait.Until(dr => dr.FindElement(By.XPath("//table//tr[1]/td")).Text);

            {
                var nextPageLink = _driver.FindElement(By.ClassName("pagination-next"));
                nextPageLink.Click();
            }
            wait.Until(dr => dr.Url.Contains("book-list/2", StringComparison.InvariantCulture));

            wait.Until(dr => dr.FindElement(By.XPath("//table//tr[1]/td")).Text != selectedTitle);

            selectedTitle = wait.Until(dr => dr.FindElement(By.XPath("//table//tr[1]/td")).Text);

            var lastPageLink = _driver.FindElement(By.ClassName("pagination-last"));
            lastPageLink.Click();

            wait.Until(dr => dr.FindElement(By.XPath("//table//tr[1]/td")).Text != selectedTitle);

            // TODO: Fix
            //{
            //    var nextPageLink = _driver.FindElement(By.ClassName("pagination-next"));
            //    Assert.Contains("disabled", nextPageLink.GetAttribute("class"), StringComparison.InvariantCulture);
            //}
        }

        [Fact]
        public void Books_add_and_edit()
        {
            _driver.NavigateHome();

            _driver.LoginAsLibrarian();

            WebDriverWait wait = new WebDriverWait(_driver, System.TimeSpan.FromSeconds(15));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            wait.Until(driver => driver.FindElement(By.Id("navigate-books"))).Click();

            // create new Book
            {
                wait.Until(driver => driver.FindElement(By.Id("books-add"))).Click();

                wait.Until(driver => driver.FindElement(By.Id("titleInput"))).SendKeys("testBook");

                wait.Until(driver => driver.FindElement(By.Id("isbn13Input"))).SendKeys("testIsbn");

                wait.Until(driver => driver.FindElement(By.Id("authorInput"))).SendKeys("testAuthor");

                wait.Until(driver => driver.FindElement(By.Id("descriptionInput"))).SendKeys("testDescription");

                wait.Until(driver => driver.FindElement(By.Id("book-save"))).Click();
            }

            Assert.Contains("Book changed successfully.", wait.Until(driver => driver.FindElement(By.Id("book-changed-alert"))).Text, StringComparison.InvariantCulture);

            // Assert that new book has correct values
            {
                wait.Until(driver => driver.FindElement(By.Id("book-changed-alert-link"))).Click();

                Assert.Equal("testBook", wait.Until(driver => driver.FindElement(By.Id("titleInput"))).GetAttribute("value"));

                Assert.Equal("testIsbn", wait.Until(driver => driver.FindElement(By.Id("isbn13Input"))).GetAttribute("value"));

                Assert.Equal("testAuthor", wait.Until(driver => driver.FindElement(By.Id("authorInput"))).GetAttribute("value"));

                Assert.Equal("testDescription", wait.Until(driver => driver.FindElement(By.Id("descriptionInput"))).GetAttribute("value"));
            }

            // edit book
            {
                wait.Until(driver => driver.FindElement(By.Id("titleInput"))).ClearAndSendKeys("testBook X");

                wait.Until(driver => driver.FindElement(By.Id("isbn13Input"))).ClearAndSendKeys("testIsbn X");

                wait.Until(driver => driver.FindElement(By.Id("authorInput"))).ClearAndSendKeys("testAuthor X");

                wait.Until(driver => driver.FindElement(By.Id("descriptionInput"))).ClearAndSendKeys("testDescription X");

                wait.Until(driver => driver.FindElement(By.Id("book-save"))).Click();
            }

            // Assert book was edited correctly
            {
                wait.Until(driver => driver.FindElement(By.Id("book-changed-alert-link"))).Click();

                Assert.Equal("testBook X", wait.Until(driver => driver.FindElement(By.Id("titleInput"))).GetAttribute("value"));

                Assert.Equal("testIsbn X", wait.Until(driver => driver.FindElement(By.Id("isbn13Input"))).GetAttribute("value"));

                Assert.Equal("testAuthor X", wait.Until(driver => driver.FindElement(By.Id("authorInput"))).GetAttribute("value"));

                Assert.Equal("testDescription X", wait.Until(driver => driver.FindElement(By.Id("descriptionInput"))).GetAttribute("value"));
            }
        }
    }
}
