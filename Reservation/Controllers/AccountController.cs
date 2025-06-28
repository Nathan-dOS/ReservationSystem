using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reservation.Data;
using Reservation.Models;
using Reservation.ViewModel;
using System.Security.Claims;

namespace Reservation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDBContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDBContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            ViewData["HideFooter"] = true;
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if(!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);

            if (user != null)
            { // User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    // Pass correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                // Password is incorrect
                TempData["Error"] = "Wrong Credentials. Please, try again";
                return View(loginVM);

            }
            // User not found
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginVM);
        }
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            ViewData["HideFooter"] = true;
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel); // Verifica se respeita as condições do banco de dados

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress); // Procura se há usuario com esse email no bd
            if (user != null) // Caso tenha, exiba erro
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var isMedico = !string.IsNullOrEmpty(registerViewModel.CRMNumber);
            var isAdvogado = !string.IsNullOrEmpty(registerViewModel.OABNumber);

            var newUser = new User() // Caso contrario, cria user
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress, // Define o Email como o UserName automaticamente Isso aqui é gambiarra
                Name = registerViewModel.Name,
                Address = registerViewModel.Address,
                PhoneNumber = registerViewModel.PhoneNumber,
                CPF = registerViewModel.CPF,
                OABNumber = isAdvogado ? registerViewModel.OABNumber : null,
                CRMNumber = isMedico ? registerViewModel.CRMNumber : null,
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            // Esse IF pesquisei para resolver o problema que só aceitava a role user no banco de dados
            if (!_roleManager.RoleExistsAsync(registerViewModel.UserType.ToString()).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(registerViewModel.UserType.ToString()));
            }

             

            if (newUserResponse.Succeeded) // Se der certo, add role ao usuario
                await _userManager.AddToRoleAsync(newUser, registerViewModel.UserType.ToString()); 

            // Se for medico, cadastra na claim 
            if(isMedico)
            {
                await _userManager.AddClaimAsync(newUser, new Claim("IsMedico", "true"));
                await _userManager.AddClaimAsync(newUser, new Claim("CRMNumber", registerViewModel.CRMNumber));
            }

            // Se for advogado cadastra na claim
            if(isAdvogado)
            {
                await _userManager.AddClaimAsync(newUser, new Claim("IsAdvogado", "true"));
                await _userManager.AddClaimAsync(newUser, new Claim("OABNumber", registerViewModel.OABNumber));
            }

            return RedirectToAction("Index", "Room");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}