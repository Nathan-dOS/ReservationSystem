using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Xunit;
using System; // Adicionado para TimeSpan e outras funcionalidades
using SeleniumExtras.WaitHelpers; // Adicionado para usar ExpectedConditions mais robustos

namespace ReservationTest.Tests
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

            // Encontra o botão primeiro
            var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));

            // Usa o executor de JavaScript para forçar o clique
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", submitButton);

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

        [Fact]
        public void Register_With_Existing_CPF_Should_Show_Error()
        {
            // Arrange – navega até a tela de registro
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Register"); // Certifique-se de que o caminho é '/Account/Register' (com 'A' maiúsculo se for ASP.NET Core MVC default)

            // Act – preenche o formulário com um CPF já existente
            wait.Until(d => d.FindElement(By.Id("EmailAddress"))).SendKeys("teste@duplicado.com");
            driver.FindElement(By.Id("Password")).SendKeys("SenhaForte123!");
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("SenhaForte123!");
            driver.FindElement(By.Id("Name")).SendKeys("Usuário Teste");
            driver.FindElement(By.Id("PhoneNumber")).SendKeys("11999999999");
            driver.FindElement(By.Id("CPF")).SendKeys("12345678901"); // CPF duplicado já existente no banco
            driver.FindElement(By.Id("Address")).SendKeys("Rua Exemplo, 123");

            // --- SEÇÃO CORRIGIDA ---

            // Encontra o dropdown do tipo de usuário
            var userTypeDropdownElement = wait.Until(d => d.FindElement(By.Id("UserType")));
            var userTypeDropdown = new SelectElement(userTypeDropdownElement);

            // Opção 1: Selecionar pelo texto "Comum" (Se você ajustou o enum com [Display(Name = "Comum")])
            // wait.Until(d => userTypeDropdownElement.FindElement(By.XPath(".//option[text()='Comum']")));
            // userTypeDropdown.SelectByText("Comum");

            // Opção 2: Selecionar pelo texto "Common" (Se o seu enum é 'Common' e não tem DisplayAttribute)
            // wait.Until(d => userTypeDropdownElement.FindElement(By.XPath(".//option[text()='Common']")));
            // userTypeDropdown.SelectByText("Common");

            // Opção 3 (Recomendado se o enum for numérico e o texto varia): Selecionar pelo VALOR numérico
            // Esta é a forma mais robusta se o texto visível puder mudar, mas o valor subjacente for estável.
            // O valor de "Comum" ou "Common" no seu EnumUserType provavelmente é 0 ou 1.
            // Se "Comum" for 0 (ou Common = 0), use:
            wait.Until(d => userTypeDropdownElement.FindElement(By.XPath(".//option[@value='0']"))); // Ajuste o '0' para o valor numérico correto
            userTypeDropdown.SelectByValue("0"); // Se o valor for 0 para "Comum" / "Common"

            // --- FIM DA SEÇÃO CORRIGIDA ---
            // Submete o formulário de forma robusta
            var submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit']")));

            // Scroll até o botão (necessário em alguns ambientes headless ou com UI oculta)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", submitButton);

            // Pequena espera opcional para garantir renderização final (evita click antecipado)
            Thread.Sleep(200); // Pode ser removido, mas ajuda em cenários instáveis

            // Clica no botão
            submitButton.Click();

            // Assert – verifica se apareceu uma mensagem de erro
            // Melhorar a robustez do Assert, esperando a mensagem de erro
            var errorMessage = wait.Until(d => d.FindElement(By.ClassName("text-danger")));
            Assert.True(errorMessage.Displayed, "A mensagem de erro não foi exibida.");
            Assert.Contains("CPF", errorMessage.Text);
        }

        [Fact]
        public void Register_With_Existing_Email_Should_Show_Error_Message()
        {
            // --- ARRANGE ---
            // Navega para a página de registro
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Register");

            // --- ACT ---
            // Preenche o formulário com um EMAIL QUE JÁ EXISTE NO SEU BANCO DE DADOS
            // ATENÇÃO: SUBSTITUA ESTE EMAIL POR UM QUE REALMENTE ESTEJA CADASTRADO!
            string existingEmail = "email.existente@exemplo.com";

            // Para garantir que o erro seja SOMENTE sobre o e-mail, use um CPF que você sabe que é único
            string uniqueCPF = $"444555666{new Random().Next(100, 999)}";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailAddress"))).SendKeys(existingEmail);
            driver.FindElement(By.Id("Password")).SendKeys("NovaSenhaValida123!");
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("NovaSenhaValida123!");
            driver.FindElement(By.Id("Name")).SendKeys("Usuario Email Teste");
            driver.FindElement(By.Id("PhoneNumber")).SendKeys("21977776666");
            driver.FindElement(By.Id("CPF")).SendKeys(uniqueCPF);
            driver.FindElement(By.Id("Address")).SendKeys("Rua do Email Duplicado, 789");

            // Seleciona o tipo de usuário (assumindo '0' para "Comum"/"Common")
            var userTypeDropdownElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserType")));
            var userTypeDropdown = new SelectElement(userTypeDropdownElement);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//option[@value='0']")));
            userTypeDropdown.SelectByValue("0");

            var submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit']")));

            // Scroll até o botão (necessário em alguns ambientes headless ou com UI oculta)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", submitButton);

            // Pequena espera opcional para garantir renderização final (evita click antecipado)
            Thread.Sleep(200); // Pode ser removido, mas ajuda em cenários instáveis

            // Clica no botão
            submitButton.Click();

            // --- ASSERT ---
            // Verifica se a mensagem de erro esperada para e-mail duplicado apareceu.
            // A localização e o texto da mensagem podem variar.
            try
            {
                // Opção A (mais comum para erro geral ou via TempData): Procura por uma div de alerta de perigo.
                var errorMessageDiv = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("alert-danger")));
                Assert.True(errorMessageDiv.Displayed, "A div de mensagem de erro 'alert-danger' não está visível.");
                Assert.Contains("email", errorMessageDiv.Text, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("Sorry!", errorMessageDiv.Text, StringComparison.OrdinalIgnoreCase);


                // Opção B (se a mensagem aparecer diretamente abaixo do campo de e-mail):
                // Descomente e use esta seção SE A MENSAGEM APARECER ASSIM no HTML:
                // <input id="EmailAddress" ...>
                // <span class="text-danger">O email já está em uso.</span>
                // var emailFieldErrorMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailAddress + .text-danger")));
                // Assert.True(emailFieldErrorMessage.Displayed, "A mensagem de erro abaixo do campo EmailAddress não foi exibida.");
                // Assert.Contains("email", emailFieldErrorMessage.Text, StringComparison.OrdinalIgnoreCase, "A mensagem de erro do campo EmailAddress não menciona 'email'.");

            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Nenhuma mensagem de erro de validação para email duplicado foi encontrada após a submissão do formulário.");
            }
        }

        [Fact]
        public void Register_With_Existing_CRM_Should_Show_Error_Message()
        {
            // --- ARRANGE ---
            // Navega para a página de registro
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Register");

            // --- ACT ---
            // Preenche o formulário com um CRM QUE JÁ EXISTE NO SEU BANCO DE DADOS
            // ATENÇÃO: SUBSTITUA ESTE CRM POR UM QUE REALMENTE ESTEJA CADASTRADO!
            string existingCRMNumber = "123456SP"; // Exemplo: CRM existente. Ajuste para um CRM real do seu BD.

            // Gere um email e CPF únicos para evitar conflitos com outras validações
            string uniqueEmail = $"crm_test_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
            string uniqueCPF = $"777888999{new Random().Next(100, 999)}";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailAddress"))).SendKeys(uniqueEmail);
            driver.FindElement(By.Id("Password")).SendKeys("SenhaCRM123!");
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("SenhaCRM123!");
            driver.FindElement(By.Id("Name")).SendKeys("Usuario CRM Teste");
            driver.FindElement(By.Id("PhoneNumber")).SendKeys("21966665555");
            driver.FindElement(By.Id("CPF")).SendKeys(uniqueCPF);
            driver.FindElement(By.Id("Address")).SendKeys("Avenida do CRM, 101");
            driver.FindElement(By.Id("CRMNumber")).SendKeys(existingCRMNumber); // Campo CRM

            // Seleciona o tipo de usuário como "Médico" (se aplicável, para ativar a validação do CRM)
            // Se o CRM for um campo para outros tipos de usuários, ajuste o valor.
            var userTypeDropdownElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserType")));
            var userTypeDropdown = new SelectElement(userTypeDropdownElement);

            // ATENÇÃO: Verifique o valor numérico para 'Médico' no seu EnumUserType
            // Se 'Médico' for 1, use "1". Se não tiver um 'Médico', ajuste a lógica.
            // Para este exemplo, assumo que 'Médico' tem valor '1' ou nome 'Doctor' com Display(Name="Médico")
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//option[@value='1']"))); // Altere '1' para o valor correto do enum para "Médico"
            userTypeDropdown.SelectByValue("1"); // Altere '1' para o valor correto do enum para "Médico"


            var submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit']")));

            // Scroll até o botão (necessário em alguns ambientes headless ou com UI oculta)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", submitButton);

            // Pequena espera opcional para garantir renderização final (evita click antecipado)
            Thread.Sleep(200); // Pode ser removido, mas ajuda em cenários instáveis

            // Clica no botão
            submitButton.Click();

            // --- ASSERT ---
            // Verifica se a mensagem de erro esperada para CRM duplicado apareceu.
            // A localização e o texto da mensagem podem variar.
            try
            {
                // Opção A (mais comum para erro geral ou via TempData): Procura por uma div de alerta de perigo.
                var errorMessageDiv = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("alert-danger")));

                Assert.True(errorMessageDiv.Displayed, "A div de mensagem de erro 'alert-danger' não está visível.");
                Assert.Contains("CRM", errorMessageDiv.Text, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("Sorry!", errorMessageDiv.Text, StringComparison.OrdinalIgnoreCase);


                // Opção B (se a mensagem aparecer diretamente abaixo do campo CRM):
                // Descomente e use esta seção SE A MENSAGEM APARECER ASSIM no HTML:
                // <input id="CRMNumber" ...>
                // <span class="text-danger">O número do CRM já está em uso.</span>
                // var crmFieldErrorMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#CRMNumber + .text-danger")));
                // Assert.True(crmFieldErrorMessage.Displayed, "A mensagem de erro abaixo do campo CRMNumber não foi exibida.");
                // Assert.Contains("CRM", crmFieldErrorMessage.Text, StringComparison.OrdinalIgnoreCase, "A mensagem de erro do campo CRMNumber não menciona 'CRM'.");

            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Nenhuma mensagem de erro de validação para CRM duplicado foi encontrada após a submissão do formulário.");
            }
        }

        [Fact]
        public void Register_With_MismatchedPasswords_Should_Show_Error_Message()
        {
            // --- ARRANGE ---
            // Navega para a página de registro
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Register");

            // --- ACT ---
            // Preenche o formulário com dados válidos, mas senhas diferentes
            string uniqueEmail = $"mismatch_pwd_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
            string uniqueCPF = $"333222111{new Random().Next(100, 999)}";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailAddress"))).SendKeys(uniqueEmail);
            driver.FindElement(By.Id("Password")).SendKeys("SenhaForte123!"); // Senha diferente
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("SenhaDiferente!"); // Confirmação diferente
            driver.FindElement(By.Id("Name")).SendKeys("Usuario Senha Teste");
            driver.FindElement(By.Id("PhoneNumber")).SendKeys("21955554444");
            driver.FindElement(By.Id("CPF")).SendKeys(uniqueCPF);
            driver.FindElement(By.Id("Address")).SendKeys("Rua da Senha Incorreta, 10");

            // Seleciona o tipo de usuário (assumindo '0' para "Comum"/"Common")
            var userTypeDropdownElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserType")));
            var userTypeDropdown = new SelectElement(userTypeDropdownElement);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//option[@value='0']")));
            userTypeDropdown.SelectByValue("0");


            var submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit']")));

            // Scroll até o botão (necessário em alguns ambientes headless ou com UI oculta)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", submitButton);

            // Pequena espera opcional para garantir renderização final (evita click antecipado)
            Thread.Sleep(200); // Pode ser removido, mas ajuda em cenários instáveis

            // Clica no botão
            submitButton.Click();

            // --- ASSERT ---
            // Verifica se a mensagem de erro esperada para senhas diferentes apareceu.
            // A localização e o texto da mensagem podem variar.
            try
            {
                // Opção A (mais comum para validação de campo): Mensagem de erro diretamente abaixo do campo ConfirmPassword.
                // Isso ocorre devido ao asp-validation-for="ConfirmPassword" na sua View e o [Compare] no ViewModel.
                var confirmPasswordErrorMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ConfirmPassword + .text-danger")));

                Assert.True(confirmPasswordErrorMessage.Displayed, "A mensagem de erro abaixo do campo ConfirmPassword não foi exibida.");
                // O texto da mensagem é definido pelo ErrorMessage no [Compare] de RegisterViewModel:
                // [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
                Assert.Contains("Password and Confirm Password must match", confirmPasswordErrorMessage.Text, StringComparison.OrdinalIgnoreCase);

                // Opção B (se a mensagem aparecer no resumo de validação do ModelOnly):
                // var validationSummary = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[asp-validation-summary='ModelOnly']")));
                // Assert.True(validationSummary.Displayed, "O resumo de validação ModelOnly não está visível.");
                // Assert.Contains("Password and Confirm Password must match", validationSummary.Text, StringComparison.OrdinalIgnoreCase, "O resumo de validação não contém a mensagem de senha.");

            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Nenhuma mensagem de erro de validação para senhas diferentes foi encontrada após a submissão do formulário.");
            }
        }

        [Fact]
        public void Register_With_Existing_OAB_Should_Show_Error_Message()
        {
            // --- ARRANGE ---
            // Navega para a página de registro
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Register");

            // --- ACT ---
            // Preenche o formulário com um número OAB QUE JÁ EXISTE NO SEU BANCO DE DADOS
            // ATENÇÃO: SUBSTITUA ESTE OAB POR UM QUE REALMENTE ESTEJA CADASTRADO!
            string existingOABNumber = "123456RJ"; // Exemplo: OAB existente. Ajuste para um OAB real do seu BD.

            // Gere um email e CPF únicos para evitar conflitos com outras validações
            string uniqueEmail = $"oab_test_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
            string uniqueCPF = $"101202303{new Random().Next(100, 999)}";

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailAddress"))).SendKeys(uniqueEmail);
            driver.FindElement(By.Id("Password")).SendKeys("SenhaOAB123!");
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("SenhaOAB123!");
            driver.FindElement(By.Id("Name")).SendKeys("Usuario OAB Teste");
            driver.FindElement(By.Id("PhoneNumber")).SendKeys("21944443333");
            driver.FindElement(By.Id("CPF")).SendKeys(uniqueCPF);
            driver.FindElement(By.Id("Address")).SendKeys("Rua da OAB Duplicada, 202");
            driver.FindElement(By.Id("OABNumber")).SendKeys(existingOABNumber); // Campo OAB

            // Seleciona o tipo de usuário como "Advogado" (se aplicável, para ativar a validação da OAB)
            // Se o OAB for um campo para outros tipos de usuários, ajuste o valor.
            var userTypeDropdownElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserType")));
            var userTypeDropdown = new SelectElement(userTypeDropdownElement);

            // ATENÇÃO: Verifique o valor numérico para 'Advogado' no seu EnumUserType
            // Se 'Advogado' tiver valor '2', use "2". Se não tiver, ajuste a lógica.
            // Para este exemplo, assumo que 'Advogado' tem valor '2' ou nome 'Lawyer' com Display(Name="Advogado")
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//option[@value='2']"))); // Altere '2' para o valor correto do enum para "Advogado"
            userTypeDropdown.SelectByValue("2"); // Altere '2' para o valor correto do enum para "Advogado"

            var submitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit']")));

            // Scroll até o botão (necessário em alguns ambientes headless ou com UI oculta)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", submitButton);

            // Pequena espera opcional para garantir renderização final (evita click antecipado)
            Thread.Sleep(200); // Pode ser removido, mas ajuda em cenários instáveis

            // Clica no botão
            submitButton.Click();

            // --- ASSERT ---
            // Verifica se a mensagem de erro esperada para OAB duplicada apareceu.
            // A localização e o texto da mensagem podem variar.
            try
            {
                // Opção A (mais comum para erro geral ou via TempData): Procura por uma div de alerta de perigo.
                var errorMessageDiv = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("alert-danger")));

                Assert.True(errorMessageDiv.Displayed, "A div de mensagem de erro 'alert-danger' não está visível.");
                Assert.Contains("OAB", errorMessageDiv.Text, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("Sorry!", errorMessageDiv.Text, StringComparison.OrdinalIgnoreCase);


                // Opção B (se a mensagem aparecer diretamente abaixo do campo OAB):
                // Descomente e use esta seção SE A MENSAGEM APARECER ASSIM no HTML:
                // <input id="OABNumber" ...>
                // <span class="text-danger">O número da OAB já está em uso.</span>
                // var oabFieldErrorMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#OABNumber + .text-danger")));
                // Assert.True(oabFieldErrorMessage.Displayed, "A mensagem de erro abaixo do campo OABNumber não foi exibida.");
                // Assert.Contains("OAB", oabFieldErrorMessage.Text, StringComparison.OrdinalIgnoreCase, "A mensagem de erro do campo OABNumber não menciona 'OAB'.");

            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Nenhuma mensagem de erro de validação para OAB duplicada foi encontrada após a submissão do formulário.");
            }
        }



        public void Dispose()
        {
            driver.Quit();
        }
    }
}