using Microsoft.AspNetCore.Mvc;
using Reservation.Interfaces;
using Reservation.Repository;
using Reservation.ViewModel;

namespace Reservation.Controllers
{
    public class UserManagmentController : Controller
    {
        private readonly IUserManagmenteRepository _userRepository;

        public UserManagmentController(IUserManagmenteRepository userManagment)
        {
             _userRepository = userManagment;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersWithRolesAsync();

            return View(users);
        }

      
    }
}
