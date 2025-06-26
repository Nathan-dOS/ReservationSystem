using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Adicionado para IHostedService
using Xunit;
using System; // Necessário para Guid, DateTime, DateOnly, TimeOnly
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq; // Para SingleOrDefault

// Seus namespaces de modelo e enum
using Reservation.Data;
using Reservation.Models;
using Reservation.ViewModel;
using Reservation.Data.Enum;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Reservation.Tests.Integration
{
    // Define um fixture de classe que inicializa a aplicação web para os testes
    public class ReserveIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ReserveIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Configura a fábrica da aplicação web para usar um banco de dados em memória
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remover o DbContext configurado no Program.cs real
                    // É importante remover as configurações existentes para evitar conflitos
                    var descriptorDbContext = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));
                    if (descriptorDbContext != null)
                    {
                        services.Remove(descriptorDbContext);
                    }

                    // ***** MUDANÇA CRUCIAL AQUI: REMOVER BanCleanupService *****
                    // Remover o IHostedService BanCleanupService para que ele não execute
                    // e tente acessar o DB antes das tabelas serem criadas no teste.
                    var descriptorHostedService = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(Reservation.Services.BanCleanupService));
                    if (descriptorHostedService != null)
                    {
                        services.Remove(descriptorHostedService);
                    }
                    // ***** FIM DA MUDANÇA *****

                    // ***** MUDANÇA CRUCIAL AQUI: Configurar o DbContext como Singleton para SQLite em memória *****
                    // Isso garante que a mesma instância do DB em memória seja usada e populada
                    // para todas as operações dentro do contexto do teste, incluindo o SeedData.
                    services.AddSingleton(new DbContextOptionsBuilder<ApplicationDBContext>()
                        .UseSqlite("DataSource=file::memory:?cache=shared") // 'cache=shared' permite múltiplas conexões
                        .Options);
                    // Registra o DbContext para que ele possa ser injetado como Singleton no serviço
                    services.AddSingleton<ApplicationDBContext>();
                    // ***** FIM DA MUDANÇA *****

                    // Construir um provedor de serviços para aplicar as migrações e seedar dados
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDBContext>();

                        // ***** MUDANÇA CRUCIAL AQUI *****
                        // Aplicar migrações ao banco de dados em memória.
                        // Isso cria todas as tabelas, incluindo as do Identity (AspNetUsers, etc.).
                        db.Database.Migrate();

                        // Agora que as tabelas existem, você pode seedar os dados com segurança
                        SeedDataForTests(db, scopedServices).Wait();
                    }
                });
            });

            // Cria um HttpClient para fazer requisições HTTP para o aplicativo em memória.
            // AllowAutoRedirect = false é crucial para testar redirecionamentos (HTTP 302/301)
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        // Método para popular o banco de dados de teste com dados iniciais
        private async Task SeedDataForTests(ApplicationDBContext context, IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed Roles
            // Cria as roles "Comum" e "Médico" se elas ainda não existirem no DB em memória
            if (!await roleManager.RoleExistsAsync("Comum"))
            {
                await roleManager.CreateAsync(new IdentityRole("Comum"));
            }
            if (!await roleManager.RoleExistsAsync("Médico"))
            {
                await roleManager.CreateAsync(new IdentityRole("Médico"));
            }

            // Seed User
            // Cria um usuário de teste se ele ainda não existir no DB em memória
            var testUser = await userManager.FindByEmailAsync("integration.testuser@example.com");
            if (testUser == null)
            {
                testUser = new User
                {
                    UserName = "integration.testuser@example.com",
                    Email = "integration.testuser@example.com",
                    EmailConfirmed = true,
                    CPF = "11111111111",
                    Name = "Usuario Integracao",
                    PhoneNumber = "1234567890",
                    Address = "Rua Integracao, 1",
                };
                var userCreationResult = await userManager.CreateAsync(testUser, "Test@123"); // Cria o usuário com senha
                if (userCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, "Comum"); // Adiciona o usuário à role "Comum"
                }
            }

            // Seed Room
            // Cria uma sala de teste se ela ainda não existir no DB em memória
            var testRoom = await context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == "TEST-001");
            if (testRoom == null)
            {
                testRoom = new Room
                {
                    RoomNumber = "TEST-001",
                    Capacity = 5,
                    SizeInSquareMeters = 25.0f,
                    RoomStatus = EnumRoomStatus.Available, // Usando um valor válido do seu EnumRoomStatus
                    RoomType = EnumRoomType.MedicalOffice, // Usando um valor válido do seu EnumRoomType
                    RoomPrice = 50.0f, // Preço da sala
                    HasInternet = true,
                    HasSecurityCamera = false,
                    HasAirConditioning = true
                };
                context.Rooms.Add(testRoom);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task CreateReserve_ValidData_ReturnsRedirectToConfirmationAndCreatesReservation()
        {
            // ARRANGE
            // Obter o contexto do DB e o UserManager do service provider da fábrica de testes.
            // É importante usar 'CreateScope()' para obter instâncias scoped se o serviço não for Singleton.
            var dbContext = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var userManager = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();

            // Obter o usuário e a sala que foram seeded no Setup do teste
            var testUser = await userManager.FindByEmailAsync("integration.testuser@example.com");
            var testRoom = await dbContext.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == "TEST-001");

            // Simular o login para obter um cookie de autenticação
            var loginRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login");
            loginRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"EmailAddress", "integration.testuser@example.com"},
                {"Password", "Test@123"}
            });
            var loginResponse = await _client.SendAsync(loginRequest);

            // Verifica que houve um redirecionamento (302 Found)
            Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);
            Assert.NotNull(loginResponse.Headers.Location);

            // ***** MUDANÇA CRUCIAL AQUI *****
            // Usar String.Equals() para a comparação e Assert.True() para a asserção
            bool isRedirectToRoot = string.Equals("/", loginResponse.Headers.Location.ToString(), StringComparison.OrdinalIgnoreCase);
            Assert.True(isRedirectToRoot, $"Não foi redirecionado para a URL raiz esperada. URL real: {loginResponse.Headers.Location.ToString()}");
            // ***** FIM DA MUDANÇA *****

            // Definir uma data de reserva futura e horários
            var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1)); // Data do dia seguinte
            var reserveStart = new TimeOnly(9, 0);  // 09:00
            var reserveEnd = new TimeOnly(10, 0);   // 10:00

            // Montar o CreateReserveViewModel com os dados da reserva
            var createReserveViewModel = new CreateReserveViewModel
            {
                RoomId = testRoom.RoomId, // ID da sala seeded
                UserId = testUser.Id, // ID do usuário logado
                ReserveDate = tomorrow,
                ReserveStart = reserveStart,
                ReserveEnd = reserveEnd,
                ReserveStatus = EnumReserveStatus.Validated.ToString(), // O status que será enviado no ViewModel
                RentPrice = testRoom.RoomPrice, // Preço da sala
            };

            // Montar o RoomDetailViewModel que o controlador espera no POST
            var roomDetailViewModel = new RoomDetailViewModel
            {
                CreateReserveViewModel = createReserveViewModel,
                RoomEquipments = new List<EquipmentViewModel>() // Assumindo nenhum equipamento selecionado para este teste.
                                                                // Adicione aqui se o envio de equipamentos for necessário para o sucesso da reserva.
            };

            // Serializar o RoomDetailViewModel para JSON para enviar no corpo da requisição POST
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(roomDetailViewModel),
                Encoding.UTF8,
                "application/json"
            );

            // ACT
            // Enviar a requisição POST para o endpoint de criação de reserva
            var response = await _client.PostAsync("/Reserve/Create", jsonContent);

            // ASSERT
            // 1. Verificar o status code da resposta HTTP (deve ser um redirecionamento 302 Found)
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            // 2. Verificar o cabeçalho Location do redirecionamento
            // O controlador redireciona para "/Reserve/Confirmation" após uma reserva bem-sucedida
            Assert.NotNull(response.Headers.Location);
            Assert.Contains("/Reserve/Confirmation", response.Headers.Location.ToString(), StringComparison.OrdinalIgnoreCase);

            // 3. Verificar se a reserva foi realmente criada no banco de dados
            // Use um novo escopo para o DbContext para garantir que você está lendo o estado atual do DB em memória
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContextForAssertion = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var createdReserve = await dbContextForAssertion.Reserves
                    .FirstOrDefaultAsync(r => r.RoomId == testRoom.RoomId &&
                                            r.UserId == testUser.Id &&
                                            r.ReserveDate == tomorrow &&
                                            r.ReserveStart == reserveStart &&
                                            r.ReserveEnd == reserveEnd);

                Assert.NotNull(createdReserve); // A reserva deve existir no banco de dados

                // Asserts adicionais para verificar as propriedades da reserva criada
                // VERIFIQUE SE O "ReserveStatus" NO SEU MODELO "Reserve.cs" É String ou Enum.
                // Se for STRING no Reserve.cs:
                // Assert.Equal(((int)EnumReserveStatus.Validated).ToString(), createdReserve.ReserveStatus);
                // Se for EnumReserveStatus no Reserve.cs:
                Assert.Equal(EnumReserveStatus.Validated, createdReserve.ReserveStatus);

                Assert.Equal(testRoom.RoomPrice, createdReserve.RentPrice);
                // Adicione mais asserts aqui para verificar outras propriedades, como equipamentos
            }
        }
    }
}