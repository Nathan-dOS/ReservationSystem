using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using OpenQA.Selenium.Firefox;


namespace Reservation.Tests.Ui
{
    public class ReservationUiTests : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;
        private readonly string baseUrl = "http://localhost:5139";

        public ReservationUiTests()
        {
            // Inicializa o FirefoxDriver (pode configurar opções se quiser)
            driver = new FirefoxDriver();

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // var options = new ChromeOptions();
            // options.BinaryLocation = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
            // options.AddArgument("--ignore-certificate-errors");
            // options.AcceptInsecureCertificates = true;
            // options.AddArgument("--headless=new");

            // driver = new ChromeDriver(options);
            wait  = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 1) Login
            driver.Navigate().GoToUrl($"{baseUrl}/account/login");

            var email = wait.Until(d => d.FindElement(By.Id("EmailAddress")));
            email.Clear();
            email.SendKeys("teste@teste.com");

            var pwd = driver.FindElement(By.Id("Password"));
            pwd.Clear();
            pwd.SendKeys("Teste123!");

            var btn = driver.FindElement(By.CssSelector("button[type='submit'], input[type='submit']"));
            btn.Click();

            // 2) Aguarda até o botão "Ver Salas Disponíveis" aparecer na home
            wait.Until(d => d.FindElement(By.LinkText("Ver Salas Disponíveis")));
        }

        [Fact]
        public void CreateReservation_InvalidBusinessHours()
        {
            // Navega para detalhe da sala
            driver.Navigate().GoToUrl($"{baseUrl}/Room/Detail/1");

            // Preenche data e horários inválidos
            var dateField = wait.Until(d => d.FindElement(By.Id("ReserveDate")));
            dateField.Clear();
            dateField.SendKeys(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));

            driver.FindElement(By.Id("ReserveStart")).Clear();
            driver.FindElement(By.Id("ReserveStart")).SendKeys("07:00");

            driver.FindElement(By.Id("ReserveEnd")).Clear();
            driver.FindElement(By.Id("ReserveEnd")).SendKeys("10:00");

            // Clica em reservar
            var submit = driver.FindElement(By.CssSelector("button[type='submit'], input[type='submit']"));

            // A SOLUÇÃO: Usar o clique via JavaScript, que ignora sobreposições.
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", submit);

            // Aguarda e verifica o alerta de erro
            var alert = wait.Until(d => d.FindElement(By.ClassName("alert-danger")));
            Assert.True(alert.Displayed);
            Assert.Contains("08:00", alert.Text, StringComparison.OrdinalIgnoreCase);
        }
        [Fact]
        public void CreateReservation_With_ValidTimes()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Room/Detail/1");

            var dateField = wait.Until(d => d.FindElement(By.Id("ReserveDate")));
            dateField.Clear();
            dateField.SendKeys(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));

            driver.FindElement(By.Id("ReserveStart")).Clear();
            driver.FindElement(By.Id("ReserveStart")).SendKeys("10:00");

            driver.FindElement(By.Id("ReserveEnd")).Clear();
            driver.FindElement(By.Id("ReserveEnd")).SendKeys("12:00");

            var submit = driver.FindElement(By.CssSelector("button[type='submit'], input[type='submit']"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", submit);

            // Espera até que a URL mude para a página de confirmação
            wait.Until(d => d.Url.Contains("/Reserve/Confirmation", StringComparison.OrdinalIgnoreCase));

            // Verifica que a URL final está correta
            Assert.Contains("/Reserve/Confirmation", driver.Url, StringComparison.OrdinalIgnoreCase);
        }


        [Fact]
        public void CreateReservation_With_PastDate()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Room/Detail/1");

            var dateField = wait.Until(d => d.FindElement(By.Id("ReserveDate")));
            dateField.Clear();
            dateField.SendKeys(DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")); // data passada

            driver.FindElement(By.Id("ReserveStart")).Clear();
            driver.FindElement(By.Id("ReserveStart")).SendKeys("10:00");

            driver.FindElement(By.Id("ReserveEnd")).Clear();
            driver.FindElement(By.Id("ReserveEnd")).SendKeys("12:00");

            var submit = driver.FindElement(By.CssSelector("button[type='submit'], input[type='submit']"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", submit);

            var errorAlert = wait.Until(d => d.FindElement(By.ClassName("alert-danger")));
            Assert.True(errorAlert.Displayed);
            Assert.Contains("invalid", errorAlert.Text, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}