using Microsoft.AspNetCore.Identity;
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


            if (User.IsInRole("general")) // Se for Gerente Geral exibe todos os relatórios
            {
                var history = await _historyRepository.GetAllHistoryAsync();
                if (history.Any())
                {
                    foreach (var item in history)
                    {
                        Console.WriteLine($"Reserva: {item.ReserveId}, Sala: {item.Room?.RoomNumber}");
                    }
                }

                return View(history);
            }
            else  // Se for 
            {
                var history = await _historyRepository.GetHistoryByUserIDAsync(userId);
                return View(history);
            }
            

        }
    }
}
