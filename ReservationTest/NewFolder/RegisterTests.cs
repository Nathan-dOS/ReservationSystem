using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Xunit;
using System; // Adicionado para TimeSpan e outras funcionalidades
using SeleniumExtras.WaitHelpers; // Adicionado para usar ExpectedConditions mais robustos

namespace Reservation.Tests.Unit
{
    public class RegisterTests : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        // URL base atualizada para HTTP e a porta correta
        private readonly string baseUrl = "http://localhost:5139";

        public RegisterTests()
        {
            driver = new FirefoxDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void Register_Successfully_Creates_New_User() // Nome do teste atualizado
        {
            // Arrange – navega até a tela de registro
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Register");

            // Act – preenche o formulário com dados VÁLIDOS e ÚNICOS para um novo cadastro
            // IMPORTANTE: Gere um email único a cada execução para garantir um novo cadastro!
            string uniqueEmail = $"teste_cadastro_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
            string uniqueCPF = $"111222333{new Random().Next(100, 999)}"; // Gera um CPF quase único para fins de teste

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailAddress"))).SendKeys(uniqueEmail);
            driver.FindElement(By.Id("Password")).SendKeys("SenhaNova123!"); // Use uma senha forte válida
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("SenhaNova123!");
            driver.FindElement(By.Id("Name")).SendKeys("Novo Usuário Teste");
            driver.FindElement(By.Id("PhoneNumber")).SendKeys("21987654321");
            driver.FindElement(By.Id("CPF")).SendKeys(uniqueCPF);
            driver.FindElement(By.Id("Address")).SendKeys("Rua dos Testes, 456");

            // Encontra o dropdown do tipo de usuário
            var userTypeDropdownElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserType")));
            var userTypeDropdown = new SelectElement(userTypeDropdownElement);

            // Selecionar pelo VALOR numérico (assumindo '0' para "Comum"/"Common")
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//option[@value='0']")));
            userTypeDropdown.SelectByValue("0");

            // Submete o formulário
            driver.FindElement(By.CssSelector("input[type='submit']")).Click();

            // Assert – Verifica o sucesso do cadastro
            // Normalmente, após um cadastro bem-sucedido, o usuário é redirecionado para:
            // 1. Uma página de confirmação de registro.
            // 2. A página de login (para ele logar).
            // 3. A página inicial (se ele for automaticamente logado).

            // Escolha UMA destas opções de Assert com base no comportamento do seu aplicativo:

            // Opção A: Redirecionamento para a página de login
            // (Comum para ASP.NET Core Identity após registro bem-sucedido)
            wait.Until(ExpectedConditions.UrlContains("/Account/Login"));
            Assert.Contains("/Account/Login", driver.Url, StringComparison.OrdinalIgnoreCase);

            // Opção B: Redirecionamento para uma página inicial ou dashboard (se o usuário for logado automaticamente)
            // wait.Until(ExpectedConditions.UrlContains("/Home/Index"));
            // Assert.Contains("/Home/Index", driver.Url, StringComparison.OrdinalIgnoreCase, "Não foi redirecionado para a página inicial após o cadastro bem-sucedido.");

            // Opção C: Mensagem de sucesso em alguma parte da página (menos comum, mas possível)
            // Se sua aplicação exibir uma mensagem de "Sucesso!" na mesma página ou em uma nova página.
            // var successMessageDiv = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("alguma-classe-de-sucesso")));
            // Assert.True(successMessageDiv.Displayed, "A mensagem de sucesso não foi exibida.");
            // Assert.Contains("Cadastro realizado com sucesso", successMessageDiv.Text, StringComparison.OrdinalIgnoreCase);

            // Se o teste chegar até aqui, significa que o Assert acima foi bem-sucedido.
            // Você pode adicionar um Console.WriteLine para ver no output do terminal do teste.
            Console.WriteLine("Cadastro de novo usuário realizado com sucesso!");
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}