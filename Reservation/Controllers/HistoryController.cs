using Microsoft.AspNetCore.Mvc;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.Repository;
using System.Security.Claims;

namespace Reservation.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IReserveHistoryRepository _historyRepository;

        public HistoryController(IReserveHistoryRepository reserveHistory)
        {
            _historyRepository = reserveHistory;
        }

        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Usuário não autenticado.";
                return RedirectToAction("Login", "Account");
            }

            // Pega historico pelo ID do usuario
            var history = await _historyRepository.GetHistoryByUserIDAsync(userId);

           
            return View(history);

        }
    }
}
