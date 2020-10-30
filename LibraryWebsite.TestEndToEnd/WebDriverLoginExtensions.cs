using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace LibraryWebsite.TestEndToEnd
{
    public static class WebDriverLoginExtensions
    {
        public static void LoginAsLibrarian(this IWebDriver driver)
        {
            string userEmail = "librarian@sample.com";
            string userPassword = "Abcdefgh!1";

            WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(15));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            IWebElement loginLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"login_user\"]")));
            loginLink.Click();

            IWebElement emailInput = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"Input_Email\"]")));
            emailInput.SendKeys(userEmail);

            IWebElement passwordInput =  wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"Input_Password\"]")));
            passwordInput.SendKeys(userPassword);

            IWebElement loginButton = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"login-submit\"]")));
            loginButton.Click();

            IWebElement manageUserLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"manage_user\"]")));
            Assert.Contains(userEmail, manageUserLink.Text, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}