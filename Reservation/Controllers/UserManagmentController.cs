using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;
using Reservation.ViewModel;
using System.Data;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Reservation.Controllers
{
    public class UserManagmentController : Controller
    {
        private readonly IUserManagmenteRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserManagmentController(IUserManagmenteRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }
        [Authorize(Roles = " general")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersWithRolesAsync();
            var UserLogged = await _userManager.GetUserAsync(User);

            if (UserLogged == null)
            {
                TempData["ErrorMessage"] = "Usuario nao esta logado ";
                return RedirectToAction("Index");

            }

            var rolesOfUserLogged = await _userManager.GetRolesAsync(UserLogged);

            var userRoles = new List<UserManagmentViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserManagmentViewModel
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CPF = user.CPF,
                    PhoneNumber = user.PhoneNumber,
                    OABNumber = user.OABNumber,
                    CRMNumber = user.CRMNumber,
                    BannedUntil = user.BannedUntil,
                    BanReason = user.BanReason,
                    Roles = roles.ToList(),
                    RolesUserLogged = rolesOfUserLogged.ToList(),
                });
            }

            return View(userRoles);
        }
        // Metodo de banir usuario
        /*
        public async Task<IActionResult> BanUser(string userID)
        {
            var user = await _userRepository.GetUserByIdAsync(userID);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuário nao encontrado";
                return RedirectToAction("Index");
            }

            user.IsBanned = true;
            user.BannedUntil = DateTime.Now.AddMinutes(1); // 1 Minuto de ban
            await _userManager.UpdateAsync(user); // Update User

            TempData["SuccessMessage"] = "Usuario banido por " + user.BannedUntil + "Minutes";
            
            return RedirectToAction("Index", "UserManagment");

        }
        */


        [HttpGet]
        public async Task<IActionResult> BanUser(string userID)
        {
            // Simulação de recuperação do usuário do banco de dados
            var user = await _userRepository.GetUserByIdAsync(userID);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            var users = await _userRepository.GetAllUsersWithRolesAsync();
            var UserLogged = await _userManager.GetUserAsync(User);

            if (UserLogged == null)
            {
                TempData["ErrorMessage"] = "Usuario nao esta logado ";
                return RedirectToAction("Index");

            }

            var roles = await _userManager.GetRolesAsync(user);

            var rolesOfUserLogged = await _userManager.GetRolesAsync(UserLogged);

            var model = new UserManagmentViewModel
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                CPF = user.CPF,
                PhoneNumber = user.PhoneNumber,
                OABNumber = user.OABNumber,
                CRMNumber = user.CRMNumber,
                BannedUntil = user.BannedUntil,
                BanReason = user.BanReason,
                Roles = roles.ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(UserManagmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            // Simulação de salvar os dados no banco de dados
            if (model.BanReason == null)
            {
                TempData["ErrorMessage"] = "Ban reason cannot be null.";
                return View(model);
            }

            await Task.Run(() => _userRepository.BanUser(model.UserId, model.BanReason, model.BannedUntil));

            return RedirectToAction("Index", "UserManagment");
        }


        public IActionResult BanView()
        {
            return View();
        }


        public async Task<IActionResult> PromoteToAdmin(string userID)
        {
            var user = await _userRepository.GetUserByIdAsync(userID);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuario nao encontrado";
                return RedirectToAction("Index");
            }
            // Pega as roles do usuario
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("admin"))
            {
                TempData["ErrorMessage"] = "Usuario já é admin";
                return RedirectToAction("Index");

            }

            if (roles.Contains("user"))
            {
                await _userManager.RemoveFromRoleAsync(user, "user");
            }

            // Adiciona como Admin
            var result = await _userManager.AddToRoleAsync(user, "admin");

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Usuário promovido a Admin do prédio com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Falha ao promover o usuário para Admin.";
            }

            return RedirectToAction("Index");

        }


        public async Task<IActionResult> RemoveAdmin(string userID)
        {
            //Usuario logado
            var userLogged = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Pega ID do usario pra banir
            var user = await _userRepository.GetUserByIdAsync(userID);

            if (userLogged == userID)
            {
                TempData["ErrorMessage"] = "Voce não pode se demitir";
                return RedirectToAction("Index");
            }


            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuario nao encontrado";
                return RedirectToAction("Index");
            }
            // Pega as roles
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("admin"))
            { // Remove Da tabela roles
                var result = await _userManager.RemoveFromRoleAsync(user, "admin");

                if (!roles.Contains("user"))
                {
                    await _userManager.AddToRoleAsync(user, "user");
                }

                if (result.Succeeded)
                {

                    TempData["ErrorMessage"] = "Usuario removido como admin";
                }

            }
            return RedirectToAction("Index");

        }



    }
}
