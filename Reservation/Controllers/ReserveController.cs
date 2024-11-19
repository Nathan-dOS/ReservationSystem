using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservation.Interfaces;
using Reservation.Models;
using Reservation.ViewModel;

namespace Reservation.Controllers
{
    public class ReserveController : Controller
    {
        private readonly IReserveRepository _reserveRepository;
        private readonly IRoomRepository _roomRepository;
        public ReserveController(IReserveRepository reserveRepository, IRoomRepository roomRepository)
        {
            _reserveRepository = reserveRepository;
            _roomRepository = roomRepository;

        }

        [Authorize]
        public IActionResult Create(int roomId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (roomId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var reserveVM = new CreateReserveViewModel
            {
                RoomId = roomId,
                UserId = userId,
            };

            return View(reserveVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomDetailViewModel roomDetail)
        {

            

            if (!ModelState.IsValid)
            {
                // Se o modelo não for válido, obteremos os detalhes da sala e passaremos para a view novamente
                var room = await _roomRepository.GetByIdAsync(roomDetail.CreateReserveViewModel.RoomId);


                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }


                if (room == null)
                {
                    return View("Error"); // Se a sala não existir, mostra um erro
                }

       

                // Passa os detalhes da sala junto com o modelo de reserva de volta para a View de detalhes
                var roomDetailViewModel = new RoomDetailViewModel
                {
                    Room = room,
                    CreateReserveViewModel = roomDetail.CreateReserveViewModel,
                   
                };

                // Exibe a página de detalhes com os erros de validação
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });
            }
            // Definindo limite de horario inicio e final
            var openingTime = new TimeOnly(8, 0);
            var closingTime = new TimeOnly(20, 0);
            // Verifica se o horario nao ultrapassa o horario comercial
            if (roomDetail.CreateReserveViewModel.ReserveStart < openingTime ||
            roomDetail.CreateReserveViewModel.ReserveEnd > closingTime)
            {
                TempData["ErrorMessage"] = "Os horários de reserva devem estar entre 08:00 e 20:00.";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });
            }
            // Garante que o inicio é anterior ao horario do fim
            if (roomDetail.CreateReserveViewModel.ReserveStart >= roomDetail.CreateReserveViewModel.ReserveEnd)
            {
                TempData["ErrorMessage"] = "O horario de inicio deve ser anterior ao horario do final. ";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });

            }

            var existingReserve = await _reserveRepository.GetReserveByRoomAndDateAsync(
             roomDetail.CreateReserveViewModel.RoomId, roomDetail.CreateReserveViewModel.ReserveDate,
             roomDetail.CreateReserveViewModel.ReserveStart, roomDetail.CreateReserveViewModel.ReserveEnd);

            if (existingReserve != null) // Modifiquei aqui, inclui o TempData ao inves do AddModelErro. (Até funcionava, mas nao tava exibindo msg pro usuario)
            {

                // ModelState.AddModelError("CreateReserveViewModel.ReserveDate", "Já existe uma reserva para esta sala neste horário.");
                TempData["ErrorMessage"] = "Já existe uma reserva para esta sala neste horário.";
                return RedirectToAction("Detail", "Room", new { id = roomDetail.CreateReserveViewModel.RoomId });
            }

            var reserve = new Reserve
            {
                RoomId = roomDetail.CreateReserveViewModel.RoomId,
                UserId = roomDetail.CreateReserveViewModel.UserId,
                ReserveDate = roomDetail.CreateReserveViewModel.ReserveDate,
                ReserveStart = roomDetail.CreateReserveViewModel.ReserveStart,
                ReserveEnd = roomDetail.CreateReserveViewModel.ReserveEnd,
                ReserveStatus = roomDetail.CreateReserveViewModel.ReserveStatus,
                RentPrice = roomDetail.CreateReserveViewModel.RentPrice,
            };

            _reserveRepository.AddReserve(reserve);
            
            return RedirectToAction("Confirmation", "Reserve");


        }

        public IActionResult Confirmation()
        {
            return View();
        }

    }
}
