using System;
using OpenQA.Selenium;

namespace LibraryWebsite.TestEndToEnd
{
    public static class WebElementExtensions
    {
        public static void ClearAndSendKeys(this IWebElement element, string text)
        {
            element.Clear();
            element.SendKeys(text);
        }
    }
}