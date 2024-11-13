using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Controllers
{
    public class ReserveController : Controller
    {
        private readonly IReserveRepository _reserveRepository;
        public ReserveController(IReserveRepository reserveRepository)
        {
            _reserveRepository = reserveRepository;
 
        }

        [Authorize]
        public IActionResult Create(int roomId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var reserveVM = new CreateReserveViewModel
            {
                RoomId = roomId,
                UserId = userId,
            };

            return View(reserveVM);

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReserveViewModel createReserveVM)
        {
            if(!ModelState.IsValid)
            {
                return View(createReserveVM);
            }

            var existingReserve = await _reserveRepository.GetReserveByRoomAndDateAsync(createReserveVM.RoomId, createReserveVM.ReserveDate,
                createReserveVM.ReserveEnd, createReserveVM.ReserveEnd);

            if (existingReserve != null)
            {
                ModelState.AddModelError("ReserveDate", "Já existe uma reserva para esta sala neste horário.");
                return View(createReserveVM);

            }

            var reserve = new Reserve
            {
                RoomId = createReserveVM.RoomId,
                UserId = createReserveVM.UserId,
                ReserveDate = createReserveVM.ReserveDate,
                ReserveStart = createReserveVM.ReserveStart,
                ReserveEnd = createReserveVM.ReserveEnd,
                ReserveStatus = createReserveVM.ReserveStatus,
                RentPrice = createReserveVM.RentPrice,
            };

            _reserveRepository.AddReserve(reserve);

            return RedirectToAction("Home", "Home");


        }

    }
}
