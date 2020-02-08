using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LibraryWebsite.TestEndToEnd
{
    public sealed class UserLoginSpec : IDisposable
    {
        private readonly WebDriverWrapper _driver;

        public UserLoginSpec(ITestOutputHelper output)
        {
            _driver = WebDriverWrapper.Open(output);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        private static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        [Fact]
        public void User_registers_and_logs_in()
        {
            _driver.NavigateTo("");

            WebDriverWait wait = new WebDriverWait(_driver, System.TimeSpan.FromSeconds(15));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            var userEmail = RandomString(8) + ".test@test.com";
            var userPassword = "Abcdefgh!1";

            {
                IWebElement loginLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"register_user\"]")));
                loginLink.Click();

                IWebElement emailInput = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"Input_Email\"]")));
                emailInput.SendKeys(userEmail);

                IWebElement passwordInput =  wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"Input_Password\"]")));
                passwordInput.SendKeys(userPassword);

                IWebElement passwordConfirmInput = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"Input_ConfirmPassword\"]")));
                passwordConfirmInput.SendKeys(userPassword);

                IWebElement registerButton = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"registerSubmit\"]")));
                registerButton.Click();

                var manageUserLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"manage_user\"]")));
                Assert.Contains(userEmail, manageUserLink.Text, StringComparison.InvariantCultureIgnoreCase);

                IWebElement logoutLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"logout_user\"]")));
                logoutLink.Click();
            }

            {
                IWebElement loginLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"login_user\"]")));
                loginLink.Click();

                IWebElement emailInput = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"Input_Email\"]")));
                emailInput.SendKeys(userEmail);

                IWebElement passwordInput =  wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"Input_Password\"]")));
                passwordInput.SendKeys(userPassword);

                IWebElement registerButton = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"login-submit\"]")));
                registerButton.Click();

                var manageUserLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"manage_user\"]")));
                Assert.Contains(userEmail, manageUserLink.Text, StringComparison.InvariantCultureIgnoreCase);

                IWebElement logoutLink = wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"logout_user\"]")));
                logoutLink.Click();
            }
        }
    }
}
