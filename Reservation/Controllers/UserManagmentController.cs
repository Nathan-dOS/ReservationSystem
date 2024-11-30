using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;
using Reservation.ViewModel;
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
        [Authorize(Roles = "admin,general")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersWithRolesAsync();

            return View(users);
        }

        public async Task<IActionResult> BanUser(string userID)
        {
            var user = await _userRepository.GetUserByIdAsync(userID);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuário nao encontrado";
                return RedirectToAction("Index");
            }

            user.IsBanned = true;
            user.BannedUntil = DateTime.UtcNow.AddMinutes(1);
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = "Usuario banido por " + user.BannedUntil + "Minutes";
            
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

            var roles = await _userManager.GetRolesAsync(user);

            if(roles.Contains("admin"))
            {
                TempData["ErrorMessage"] = "Usuario já é gerente ou admin";
                return RedirectToAction("Index");

            }

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
            var user = await _userRepository.GetUserByIdAsync(userID);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuario nao encontrado";
                return RedirectToAction("Index");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("admin"))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, "admin");

                if(result.Succeeded)
                {
                    TempData["ErrorMessage"] = "Usuario removido como admin";
                }

            }
            return RedirectToAction("Index");

        }

       

    }
}
