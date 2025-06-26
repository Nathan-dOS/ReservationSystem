using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI; // Essencial para o WebDriverWait
using Xunit;

namespace ReservationTeste.Tests
{
    public class LoginTests : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        // ATENÇÃO: Atualizei a URL para a que você informou!
        private readonly string baseUrl = "https://localhost:7105";

        private void OpenOffcanvasMenu()
        {
            // Abre o menu lateral (offcanvas) clicando no botão de menu
            wait.Until(d => d.FindElement(By.ClassName("menu-button"))).Click();
            wait.Until(d => d.FindElement(By.Id("offcanvasMenu")).GetAttribute("class").Contains("show"));
        }

        public LoginTests()
        {
            // Garante que o pacote NuGet 'Selenium.WebDriver.ChromeDriver' está instalado no projeto de teste.
            driver = new FirefoxDriver();

            // Configura uma espera padrão de 10 segundos. O teste não vai esperar 10s sempre,
            // mas sim ATÉ 10s para que uma condição seja atendida.
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void Login_With_Valid_Credentials()
        {
            // --- FASE 1: LOGIN ---
            driver.Navigate().GoToUrl($"{baseUrl}/account/login");
            wait.Until(d => d.FindElement(By.Id("EmailAddress"))).SendKeys("teste@teste.com");
            driver.FindElement(By.Id("Password")).SendKeys("Teste123!");
            driver.FindElement(By.CssSelector("input.btn-primary[type='submit']")).Click();

            // Confirma que o login foi bem sucedido esperando a URL mudar para a home
            wait.Until(d => d.Url == $"{baseUrl}/");

            // --- FASE 2: LOGOUT ---
            // Abre o menu lateral agora como um usuário logado
            OpenOffcanvasMenu();

            // Clica no menu "Conta" para expandi-lo
            var contaMenu = wait.Until(d => d.FindElement(By.CssSelector("a[href='#submenu1']")));
            contaMenu.Click();

            // Clica no link de "Logout" que agora está visível
            var logoutLink = wait.Until(d => d.FindElement(By.LinkText("Logout")));
            logoutLink.Click();

            // --- FASE 3: ASSERT FINAL ---
            // Após o logout, devemos voltar para a home page.
            wait.Until(d => d.Url == $"{baseUrl}/");

            // E para confirmar, vamos abrir o menu de novo e ver se o botão de Login voltou a aparecer.
            OpenOffcanvasMenu();
            var loginButton = driver.FindElement(By.XPath("//a[contains(@href, '/Account/Login')]/button"));
            Assert.True(loginButton.Displayed, "O botão de Login não apareceu após o logout.");
        }


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

        [Fact]
        public void Cancel_Button_Login_()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}/account/login");

            // Act
            // Usando By.LinkText por ser mais legível para pegar o <a>Cancel</a>
            wait.Until(d => d.FindElement(By.LinkText("Cancel"))).Click();

            // Assert
            // Verifica se voltou para a raiz do site ou para a Home.
            // A URL pode ser exatamente a base URL ou conter /Home.
            wait.Until(d => d.Url != $"{baseUrl}/account/login"); // Espera sair da página de login
            Assert.Equal($"{baseUrl}/", driver.Url, ignoreCase: true); // Verifica se voltou para a raiz
        }
        [Fact]
        public void Access_RestrictedPage()
        {
            // Começa deslogado
            driver.Manage().Cookies.DeleteAllCookies();

            // Tenta acessar uma página protegida
            string restrictedUrl = $"{baseUrl}/Room/Create";
            driver.Navigate().GoToUrl(restrictedUrl);

            // Espera redirecionamento para login
            wait.Until(d => d.Url.Contains("/Account/Login"));

            Assert.Contains("/Account/Login", driver.Url, StringComparison.OrdinalIgnoreCase);
        }

        // O Dispose é chamado pelo xUnit após cada teste na classe, garantindo que o navegador feche.
        public void Dispose()
        {
            driver.Quit();
        }
    }
}