using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI; // Essencial para o WebDriverWait
using Xunit;

namespace Reservation.Tests.Unit
{
    public class LoginTests : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        // ATENÇÃO: Atualizei a URL para a que você informou!
        private readonly string baseUrl = "https://localhost:7105";

        public LoginTests()
        {
            // Garante que o pacote NuGet 'Selenium.WebDriver.ChromeDriver' está instalado no projeto de teste.
            driver = new FirefoxDriver();

            // Configura uma espera padrão de 10 segundos. O teste não vai esperar 10s sempre,
            // mas sim ATÉ 10s para que uma condição seja atendida.
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        //[Fact]
        //public void Login_With_Valid_Credentials()
        //{
        //    // Arrange: Navega para a página de login
        //    driver.Navigate().GoToUrl($"{baseUrl}/account/login");

        //    // Act: Preenche o formulário e clica em login
        //    // Usamos 'wait.Until' para garantir que o campo existe antes de interagir com ele.
        //    wait.Until(d => d.FindElement(By.Id("EmailAddress"))).SendKeys("teste@teste.com");
        //    driver.FindElement(By.Id("Password")).SendKeys("Teste123!");
        //    driver.FindElement(By.CssSelector("input.btn-primary[type='submit']")).Click();

        //    // Assert: Verifica se foi redirecionado para a página principal (Home)
        //    // A melhor forma de verificar um login bem-sucedido é checar a URL final.
        //    // Ajuste "/Home" se a sua página principal tiver outra URL.

        //    var logoutElement = wait.Until(d => d.FindElement(By.LinkTex("Logout")));

        //    wait.Until(d => d.Url.Contains("/home/index"));
        //    Assert.Contains("/home/index", driver.Url);
        //}

        [Fact]
        public void Login_With_Invalid_Credentials()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}/account/login");

            // Act
            wait.Until(d => d.FindElement(By.Id("EmailAddress"))).SendKeys("email-invalido@teste.com");
            driver.FindElement(By.Id("Password")).SendKeys("senha-errada");
            driver.FindElement(By.CssSelector("input.btn-primary[type='submit']")).Click();

            // Assert: Verifica se a mensagem de erro apareceu na tela
            // Esperamos o elemento com a classe 'alert-danger' aparecer.
            var errorMessageDiv = wait.Until(d => d.FindElement(By.ClassName("alert-danger")));

            // Verificamos se o elemento está visível e contém o texto esperado.
            Assert.True(errorMessageDiv.Displayed);
            Assert.Contains("Sorry!", errorMessageDiv.Text);
        }

        //[Fact]
        //public void Cancel_Button_Should_()
        //{
        //    // Arrange
        //    driver.Navigate().GoToUrl($"{baseUrl}/account/login");

        //    // Act
        //    // Usando By.LinkText por ser mais legível para pegar o <a>Cancel</a>
        //    wait.Until(d => d.FindElement(By.LinkText("Cancel"))).Click();

        //    // Assert
        //    // Verifica se voltou para a raiz do site ou para a Home.
        //    // A URL pode ser exatamente a base URL ou conter /Home.
        //    wait.Until(d => d.Url != $"{baseUrl}/account/login"); // Espera sair da página de login
        //    Assert.Equal($"{baseUrl}/", driver.Url, ignoreCase: true); // Verifica se voltou para a raiz
        //}


        // O Dispose é chamado pelo xUnit após cada teste na classe, garantindo que o navegador feche.
        public void Dispose()
        {
            driver.Quit();
        }
    }
}