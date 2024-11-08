using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reservation.Data;
using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDBContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDBContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
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
                        return RedirectToAction("Index", "Room");
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
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new User()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress.Split('@')[0]
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            return RedirectToAction("Index", "Room");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }

    }
}
