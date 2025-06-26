using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Reservation.Tests.Ui
{
    public class AccessControlTests : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;
        private readonly string baseUrl = "http://localhost:5139"; 

        public AccessControlTests()
        {
            var options = new FirefoxOptions();
            options.AddArgument("--disable-gpu");
            options.AddArgument("--ignore-certificate-errors"); // evita erros de HTTPS (caso mude)
            driver = new FirefoxDriver(options);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Log in as a regular user
            driver.Navigate().GoToUrl($"{baseUrl}/account/login");

            var email = wait.Until(d => d.FindElement(By.Id("EmailAddress")));
            email.Clear();
            email.SendKeys("teste@teste.com");

            var pwd = driver.FindElement(By.Id("Password"));
            pwd.Clear();
            pwd.SendKeys("Teste123!");

            var btn = driver.FindElement(By.CssSelector("button[type='submit'], input[type='submit']"));
            btn.Click();

            wait.Until(d => d.FindElement(By.LinkText("Ver Salas Disponíveis")));
        }

        [Fact(DisplayName = "Regular user should be blocked from creating rooms")]
        public void ShouldBlockAccessToRoomCreatePage()
        {
            string relativeUrl = "/Room/Create";
            driver.Navigate().GoToUrl($"{baseUrl}{relativeUrl}");

            bool accessBlocked = driver.PageSource.Contains("Access Denied", StringComparison.OrdinalIgnoreCase)
                || driver.PageSource.Contains("Acesso negado", StringComparison.OrdinalIgnoreCase)
                || driver.Url.Contains("/AccessDenied", StringComparison.OrdinalIgnoreCase)
                || driver.Url.Contains("/account/login", StringComparison.OrdinalIgnoreCase)
                || driver.PageSource.Contains("403")
                || driver.Title.Contains("Error", StringComparison.OrdinalIgnoreCase);

            Assert.True(accessBlocked, "O usuário regular conseguiu acessar {relativeUrl} (deveria ser restrito).");
        }

        [Fact(DisplayName = "Regular user should be blocked from user management")]
        public void ShouldBlockAccessToUserManagementPage()
        {

            string relativeUrl = "/UserManagment/Index";
            driver.Navigate().GoToUrl($"{baseUrl}{relativeUrl}");

            bool accessBlocked = driver.PageSource.Contains("Access Denied", StringComparison.OrdinalIgnoreCase)
                || driver.PageSource.Contains("Acesso negado", StringComparison.OrdinalIgnoreCase)
                || driver.Url.Contains("/AccessDenied", StringComparison.OrdinalIgnoreCase)
                || driver.Url.Contains("/account/login", StringComparison.OrdinalIgnoreCase)
                || driver.PageSource.Contains("403")
                || driver.Title.Contains("Error", StringComparison.OrdinalIgnoreCase);

            Assert.True(accessBlocked, $"O usuário regular conseguiu acessar {relativeUrl} (deveria ser restrito)");
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}