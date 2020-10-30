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
            wait.Until(dr => dr.FindElements(By.XPath("//table/tbody/tr")).Count == defaultRowCount);

            // paging

            var selectedTitle = wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text);

            {
                var nextPageLink = _driver.FindElement(By.ClassName("pagination-next"));
                nextPageLink.Click();
            }
            wait.Until(dr => dr.Url.Contains("book-list/2", StringComparison.InvariantCulture));

            wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text != selectedTitle);

            selectedTitle = wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text);

            var lastPageLink = _driver.FindElement(By.ClassName("pagination-last"));
            lastPageLink.Click();
            
            wait.Until(dr => dr.FindElement(By.XPath("//table//tr[3]/td")).Text != selectedTitle);

            // TODO: Fix
            //{
            //    var nextPageLink = _driver.FindElement(By.ClassName("pagination-next"));
            //    Assert.Contains("disabled", nextPageLink.GetAttribute("class"), StringComparison.InvariantCulture);
            //}
        }

        [Fact]
        public void Books_add()
        {
            _driver.NavigateHome();

            _driver.LoginAsLibrarian();

            WebDriverWait wait = new WebDriverWait(_driver, System.TimeSpan.FromSeconds(15));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            IWebElement booksLink = wait.Until(driver => driver.FindElement(By.Id("navigate-books")));
            booksLink.Click();

            IWebElement addBookLink = wait.Until(driver => driver.FindElement(By.Id("books-add")));
            addBookLink.Click();

            IWebElement titleText = wait.Until(driver => driver.FindElement(By.Id("titleInput")));
            titleText.SendKeys("testBook");

            IWebElement isbn13Text = wait.Until(driver => driver.FindElement(By.Id("isbn13Input")));
            isbn13Text.SendKeys("testIsbn");

            IWebElement authorText = wait.Until(driver => driver.FindElement(By.Id("authorInput")));
            authorText.SendKeys("testAuthor");

            IWebElement descriptionText = wait.Until(driver => driver.FindElement(By.Id("descriptionInput")));
            descriptionText.SendKeys("testDescription");

            IWebElement saveLink = wait.Until(driver => driver.FindElement(By.Id("book-save")));
            saveLink.Click();
        }
    }
}
